using easy_core;
using SpreadsheetLight;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace OFX_To_XLSX;

public class OfxParser
{
	/// <summary>
	/// Reads the specified file and parses the OFX data.
	/// </summary>
	/// <param name="file">The path to the file to parse.</param>
	public OfxFile? ReadOfxFile(string file)
	{
		if (File.Exists(file) == false)
			return null;

		var text = File.ReadAllText(file);
		var lines = text.ToLines().Where(x => string.IsNullOrWhiteSpace(x) == false).Select(x => x.Trim()).ToArray();
		var ofx = new OfxFile();

		while (lines.Length > 0 && lines[0].FirstOrDefault() != '<')
		{
			if (lines[0].StartsWith("OFXHEADER"))
				ofx.Header.HeaderVersion = int.TryParse(lines[0].Split(':').Last(), out var version) ? version : 0;
			else if (lines[0].StartsWith("DATA"))
				ofx.Header.ContentType = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("VERSION"))
				ofx.Header.Version = int.TryParse(lines[0].Split(':').Last(), out var version) ? version : 0;
			else if (lines[0].StartsWith("SECURITY"))
				ofx.Header.Security = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("ENCODING"))
				ofx.Header.Encoding = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("CHARSET"))
				ofx.Header.CharacterSet = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("COMPRESSION"))
				ofx.Header.Compression = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("OLDFILEUID"))
				ofx.Header.OldFileId = lines[0].Split(':').Skip(1).FirstOrDefault();
			else if (lines[0].StartsWith("NEWFILEUID"))
				ofx.Header.NewFileId = lines[0].Split(':').Skip(1).FirstOrDefault();

			lines = lines.Skip(1).ToArray();
		}

		var xml = ConvertOfxToXml(lines);

		if (string.IsNullOrWhiteSpace(xml))
			return null;

		ParseXmlData(xml, ofx);
		return ofx;
	}

	private string ConvertOfxToXml(string[] lines)
	{
		using var writer = new StringWriter();
		using var xml = XmlWriter.Create(writer);

		xml.WriteStartDocument();

		foreach (var line in lines)
		{
			if (line.StartsWith("</") && line.EndsWith('>'))
				xml.WriteEndElement();
			else if (line.StartsWith('<') && line.EndsWith('>'))
				xml.WriteStartElement(line.TrimStart('<').TrimEnd('>'));
			else if (line.Contains('>') && line.Length > line.IndexOf('>') + 1)
				xml.WriteElementString(line.Split('>').First().TrimStart('<'), line.Split('>').Last());
		}

		xml.WriteEndDocument();
		xml.Flush();

		return writer.ToString();
	}

	private void ParseXmlData(string xml, OfxFile ofx)
	{
		var document = XDocument.Parse(xml);

		var signon = document.Descendants("SONRS").SingleOrDefault();
		var account = document.Descendants("BANKACCTFROM").FirstOrDefault();
		var ledger = document.Descendants("LEDGERBAL").FirstOrDefault();
		var transactions = document.Descendants("STMTTRN");

		if (signon != null && signon.HasElements)
		{
			var institution = signon.Element("FI");

			if (institution != null)
			{
				ofx.SignOnResponse.Organization = institution.Element("ORG")?.Value;
				ofx.SignOnResponse.OrganizationId = institution.Element("FID")?.Value;
			}

			var status = signon.Element("STATUS");

			if (status != null)
			{
				ofx.SignOnResponse.Status = int.TryParse(status.Element("CODE")?.Value, out int code) ? code : 0;
				ofx.SignOnResponse.StatusSeverity = status.Element("SEVERITY")?.Value;
			}

			ofx.SignOnResponse.Language = signon.Element("LANGUAGE")?.Value;
			ofx.SignOnResponse.ServerTime = ParseXmlDate(signon.Element("DTSERVER"));
		}

		if (account != null && account.HasElements)
		{
			ofx.Account.BankId = account.Element("BANKID")?.Value;
			ofx.Account.AccountId = account.Element("ACCTID")?.Value;
			ofx.Account.AccountType = account.Element("ACCTTYPE")?.Value;
		}

		ofx.Currency = document.Descendants("CURDEF").FirstOrDefault()?.Value;
		ofx.Start = ParseXmlDate(document.Descendants("DTSTART").FirstOrDefault());
		ofx.End = ParseXmlDate(document.Descendants("DTEND").FirstOrDefault());

		if (ledger != null && ledger.HasElements)
		{
			ofx.Balance = decimal.TryParse(ledger.Element("BALAMT")?.Value, out decimal balance) ? balance : null;
			ofx.BalanceDate = ParseXmlDate(ledger.Element("DTASOF"));
		}

		foreach (var transaction in transactions.Where(x => x.HasElements))
		{
			ofx.Transactions.Add(new OfxTransaction
			{
				TransactionType = transaction.Element("TRNTYPE")?.Value,
				Amount = decimal.TryParse(transaction.Element("TRNAMT")?.Value, out decimal amount) ? amount : 0,
				Posted = ParseXmlDate(transaction.Element("DTPOSTED")),
				TransactionId = transaction.Element("FITID")?.Value,
				CheckNumber = transaction.Element("CHECKNUM")?.Value,
				Name = transaction.Element("NAME")?.Value,
				Memo = transaction.Element("MEMO")?.Value,
			});
		}
	}

	private DateTime? ParseXmlDate(XElement? element) => DateTime.TryParseExact(element?.Value[..14], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime time) ? time : null;

	/// <summary>
	/// Converts the provided OFX file into an XLSX file and saves it at the specified path.
	/// </summary>
	/// <param name="file">The path and filename to save the XLSX file to.</param>
	/// <param name="ofx">Object containing the parsed OFX data.</param>
	/// <param name="includeHeader"></param>
	/// <param name="includeSignOn"></param>
	/// <param name="includeDetails"></param>
	/// <param name="useTableHeader"></param>
	public bool ConvertOfxToXlsx(string file, OfxFile ofx, bool includeHeader = false, bool includeSignOn = false, bool includeDetails = true, bool useTableHeader = true)
	{
		// Prepare styles
		var excel = new SLDocument();

		var title = excel.CreateStyle();
		var accent1 = excel.CreateStyle();
		var accent2 = excel.CreateStyle();
		var dates = excel.CreateStyle();
		var numbers = excel.CreateStyle();

		title.ApplyNamedCellStyle(SLNamedCellStyleValues.Title);
		accent1.ApplyNamedCellStyle(SLNamedCellStyleValues.Accent1);
		accent2.ApplyNamedCellStyle(SLNamedCellStyleValues.Accent2);
		dates.FormatCode = "yyyy-mm-dd hh:mm:ss";
		numbers.FormatCode = "0.00";

		// Prepare data
		if (useTableHeader)
		{
			excel.SetCellValue("A1", "Transaction Id");
			excel.SetCellValue("B1", "Transaction Type");
			excel.SetCellValue("C1", "Posted");
			excel.SetCellValue("D1", "Amount");
			excel.SetCellValue("E1", "Detail");
			excel.SetCellValue("F1", "Reference");
			excel.SetCellValue("G1", "Memo");

			excel.SetCellStyle("A1", "G1", accent1);
		}

		var offset = useTableHeader ? 2 : 1;

		for (var i = 0; i < ofx.Transactions.Count; i++)
		{
			excel.SetCellValue($"A{i + offset}", ofx.Transactions[i].TransactionId);
			excel.SetCellValue($"B{i + offset}", ofx.Transactions[i].TransactionType);

			if (ofx.Transactions[i].Posted != null)
				excel.SetCellValue($"C{i + offset}", ofx.Transactions[i].Posted!.Value);

			excel.SetCellValue($"D{i + offset}", ofx.Transactions[i].Amount);
			excel.SetCellValue($"E{i + offset}", ofx.Transactions[i].Name);
			excel.SetCellValue($"F{i + offset}", ofx.Transactions[i].CheckNumber);
			excel.SetCellValue($"G{i + offset}", ofx.Transactions[i].Memo);
		}

		excel.SetCellStyle("C" + offset, "C" + (offset + ofx.Transactions.Count - 1), dates);
		excel.SetCellStyle("D" + offset, "D" + (offset + ofx.Transactions.Count - 1), numbers);

		// Autofit
		excel.AutoFitColumn("A", "G");

		// Save
		try
		{
			excel.SaveAs(file);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
