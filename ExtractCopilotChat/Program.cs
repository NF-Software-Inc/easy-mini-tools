using ExtractCopilotChat;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Set variables
if (args.Length < 1)
	Console.WriteLine("Usage: dotnet run -- <inputDir> [outputDir]");

var inputDir = args.Length > 0 ? Path.GetFullPath(args[0]) : Console.ReadLine();

if (Directory.Exists(inputDir) == false)
{
	Console.Error.WriteLine($"Input directory does not exist: {inputDir}");
	return;
}

var outputDir = args.Length >= 2 ? Path.GetFullPath(args[1]) : inputDir;

Directory.CreateDirectory(outputDir);

var jsonFiles = Directory.EnumerateFiles(inputDir, "*.json", SearchOption.TopDirectoryOnly).ToList();

if (jsonFiles.Count == 0)
{
	Console.WriteLine("No .json files found.");
	return;
}

Console.WriteLine($"Found {jsonFiles.Count} JSON files.");
Console.WriteLine($"Input : {inputDir}");
Console.WriteLine($"Output: {outputDir}");
Console.WriteLine();

var options = new JsonSerializerOptions
{
	PropertyNameCaseInsensitive = true,
	ReadCommentHandling = JsonCommentHandling.Skip,
	AllowTrailingCommas = true
};

int converted = 0, skipped = 0;

foreach (var file in jsonFiles)
{
	try
	{
		// Read file
		var json = await File.ReadAllTextAsync(file);
		using var doc = JsonDocument.Parse(json);

		var messages = ExtractMessages(doc.RootElement);

		if (messages.Count == 0)
		{
			Console.WriteLine($"[SKIP] No messages found in {Path.GetFileName(file)}");

			skipped++;
			continue;
		}

		// Normalize + sort
		var normalized = messages
			.Select(m => m with
			{
				Author = (m.Author ?? "").Trim(),
				Text = NormalizeLineEndings((m.Text ?? "").Trim()),
				Timestamp = NormalizeTimestamp(m.Timestamp).ToString()
			})
			.Where(m => !string.IsNullOrWhiteSpace(m.Text))
			.OrderBy(m => m.Timestamp ?? DateTimeOffset.MinValue.ToString())
			.ToList();

		// Create output
		var deduped = Deduplicate(normalized);
		var md = RenderMarkdown(deduped, sourceFile: Path.GetFileName(file));

		var outName = Path.GetFileNameWithoutExtension(file) + ".md";
		var outPath = Path.Combine(outputDir, outName);

		await File.WriteAllTextAsync(outPath, md, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

		Console.WriteLine($"[OK] {Path.GetFileName(file)} -> {outName} ({deduped.Count} msgs, {normalized.Count - deduped.Count} dupes removed)");
		converted++;
	}
	catch (Exception ex)
	{
		Console.WriteLine($"[ERR] {Path.GetFileName(file)}: {ex.Message}");
		skipped++;
	}
}

Console.WriteLine();
Console.WriteLine($"Done. Converted={converted}, Skipped/Errored={skipped}");
Console.WriteLine();

static List<ChatMessage> ExtractMessages(JsonElement root)
{
	if (TryGetProperty(root, "store", out var store) == false)
		return [];

	if (TryGetProperty(store, "rawConversationResponse", out var raw) == false)
		return [];

	if (TryGetProperty(raw, "messages", out var messages) == false || messages.ValueKind != JsonValueKind.Array)
		return [];

	var list = new List<ChatMessage>();

	foreach (var message in messages.EnumerateArray())
	{
		var author = TryGetString(message, "author");
		var timestamp = TryGetString(message, "timestamp");
		var text = TryGetString(message, "text");

		list.Add(new ChatMessage(author, timestamp, text));
	}

	return list;
}

static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
{
	if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out value))
		return true;

	value = default;
	return false;
}

static string? TryGetString(JsonElement element, string name)
{
	if (element.ValueKind != JsonValueKind.Object)
		return null;

	if (element.TryGetProperty(name, out var prop) == false)
		return null;

	return prop.ValueKind switch
	{
		JsonValueKind.String => prop.GetString(),
		JsonValueKind.Number => prop.GetRawText(), // fallback
		JsonValueKind.True => "true",
		JsonValueKind.False => "false",
		_ => prop.GetRawText()
	};
}

static string NormalizeLineEndings(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

static DateTimeOffset? NormalizeTimestamp(string? timestamp)
{
	if (string.IsNullOrWhiteSpace(timestamp))
		return null;

	// Accept ISO 8601 and a few common variants.
	if (DateTimeOffset.TryParse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var offset))
		return offset;

	// Sometimes comes as epoch milliseconds/seconds in string form.
	if (long.TryParse(timestamp, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks))
	{
		// Heuristic: > 10^12 likely ms, otherwise seconds.
		if (ticks > 1_000_000_000_000)
			return DateTimeOffset.FromUnixTimeMilliseconds(ticks);
		if (ticks > 1_000_000_000)
			return DateTimeOffset.FromUnixTimeSeconds(ticks);
	}

	return null;
}

static List<ChatMessage> Deduplicate(List<ChatMessage> messages)
{
	// Default strategy: hash(author + normalized timestamp + text)
	var seen = new HashSet<string>(StringComparer.Ordinal);
	var result = new List<ChatMessage>(messages.Count);

	foreach (var message in messages)
	{
		var key = MakeKey(message);

		if (seen.Add(key))
			result.Add(message);
	}

	return result;
}

static string MakeKey(ChatMessage message)
{
	var author = (message.Author ?? "").Trim().ToLowerInvariant();
	var timestamp = message.TimestampParsed?.ToUniversalTime().ToString("O") ?? (message.Timestamp ?? "").Trim();
	var text = (message.Text ?? "").Trim();

	// If timestamp exists, use it. If it doesn't, still dedupe identical text+author.
	var raw = $"{author}\n{timestamp}\n{text}";
	return Sha256(raw);
}

static string Sha256(string input)
{
	var bytes = Encoding.UTF8.GetBytes(input);
	var hash = SHA256.HashData(bytes);
	return Convert.ToHexString(hash);
}

static string RenderMarkdown(List<ChatMessage> messages, string sourceFile)
{
	var sb = new StringBuilder();

	sb.AppendLine("# Copilot Chat Export");
	sb.AppendLine();
	sb.AppendLine($"- Source: `{EscapeInlineCode(sourceFile)}`");
	sb.AppendLine($"- Exported: `{DateTimeOffset.UtcNow:O}`");
	sb.AppendLine();

	string? lastRole = null;

	foreach (var m in messages)
	{
		var role = MapRole(m.Author);
		var stamp = m.TimestampParsed?.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz") ?? m.Timestamp ?? "";

		// Group by role for readability: add header when role changes
		if (string.Equals(role, lastRole, StringComparison.Ordinal) == false)
		{
			sb.AppendLine($"## {role}");
			sb.AppendLine();
			lastRole = role;
		}

		if (string.IsNullOrWhiteSpace(stamp) == false)
		{
			sb.AppendLine($"*{stamp}*");
			sb.AppendLine();
		}

		// Message body
		sb.AppendLine(m.Text ?? "");
		sb.AppendLine();
		sb.AppendLine("---");
		sb.AppendLine();
	}

	return sb.ToString();
}
static string MapRole(string? author)
{
	var a = (author ?? "").Trim().ToLowerInvariant();

	// Adjust these mappings to match your exported JSON's author values.
	return a switch
	{
		"user" => "You",
		"you" => "You",
		"assistant" => "Copilot",
		"copilot" => "Copilot",
		"bot" => "Copilot",
		"system" => "System",
		"" => "Unknown",
		_ => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(a)
	};
}

static string EscapeInlineCode(string input) => input.Replace("`", "\\`");
