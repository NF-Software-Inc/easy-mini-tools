namespace OFX_To_XLSX;

public class OfxHeader
{
	/// <summary>
	/// OFXHEADER specifies the version number of the Open Financial Exchange headers.
	/// </summary>
	public int HeaderVersion { get; set; }

	/// <summary>
	/// DATA specifies the content type, in this case OFXSGML.
	/// </summary>
	public string? ContentType { get; set; }

	/// <summary>
	/// VERSION specifies the version number of the Document Type Definition (DTD) used for parsing.
	/// </summary>
	public int Version { get; set; }

	/// <summary>
	/// SECURITY defines the type of application-level security, if any, that is used for the OFX block. The values for SECURITY can be NONE or TYPE1.
	/// </summary>
	public string? Security { get; set; }

	/// <summary>
	/// ENCODING defines the text encoding used for character data. The values for ENCODING can be UNICODE or USASCII.
	/// </summary>
	public string? Encoding { get; set; }

	/// <summary>
	/// CHARSET defines the character set used for character data.
	/// </summary>
	public string? CharacterSet { get; set; }

	/// <summary>
	/// A future version of the specification will define compression.
	/// </summary>
	public string? Compression { get; set; }

	/// <summary>
	/// NEWFILEUID uniquely identifies this request file.
	/// </summary>
	public string? NewFileId { get; set; }

	/// <summary>
	/// OLDFILEUID is used together with NEWFILEUID only when the client and server support file-based error recovery.
	/// </summary>
	public string? OldFileId { get; set; }
}
