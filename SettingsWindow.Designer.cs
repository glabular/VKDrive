namespace WinFormsApp1
{
    partial class SettingsWindow
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
            textBox1 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            textBoxFilesizeUpload = new TextBox();
            groupBox1 = new GroupBox();
            textBoxToken = new TextBox();
            label4 = new Label();
            textBox3 = new TextBox();
            label3 = new Label();
            groupBox2 = new GroupBox();
            txtBoxAESpassLength = new TextBox();
            txtBoxArchivePassLength = new TextBox();
            label6 = new Label();
            label5 = new Label();
            groupBox3 = new GroupBox();
            groupBox5 = new GroupBox();
            radioBtnPriorityLow = new RadioButton();
            radioBtnPriorityNormal = new RadioButton();
            groupBox4 = new GroupBox();
            textBoxHTTPclientTimeout = new TextBox();
            label7 = new Label();
            applyButton = new Button();
            cancelButton = new Button();
            interfaceGroupBox = new GroupBox();
            groupBox7 = new GroupBox();
            radioBtnSortByDate = new RadioButton();
            radioBtnSortByName = new RadioButton();
            checkBoxOpenFolderAfterLoad = new CheckBox();
            checkBoxAskBeforeDelete = new CheckBox();
            checkBox1 = new CheckBox();
            groupBox6 = new GroupBox();
            labelCompressionLevel = new Label();
            comboBoxCompressionLvl = new ComboBox();
            checkBoxEnableConsole = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            interfaceGroupBox.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new Point(198, 37);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(121, 23);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 72);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "Токен";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 19);
            label2.Name = "label2";
            label2.Size = new Size(122, 30);
            label2.TabIndex = 2;
            label2.Text = "Размер части файла,\r\nв МБ (1-200)";
            // 
            // textBoxFilesizeUpload
            // 
            textBoxFilesizeUpload.Location = new Point(12, 52);
            textBoxFilesizeUpload.Name = "textBoxFilesizeUpload";
            textBoxFilesizeUpload.Size = new Size(119, 23);
            textBoxFilesizeUpload.TabIndex = 3;
            textBoxFilesizeUpload.TextChanged += textBoxFilesizeUpload_TextChanged;
            textBoxFilesizeUpload.KeyPress += textBoxFilesizeUpload_KeyPress;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxToken);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(466, 121);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "VK API";
            // 
            // textBoxToken
            // 
            textBoxToken.Location = new Point(13, 90);
            textBoxToken.Name = "textBoxToken";
            textBoxToken.Size = new Size(436, 23);
            textBoxToken.TabIndex = 5;
            textBoxToken.Click += textBoxToken_Click;
            textBoxToken.TextChanged += textBoxToken_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(198, 19);
            label4.Name = "label4";
            label4.Size = new Size(96, 15);
            label4.TabIndex = 4;
            label4.Text = "ID пользователя";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(13, 37);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(149, 23);
            textBox3.TabIndex = 3;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 19);
            label3.Name = "label3";
            label3.Size = new Size(62, 15);
            label3.TabIndex = 2;
            label3.Text = "ID группы";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtBoxAESpassLength);
            groupBox2.Controls.Add(txtBoxArchivePassLength);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Location = new Point(12, 139);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(466, 77);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Безопасность";
            // 
            // txtBoxAESpassLength
            // 
            txtBoxAESpassLength.Location = new Point(159, 47);
            txtBoxAESpassLength.Name = "txtBoxAESpassLength";
            txtBoxAESpassLength.Size = new Size(149, 23);
            txtBoxAESpassLength.TabIndex = 3;
            txtBoxAESpassLength.TextChanged += txtBoxAESpassLength_TextChanged;
            txtBoxAESpassLength.KeyPress += txtBoxAESpassLength_KeyPress;
            // 
            // txtBoxArchivePassLength
            // 
            txtBoxArchivePassLength.Location = new Point(9, 47);
            txtBoxArchivePassLength.Name = "txtBoxArchivePassLength";
            txtBoxArchivePassLength.Size = new Size(125, 23);
            txtBoxArchivePassLength.TabIndex = 2;
            txtBoxArchivePassLength.TextChanged += txtBoxArchivePassLength_TextChanged;
            txtBoxArchivePassLength.KeyPress += txtBoxArchivePassLength_KeyPress;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(159, 29);
            label6.Name = "label6";
            label6.Size = new Size(204, 15);
            label6.TabIndex = 1;
            label6.Text = "Длина пароля AES шифр. (макс. 32)";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 29);
            label5.Name = "label5";
            label5.Size = new Size(126, 15);
            label5.TabIndex = 0;
            label5.Text = "Длина пароля архива";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(groupBox5);
            groupBox3.Location = new Point(12, 227);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(184, 85);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Система";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(radioBtnPriorityLow);
            groupBox5.Controls.Add(radioBtnPriorityNormal);
            groupBox5.Location = new Point(9, 22);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(169, 53);
            groupBox5.TabIndex = 0;
            groupBox5.TabStop = false;
            groupBox5.Text = "Приоритет программы";
            // 
            // radioBtnPriorityLow
            // 
            radioBtnPriorityLow.AutoSize = true;
            radioBtnPriorityLow.Location = new Point(95, 20);
            radioBtnPriorityLow.Name = "radioBtnPriorityLow";
            radioBtnPriorityLow.Size = new Size(66, 19);
            radioBtnPriorityLow.TabIndex = 1;
            radioBtnPriorityLow.TabStop = true;
            radioBtnPriorityLow.Text = "Низкий";
            radioBtnPriorityLow.UseVisualStyleBackColor = true;
            radioBtnPriorityLow.CheckedChanged += radioBtnPriorityLow_CheckedChanged;
            // 
            // radioBtnPriorityNormal
            // 
            radioBtnPriorityNormal.AutoSize = true;
            radioBtnPriorityNormal.Location = new Point(9, 20);
            radioBtnPriorityNormal.Name = "radioBtnPriorityNormal";
            radioBtnPriorityNormal.Size = new Size(80, 19);
            radioBtnPriorityNormal.TabIndex = 0;
            radioBtnPriorityNormal.TabStop = true;
            radioBtnPriorityNormal.Text = "Обычный";
            radioBtnPriorityNormal.UseVisualStyleBackColor = true;
            radioBtnPriorityNormal.CheckedChanged += radioBtnPriorityNormal_CheckedChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(textBoxHTTPclientTimeout);
            groupBox4.Controls.Add(label7);
            groupBox4.Location = new Point(204, 227);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(274, 85);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Сеть";
            // 
            // textBoxHTTPclientTimeout
            // 
            textBoxHTTPclientTimeout.Location = new Point(6, 52);
            textBoxHTTPclientTimeout.Name = "textBoxHTTPclientTimeout";
            textBoxHTTPclientTimeout.Size = new Size(100, 23);
            textBoxHTTPclientTimeout.TabIndex = 9;
            textBoxHTTPclientTimeout.TextChanged += textBoxHTTPclientTimeout_TextChanged;
            textBoxHTTPclientTimeout.KeyPress += textBoxHTTPclientTimeout_KeyPress;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 19);
            label7.Name = "label7";
            label7.Size = new Size(115, 30);
            label7.TabIndex = 8;
            label7.Text = "HTTP client timeout,\r\nв секундах";
            // 
            // applyButton
            // 
            applyButton.Location = new Point(297, 531);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(83, 25);
            applyButton.TabIndex = 8;
            applyButton.Text = "Применить";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += applyButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(386, 531);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 9;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // interfaceGroupBox
            // 
            interfaceGroupBox.Controls.Add(checkBoxEnableConsole);
            interfaceGroupBox.Controls.Add(groupBox7);
            interfaceGroupBox.Controls.Add(checkBoxOpenFolderAfterLoad);
            interfaceGroupBox.Controls.Add(checkBoxAskBeforeDelete);
            interfaceGroupBox.Controls.Add(checkBox1);
            interfaceGroupBox.Location = new Point(10, 318);
            interfaceGroupBox.Name = "interfaceGroupBox";
            interfaceGroupBox.Size = new Size(468, 101);
            interfaceGroupBox.TabIndex = 10;
            interfaceGroupBox.TabStop = false;
            interfaceGroupBox.Text = "Интерфейс";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(radioBtnSortByDate);
            groupBox7.Controls.Add(radioBtnSortByName);
            groupBox7.Location = new Point(269, 14);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(182, 77);
            groupBox7.TabIndex = 3;
            groupBox7.TabStop = false;
            groupBox7.Text = "Сортировка по";
            // 
            // radioBtnSortByDate
            // 
            radioBtnSortByDate.AutoSize = true;
            radioBtnSortByDate.Location = new Point(15, 43);
            radioBtnSortByDate.Name = "radioBtnSortByDate";
            radioBtnSortByDate.Size = new Size(50, 19);
            radioBtnSortByDate.TabIndex = 0;
            radioBtnSortByDate.TabStop = true;
            radioBtnSortByDate.Text = "Дата";
            radioBtnSortByDate.UseVisualStyleBackColor = true;
            radioBtnSortByDate.CheckedChanged += radioBtnSortByDate_CheckedChanged;
            // 
            // radioBtnSortByName
            // 
            radioBtnSortByName.AutoSize = true;
            radioBtnSortByName.Location = new Point(15, 18);
            radioBtnSortByName.Name = "radioBtnSortByName";
            radioBtnSortByName.Size = new Size(49, 19);
            radioBtnSortByName.TabIndex = 0;
            radioBtnSortByName.TabStop = true;
            radioBtnSortByName.Text = "Имя";
            radioBtnSortByName.UseVisualStyleBackColor = true;
            radioBtnSortByName.CheckedChanged += radioBtnSortByName_CheckedChanged;
            // 
            // checkBoxOpenFolderAfterLoad
            // 
            checkBoxOpenFolderAfterLoad.AutoSize = true;
            checkBoxOpenFolderAfterLoad.Location = new Point(12, 22);
            checkBoxOpenFolderAfterLoad.Name = "checkBoxOpenFolderAfterLoad";
            checkBoxOpenFolderAfterLoad.Size = new Size(251, 19);
            checkBoxOpenFolderAfterLoad.TabIndex = 2;
            checkBoxOpenFolderAfterLoad.Text = "Открыть папку с файлом после загрузки";
            checkBoxOpenFolderAfterLoad.UseVisualStyleBackColor = true;
            checkBoxOpenFolderAfterLoad.CheckedChanged += checkBoxOpenFolderAfterLoad_CheckedChanged;
            // 
            // checkBoxAskBeforeDelete
            // 
            checkBoxAskBeforeDelete.AutoSize = true;
            checkBoxAskBeforeDelete.Location = new Point(12, 47);
            checkBoxAskBeforeDelete.Name = "checkBoxAskBeforeDelete";
            checkBoxAskBeforeDelete.Size = new Size(193, 19);
            checkBoxAskBeforeDelete.TabIndex = 1;
            checkBoxAskBeforeDelete.Text = "Спрашивать перед удалением";
            checkBoxAskBeforeDelete.UseVisualStyleBackColor = true;
            checkBoxAskBeforeDelete.CheckedChanged += checkBoxAskBeforeDelete_CheckedChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(11, 72);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(58, 19);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Звуки";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(labelCompressionLevel);
            groupBox6.Controls.Add(comboBoxCompressionLvl);
            groupBox6.Controls.Add(label2);
            groupBox6.Controls.Add(textBoxFilesizeUpload);
            groupBox6.Location = new Point(11, 425);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(467, 100);
            groupBox6.TabIndex = 11;
            groupBox6.TabStop = false;
            groupBox6.Text = "Обработка файлов";
            // 
            // labelCompressionLevel
            // 
            labelCompressionLevel.AutoSize = true;
            labelCompressionLevel.Location = new Point(175, 34);
            labelCompressionLevel.Name = "labelCompressionLevel";
            labelCompressionLevel.Size = new Size(135, 15);
            labelCompressionLevel.TabIndex = 5;
            labelCompressionLevel.Text = "Степень сжатия архива";
            // 
            // comboBoxCompressionLvl
            // 
            comboBoxCompressionLvl.FormattingEnabled = true;
            comboBoxCompressionLvl.Location = new Point(179, 52);
            comboBoxCompressionLvl.Name = "comboBoxCompressionLvl";
            comboBoxCompressionLvl.Size = new Size(121, 23);
            comboBoxCompressionLvl.TabIndex = 4;
            comboBoxCompressionLvl.SelectedIndexChanged += comboBoxCompressionLvl_SelectedIndexChanged;
            comboBoxCompressionLvl.MouseHover += comboBoxCompressionLvl_MouseHover;
            // 
            // checkBoxEnableConsole
            // 
            checkBoxEnableConsole.AutoSize = true;
            checkBoxEnableConsole.Location = new Point(75, 72);
            checkBoxEnableConsole.Name = "checkBoxEnableConsole";
            checkBoxEnableConsole.Size = new Size(73, 19);
            checkBoxEnableConsole.TabIndex = 4;
            checkBoxEnableConsole.Text = "Консоль";
            checkBoxEnableConsole.UseVisualStyleBackColor = true;
            checkBoxEnableConsole.CheckedChanged += checkBoxEnableConsole_CheckedChanged;
            // 
            // SettingsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(490, 568);
            Controls.Add(groupBox6);
            Controls.Add(interfaceGroupBox);
            Controls.Add(cancelButton);
            Controls.Add(applyButton);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "SettingsWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Настройки";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            interfaceGroupBox.ResumeLayout(false);
            interfaceGroupBox.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private TextBox textBoxFilesizeUpload;
        private GroupBox groupBox1;
        private TextBox textBox3;
        private Label label3;
        private TextBox textBoxToken;
        private Label label4;
        private GroupBox groupBox2;
        private Label label6;
        private Label label5;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private TextBox txtBoxAESpassLength;
        private TextBox txtBoxArchivePassLength;
        private GroupBox groupBox5;
        private TextBox textBoxHTTPclientTimeout;
        private Label label7;
        private RadioButton radioBtnPriorityLow;
        private RadioButton radioBtnPriorityNormal;
        private Button applyButton;
        private Button cancelButton;
        private GroupBox interfaceGroupBox;
        private CheckBox checkBox1;
        private CheckBox checkBoxAskBeforeDelete;
        private CheckBox checkBoxOpenFolderAfterLoad;
        private GroupBox groupBox6;
        private Label labelCompressionLevel;
        private ComboBox comboBoxCompressionLvl;
        private GroupBox groupBox7;
        private RadioButton radioBtnSortByDate;
        private RadioButton radioBtnSortByName;
        private CheckBox checkBoxEnableConsole;
    }
}