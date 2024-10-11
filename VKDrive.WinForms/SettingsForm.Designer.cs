namespace VKDrive.WinForms;

partial class SettingsForm
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
        groupBox1 = new GroupBox();
        labelApiKey = new Label();
        textBoxApiKey = new TextBox();
        textBoxGroupId = new TextBox();
        labelGroupId = new Label();
        groupBoxSecurity = new GroupBox();
        textBoxArchivePasswordLength = new TextBox();
        labelArchivePasswordLength = new Label();
        buttonSaveSettings = new Button();
        buttonCancelSettings = new Button();
        groupBoxSystem = new GroupBox();
        groupBoxAppPriority = new GroupBox();
        radioButtonLowPriority = new RadioButton();
        radioButtonNormalPriority = new RadioButton();
        groupBoxInterface = new GroupBox();
        checkBoxSoundsOn = new CheckBox();
        checkBoxAskDeletion = new CheckBox();
        checkBoxOpenFolderAfterDownload = new CheckBox();
        groupBoxFileProcessing = new GroupBox();
        comboBoxCompressionLevel = new ComboBox();
        label2 = new Label();
        textBoxChunkSize = new TextBox();
        label1 = new Label();
        groupBox1.SuspendLayout();
        groupBoxSecurity.SuspendLayout();
        groupBoxSystem.SuspendLayout();
        groupBoxAppPriority.SuspendLayout();
        groupBoxInterface.SuspendLayout();
        groupBoxFileProcessing.SuspendLayout();
        SuspendLayout();
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(labelApiKey);
        groupBox1.Controls.Add(textBoxApiKey);
        groupBox1.Controls.Add(textBoxGroupId);
        groupBox1.Controls.Add(labelGroupId);
        groupBox1.Location = new Point(12, 12);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(410, 81);
        groupBox1.TabIndex = 0;
        groupBox1.TabStop = false;
        groupBox1.Text = "VK API";
        // 
        // labelApiKey
        // 
        labelApiKey.AutoSize = true;
        labelApiKey.Location = new Point(167, 19);
        labelApiKey.Name = "labelApiKey";
        labelApiKey.Size = new Size(46, 15);
        labelApiKey.TabIndex = 3;
        labelApiKey.Text = "API key";
        // 
        // textBoxApiKey
        // 
        textBoxApiKey.Location = new Point(167, 37);
        textBoxApiKey.Name = "textBoxApiKey";
        textBoxApiKey.Size = new Size(237, 23);
        textBoxApiKey.TabIndex = 2;
        textBoxApiKey.Click += textBoxApiKey_Click;
        textBoxApiKey.TextChanged += textBoxApiKey_TextChanged;
        // 
        // textBoxGroupId
        // 
        textBoxGroupId.Location = new Point(6, 37);
        textBoxGroupId.Name = "textBoxGroupId";
        textBoxGroupId.Size = new Size(134, 23);
        textBoxGroupId.TabIndex = 1;
        textBoxGroupId.TextChanged += textBoxGroupId_TextChanged;
        textBoxGroupId.KeyPress += textBoxGroupId_KeyPress;
        // 
        // labelGroupId
        // 
        labelGroupId.AutoSize = true;
        labelGroupId.Location = new Point(6, 19);
        labelGroupId.Name = "labelGroupId";
        labelGroupId.Size = new Size(54, 15);
        labelGroupId.TabIndex = 0;
        labelGroupId.Text = "Group ID";
        // 
        // groupBoxSecurity
        // 
        groupBoxSecurity.Controls.Add(textBoxArchivePasswordLength);
        groupBoxSecurity.Controls.Add(labelArchivePasswordLength);
        groupBoxSecurity.Location = new Point(12, 99);
        groupBoxSecurity.Name = "groupBoxSecurity";
        groupBoxSecurity.Size = new Size(159, 77);
        groupBoxSecurity.TabIndex = 1;
        groupBoxSecurity.TabStop = false;
        groupBoxSecurity.Text = "Security";
        // 
        // textBoxArchivePasswordLength
        // 
        textBoxArchivePasswordLength.Location = new Point(6, 37);
        textBoxArchivePasswordLength.Name = "textBoxArchivePasswordLength";
        textBoxArchivePasswordLength.Size = new Size(134, 23);
        textBoxArchivePasswordLength.TabIndex = 1;
        textBoxArchivePasswordLength.TextChanged += textBoxArchivePasswordLength_TextChanged;
        textBoxArchivePasswordLength.KeyPress += textBoxArchivePasswordLength_KeyPress;
        // 
        // labelArchivePasswordLength
        // 
        labelArchivePasswordLength.AutoSize = true;
        labelArchivePasswordLength.Location = new Point(6, 19);
        labelArchivePasswordLength.Name = "labelArchivePasswordLength";
        labelArchivePasswordLength.Size = new Size(137, 15);
        labelArchivePasswordLength.TabIndex = 0;
        labelArchivePasswordLength.Text = "Archive password length";
        // 
        // buttonSaveSettings
        // 
        buttonSaveSettings.Location = new Point(266, 388);
        buttonSaveSettings.Name = "buttonSaveSettings";
        buttonSaveSettings.Size = new Size(75, 23);
        buttonSaveSettings.TabIndex = 2;
        buttonSaveSettings.Text = "Save";
        buttonSaveSettings.UseVisualStyleBackColor = true;
        buttonSaveSettings.Click += buttonSaveSettings_Click;
        // 
        // buttonCancelSettings
        // 
        buttonCancelSettings.Location = new Point(347, 388);
        buttonCancelSettings.Name = "buttonCancelSettings";
        buttonCancelSettings.Size = new Size(75, 23);
        buttonCancelSettings.TabIndex = 3;
        buttonCancelSettings.Text = "Cancel";
        buttonCancelSettings.UseVisualStyleBackColor = true;
        buttonCancelSettings.Click += buttonCancelSettings_Click;
        // 
        // groupBoxSystem
        // 
        groupBoxSystem.Controls.Add(groupBoxAppPriority);
        groupBoxSystem.Location = new Point(197, 99);
        groupBoxSystem.Name = "groupBoxSystem";
        groupBoxSystem.Size = new Size(225, 77);
        groupBoxSystem.TabIndex = 4;
        groupBoxSystem.TabStop = false;
        groupBoxSystem.Text = "System";
        // 
        // groupBoxAppPriority
        // 
        groupBoxAppPriority.Controls.Add(radioButtonLowPriority);
        groupBoxAppPriority.Controls.Add(radioButtonNormalPriority);
        groupBoxAppPriority.Location = new Point(6, 19);
        groupBoxAppPriority.Name = "groupBoxAppPriority";
        groupBoxAppPriority.Size = new Size(207, 47);
        groupBoxAppPriority.TabIndex = 0;
        groupBoxAppPriority.TabStop = false;
        groupBoxAppPriority.Text = "App priority";
        // 
        // radioButtonLowPriority
        // 
        radioButtonLowPriority.AutoSize = true;
        radioButtonLowPriority.Location = new Point(90, 22);
        radioButtonLowPriority.Name = "radioButtonLowPriority";
        radioButtonLowPriority.Size = new Size(47, 19);
        radioButtonLowPriority.TabIndex = 1;
        radioButtonLowPriority.TabStop = true;
        radioButtonLowPriority.Text = "Low";
        radioButtonLowPriority.UseVisualStyleBackColor = true;
        radioButtonLowPriority.CheckedChanged += radioButtonLowPriority_CheckedChanged;
        // 
        // radioButtonNormalPriority
        // 
        radioButtonNormalPriority.AutoSize = true;
        radioButtonNormalPriority.Location = new Point(15, 22);
        radioButtonNormalPriority.Name = "radioButtonNormalPriority";
        radioButtonNormalPriority.Size = new Size(65, 19);
        radioButtonNormalPriority.TabIndex = 0;
        radioButtonNormalPriority.TabStop = true;
        radioButtonNormalPriority.Text = "Normal";
        radioButtonNormalPriority.UseVisualStyleBackColor = true;
        radioButtonNormalPriority.CheckedChanged += radioButtonNormalPriority_CheckedChanged;
        // 
        // groupBoxInterface
        // 
        groupBoxInterface.Controls.Add(checkBoxSoundsOn);
        groupBoxInterface.Controls.Add(checkBoxAskDeletion);
        groupBoxInterface.Controls.Add(checkBoxOpenFolderAfterDownload);
        groupBoxInterface.Location = new Point(12, 182);
        groupBoxInterface.Name = "groupBoxInterface";
        groupBoxInterface.Size = new Size(410, 79);
        groupBoxInterface.TabIndex = 5;
        groupBoxInterface.TabStop = false;
        groupBoxInterface.Text = "Interface";
        // 
        // checkBoxSoundsOn
        // 
        checkBoxSoundsOn.AutoSize = true;
        checkBoxSoundsOn.Location = new Point(229, 22);
        checkBoxSoundsOn.Name = "checkBoxSoundsOn";
        checkBoxSoundsOn.Size = new Size(65, 19);
        checkBoxSoundsOn.TabIndex = 2;
        checkBoxSoundsOn.Text = "Sounds";
        checkBoxSoundsOn.UseVisualStyleBackColor = true;
        checkBoxSoundsOn.CheckedChanged += checkBoxSoundsOn_CheckedChanged;
        // 
        // checkBoxAskDeletion
        // 
        checkBoxAskDeletion.AutoSize = true;
        checkBoxAskDeletion.Location = new Point(15, 47);
        checkBoxAskDeletion.Name = "checkBoxAskDeletion";
        checkBoxAskDeletion.Size = new Size(128, 19);
        checkBoxAskDeletion.TabIndex = 1;
        checkBoxAskDeletion.Text = "Ask before deletion";
        checkBoxAskDeletion.UseVisualStyleBackColor = true;
        checkBoxAskDeletion.CheckedChanged += checkBoxAskDeletion_CheckedChanged;
        // 
        // checkBoxOpenFolderAfterDownload
        // 
        checkBoxOpenFolderAfterDownload.AutoSize = true;
        checkBoxOpenFolderAfterDownload.Location = new Point(15, 22);
        checkBoxOpenFolderAfterDownload.Name = "checkBoxOpenFolderAfterDownload";
        checkBoxOpenFolderAfterDownload.Size = new Size(189, 19);
        checkBoxOpenFolderAfterDownload.TabIndex = 0;
        checkBoxOpenFolderAfterDownload.Text = "Open folder after downloading";
        checkBoxOpenFolderAfterDownload.UseVisualStyleBackColor = true;
        checkBoxOpenFolderAfterDownload.CheckedChanged += checkBoxOpenFolderAfterDownload_CheckedChanged;
        // 
        // groupBoxFileProcessing
        // 
        groupBoxFileProcessing.Controls.Add(comboBoxCompressionLevel);
        groupBoxFileProcessing.Controls.Add(label2);
        groupBoxFileProcessing.Controls.Add(textBoxChunkSize);
        groupBoxFileProcessing.Controls.Add(label1);
        groupBoxFileProcessing.Location = new Point(12, 267);
        groupBoxFileProcessing.Name = "groupBoxFileProcessing";
        groupBoxFileProcessing.Size = new Size(410, 100);
        groupBoxFileProcessing.TabIndex = 6;
        groupBoxFileProcessing.TabStop = false;
        groupBoxFileProcessing.Text = "File processing";
        // 
        // comboBoxCompressionLevel
        // 
        comboBoxCompressionLevel.FormattingEnabled = true;
        comboBoxCompressionLevel.Location = new Point(191, 52);
        comboBoxCompressionLevel.Name = "comboBoxCompressionLevel";
        comboBoxCompressionLevel.Size = new Size(121, 23);
        comboBoxCompressionLevel.TabIndex = 3;
        comboBoxCompressionLevel.SelectedIndexChanged += comboBoxCompressionLevel_SelectedIndexChanged;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(191, 34);
        label2.Name = "label2";
        label2.Size = new Size(104, 15);
        label2.TabIndex = 2;
        label2.Text = "Compression level";
        // 
        // textBoxChunkSize
        // 
        textBoxChunkSize.Location = new Point(6, 52);
        textBoxChunkSize.Name = "textBoxChunkSize";
        textBoxChunkSize.Size = new Size(111, 23);
        textBoxChunkSize.TabIndex = 1;
        textBoxChunkSize.TextChanged += textBoxChunkSize_TextChanged;
        textBoxChunkSize.KeyPress += textBoxChunkSize_KeyPress;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(6, 19);
        label1.Name = "label1";
        label1.Size = new Size(111, 30);
        label1.TabIndex = 0;
        label1.Text = "File part size, in MB \r\n(1-200)";
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(434, 423);
        Controls.Add(groupBoxFileProcessing);
        Controls.Add(groupBoxInterface);
        Controls.Add(groupBoxSystem);
        Controls.Add(buttonCancelSettings);
        Controls.Add(buttonSaveSettings);
        Controls.Add(groupBoxSecurity);
        Controls.Add(groupBox1);
        Name = "SettingsForm";
        Text = "Settings";
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBoxSecurity.ResumeLayout(false);
        groupBoxSecurity.PerformLayout();
        groupBoxSystem.ResumeLayout(false);
        groupBoxAppPriority.ResumeLayout(false);
        groupBoxAppPriority.PerformLayout();
        groupBoxInterface.ResumeLayout(false);
        groupBoxInterface.PerformLayout();
        groupBoxFileProcessing.ResumeLayout(false);
        groupBoxFileProcessing.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox groupBox1;
    private Label labelGroupId;
    private TextBox textBoxGroupId;
    private TextBox textBoxApiKey;
    private Label labelApiKey;
    private GroupBox groupBoxSecurity;
    private Label labelArchivePasswordLength;
    private TextBox textBoxArchivePasswordLength;
    private Button buttonSaveSettings;
    private Button buttonCancelSettings;
    private GroupBox groupBoxSystem;
    private GroupBox groupBoxAppPriority;
    private RadioButton radioButtonLowPriority;
    private RadioButton radioButtonNormalPriority;
    private GroupBox groupBoxInterface;
    private CheckBox checkBoxAskDeletion;
    private CheckBox checkBoxOpenFolderAfterDownload;
    private CheckBox checkBoxSoundsOn;
    private GroupBox groupBoxFileProcessing;
    private Label label1;
    private TextBox textBoxChunkSize;
    private ComboBox comboBoxCompressionLevel;
    private Label label2;
}