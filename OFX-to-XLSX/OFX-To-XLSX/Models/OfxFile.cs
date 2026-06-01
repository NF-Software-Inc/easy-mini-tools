namespace OFX_To_XLSX;

public class OfxFile
{
	/// <summary>
	/// Contains the header data from the OFX file.
	/// </summary>
	public OfxHeader Header { get; init; } = new();

	/// <summary>
	/// The Signon message set includes the signon message, USERPASS change message, and challenge message, which must appear in that order.
	/// </summary>
	public OfxSignOnResponse SignOnResponse { get; init; } = new();

	/// <summary>
	/// Contains details on the account the OFX file is for.
	/// </summary>
	public OfxAccount Account { get; init; } = new();

	/// <summary>
	/// Default currency for the statement.
	/// </summary>
	public string? Currency { get; set; }

	/// <summary>
	/// Start date of statement requested.
	/// </summary>
	public DateTime? Start { get; set; }

	/// <summary>
	/// End date of statement requested.
	/// </summary>
	public DateTime? End { get; set; }

	/// <summary>
	/// Ledger balance amount.
	/// </summary>
	public decimal? Balance { get; set; }

	/// <summary>
	/// Balance date.
	/// </summary>
	public DateTime? BalanceDate { get; set; }

	/// <summary>
	/// Contains the transactions from the OFX file.
	/// </summary>
	public List<OfxTransaction> Transactions { get; init; } = [];
}
