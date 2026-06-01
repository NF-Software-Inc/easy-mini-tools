using OFX_To_XLSX;

namespace OFX_To_XLSX_Converter;

public partial class ConverterForm : Form
{
	public ConverterForm()
	{
		InitializeComponent();
	}

	private void OnOpenFileClicked(object sender, EventArgs args)
	{
		if (openFileDialog.ShowDialog() == DialogResult.OK)
			textBoxOfxFile.Text = openFileDialog.FileName;
	}

	private void OnSaveFileClicked(object sender, EventArgs args)
	{
		if (saveFileDialog.ShowDialog() == DialogResult.OK )
			textBoxXlsxFile.Text = saveFileDialog.FileName;
	}

	private void OnConvertFileClicked(object sender, EventArgs args)
	{
		richTextBoxResults.ResetText();

		if (string.IsNullOrWhiteSpace(textBoxOfxFile.Text) || string.IsNullOrWhiteSpace(textBoxXlsxFile.Text))
		{
			richTextBoxResults.AppendText("Must provide both OFX and XLSX files.");
			return;
		}
		else if (File.Exists(textBoxOfxFile.Text) == false || Directory.Exists(Path.GetDirectoryName(textBoxXlsxFile.Text)) == false)
		{
			richTextBoxResults.AppendText("OFX file and directory to save XLSX file must both exist.");
			return;
		}

		try
		{
			var converter = new OfxParser();
			var ofx = converter.ReadOfxFile(textBoxOfxFile.Text);

			if (ofx == null)
				richTextBoxResults.AppendText("Failed reading OFX file.");
			else if (converter.ConvertOfxToXlsx(textBoxXlsxFile.Text, ofx))
				richTextBoxResults.AppendText("Converted OFX to XLSX successfully.");
			else
				richTextBoxResults.AppendText("Failed converting OFX to XLSX.");
		}
		catch (Exception e)
		{
			richTextBoxResults.AppendText("Failed converting OFX to XLSX.");
			richTextBoxResults.AppendText($"Error details {e.GetType().Name}, {e.Message}.");
		}
	}
}
