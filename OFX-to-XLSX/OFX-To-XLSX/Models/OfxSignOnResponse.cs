namespace OFX_To_XLSX;

public class OfxSignOnResponse
{
	/// <summary>
	/// Organization defining this Financial Institution name space.
	/// </summary>
	public string? Organization { get; set; }

	/// <summary>
	/// Financial Institution ID.
	/// </summary>
	public string? OrganizationId { get; set; }

	/// <summary>
	/// Contains any error code that occurs during sign-on.
	/// </summary>
	public int Status { get; set; }

	/// <summary>
	/// Contains any severity level of error codes that occur during sign-on.
	/// </summary>
	public string? StatusSeverity { get; set; }

	/// <summary>
	/// Language used in text responses.
	/// </summary>
	public string? Language { get; set; }

	/// <summary>
	/// Date and time of the server response.
	/// </summary>
	public DateTime? ServerTime { get; set; }
}
