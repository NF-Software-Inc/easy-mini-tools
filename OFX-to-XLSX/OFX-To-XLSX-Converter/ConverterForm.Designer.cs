namespace OFX_To_XLSX_Converter
{
	partial class ConverterForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			labelOpenFile = new Label();
			openFileDialog = new OpenFileDialog();
			buttonOpenFile = new Button();
			textBoxOfxFile = new TextBox();
			textBoxXlsxFile = new TextBox();
			buttonSaveFile = new Button();
			saveFileDialog = new SaveFileDialog();
			buttonConvert = new Button();
			labelSaveFile = new Label();
			richTextBoxResults = new RichTextBox();
			SuspendLayout();
			// 
			// labelOpenFile
			// 
			labelOpenFile.AutoSize = true;
			labelOpenFile.Location = new Point(12, 9);
			labelOpenFile.Name = "labelOpenFile";
			labelOpenFile.Size = new Size(142, 25);
			labelOpenFile.TabIndex = 0;
			labelOpenFile.Text = "Choose OFX File";
			// 
			// openFileDialog
			// 
			openFileDialog.FileName = "openFileDialog";
			openFileDialog.Filter = "OFX Files|*.ofx|All Files|*.*";
			// 
			// buttonOpenFile
			// 
			buttonOpenFile.Location = new Point(613, 34);
			buttonOpenFile.Name = "buttonOpenFile";
			buttonOpenFile.Size = new Size(175, 34);
			buttonOpenFile.TabIndex = 1;
			buttonOpenFile.Text = "Browse...";
			buttonOpenFile.UseVisualStyleBackColor = true;
			buttonOpenFile.Click += OnOpenFileClicked;
			// 
			// textBoxOfxFile
			// 
			textBoxOfxFile.Location = new Point(12, 37);
			textBoxOfxFile.Name = "textBoxOfxFile";
			textBoxOfxFile.Size = new Size(595, 31);
			textBoxOfxFile.TabIndex = 2;
			// 
			// textBoxXlsxFile
			// 
			textBoxXlsxFile.Location = new Point(12, 121);
			textBoxXlsxFile.Name = "textBoxXlsxFile";
			textBoxXlsxFile.Size = new Size(595, 31);
			textBoxXlsxFile.TabIndex = 3;
			// 
			// buttonSaveFile
			// 
			buttonSaveFile.Location = new Point(613, 118);
			buttonSaveFile.Name = "buttonSaveFile";
			buttonSaveFile.Size = new Size(175, 34);
			buttonSaveFile.TabIndex = 4;
			buttonSaveFile.Text = "Set Name...";
			buttonSaveFile.UseVisualStyleBackColor = true;
			buttonSaveFile.Click += OnSaveFileClicked;
			// 
			// saveFileDialog
			// 
			saveFileDialog.DefaultExt = "xlsx";
			saveFileDialog.Filter = "Excel Files|*.xlsx|All Files|*.*";
			// 
			// buttonConvert
			// 
			buttonConvert.Location = new Point(12, 220);
			buttonConvert.Name = "buttonConvert";
			buttonConvert.Size = new Size(776, 84);
			buttonConvert.TabIndex = 5;
			buttonConvert.Text = "Convert!";
			buttonConvert.UseVisualStyleBackColor = true;
			buttonConvert.Click += OnConvertFileClicked;
			// 
			// labelSaveFile
			// 
			labelSaveFile.AutoSize = true;
			labelSaveFile.Location = new Point(12, 93);
			labelSaveFile.Name = "labelSaveFile";
			labelSaveFile.Size = new Size(148, 25);
			labelSaveFile.TabIndex = 6;
			labelSaveFile.Text = "Save To XLSX File";
			// 
			// richTextBoxResults
			// 
			richTextBoxResults.Location = new Point(12, 310);
			richTextBoxResults.Name = "richTextBoxResults";
			richTextBoxResults.Size = new Size(776, 163);
			richTextBoxResults.TabIndex = 7;
			richTextBoxResults.Text = "";
			// 
			// ConverterForm
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 485);
			Controls.Add(richTextBoxResults);
			Controls.Add(labelSaveFile);
			Controls.Add(buttonConvert);
			Controls.Add(buttonSaveFile);
			Controls.Add(textBoxXlsxFile);
			Controls.Add(textBoxOfxFile);
			Controls.Add(buttonOpenFile);
			Controls.Add(labelOpenFile);
			Name = "ConverterForm";
			Text = "Convert OFX To XLSX";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Label labelOpenFile;
		private OpenFileDialog openFileDialog;
		private Button buttonOpenFile;
		private TextBox textBoxOfxFile;
		private TextBox textBoxXlsxFile;
		private Button buttonSaveFile;
		private SaveFileDialog saveFileDialog;
		private Button buttonConvert;
		private Label labelSaveFile;
		private RichTextBox richTextBoxResults;
	}
}