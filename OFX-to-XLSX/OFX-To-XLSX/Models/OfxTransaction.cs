namespace OFX_To_XLSX;

public class OfxTransaction
{
	/// <summary>
	/// Transaction type.
	/// </summary>
	public string? TransactionType { get; set; }

	/// <summary>
	/// Date transaction was posted to account.
	/// </summary>
	public DateTime? Posted { get; set; }

	/// <summary>
	/// Amount of transaction.
	/// </summary>
	public decimal Amount { get; set; }

	/// <summary>
	/// Transaction ID issued by financial institution.
	/// </summary>
	public string? TransactionId { get; set; }

	/// <summary>
	/// Check (or other reference) number.
	/// </summary>
	public string? CheckNumber { get; set; }

	/// <summary>
	/// Name of payee or description of transaction.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Extra information (not in NAME).
	/// </summary>
	public string? Memo { get; set; }
}
