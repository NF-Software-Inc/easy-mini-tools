namespace OFX_To_XLSX;

public class OfxAccount
{
	/// <summary>
	/// Routing and transit number.
	/// </summary>
	public string? BankId { get; set; }

	/// <summary>
	/// Account number.
	/// </summary>
	public string? AccountId { get; set; }

	/// <summary>
	/// Type of account.
	/// </summary>
	public string? AccountType { get; set; }
}
