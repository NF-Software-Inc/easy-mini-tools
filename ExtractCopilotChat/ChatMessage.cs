namespace ExtractCopilotChat;

internal record ChatMessage(string? Author, string? Timestamp, string? Text)
{
	// Parsed timestamp for ordering
	public DateTimeOffset? TimestampParsed { get; init; }
};
