using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Ionic.Zlib;
using Newtonsoft.Json.Linq;
using VKDrive;

namespace WinFormsApp1
{
    public partial class SettingsWindow : Form
    {
        internal Settings _settings;

        internal SettingsWindow(Settings settings)
        {
            MaximizeBox = false;
            InitializeComponent();
            _settings = settings;

            radioBtnPriorityLow.Checked = _settings.ProcessPriority == System.Diagnostics.ProcessPriorityClass.BelowNormal;
            radioBtnPriorityNormal.Checked = _settings.ProcessPriority == System.Diagnostics.ProcessPriorityClass.Normal;
            textBoxFilesizeUpload.Text = _settings.ChunkToUploadSize.ToString();
            textBoxHTTPclientTimeout.Text = _settings.HttpClientTimeout.ToString();
            txtBoxArchivePassLength.Text = _settings.ArchivePasswordLength.ToString();
            txtBoxAESpassLength.Text = _settings.AesPasswordLength.ToString();
            textBoxToken.Text = _settings.AccessToken;
            textBox3.Text = _settings.GroupID.ToString();
            checkBox1.Checked = _settings.SoundsOn;
            checkBoxAskBeforeDelete.Checked = _settings.AskBeforeDelete;
            checkBoxOpenFolderAfterLoad.Checked = _settings.OpenFolderAfterDownload;
            radioBtnSortByName.Checked = _settings.SortByName;
            radioBtnSortByDate.Checked = _settings.SortByDate;

            comboBoxCompressionLvl.DropDownStyle = ComboBoxStyle.DropDownList;
            PopulateCompressionLevelCombobox();
            comboBoxCompressionLvl.SelectedIndex = GetSelectedIndex(_settings.CompressionLevel);
        }

        private void PopulateCompressionLevelCombobox()
        {
            var levels = new string[4]
            {
                "Без сжатия",
                "Минимальная",
                "Обычная",
                "Максимальная"
            };

            foreach (var value in levels)
            {
                comboBoxCompressionLvl.Items.Add(value);
            }
        }

        private int GetSelectedIndex(MyCompressionLevels level)
        {
            return level switch
            {
                MyCompressionLevels.None => 0,
                MyCompressionLevels.Minimum => 1,
                MyCompressionLevels.Default => 2,
                MyCompressionLevels.Best => 3,
                _ => 404,
            };
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBoxFilesizeUpload_TextChanged(object sender, EventArgs e)
        {
            var textboxValue = textBoxFilesizeUpload.Text;
            int parsed = 0;
            if (int.TryParse(textboxValue, out parsed))
            {
                _settings.ChunkToUploadSize = parsed;
            }
        }

        private void textBoxFilesizeUpload_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // this will prevent the character from being entered
            }
        }

        private void textBoxHTTPclientTimeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBoxArchivePassLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBoxAESpassLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxHTTPclientTimeout_TextChanged(object sender, EventArgs e)
        {
            var textboxValue = textBoxHTTPclientTimeout.Text;
            int parsed = 0;
            if (int.TryParse(textboxValue, out parsed))
            {
                _settings.HttpClientTimeout = parsed;
            }
        }

        private void radioBtnPriorityNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnPriorityNormal.Checked)
            {
                _settings.ProcessPriority = System.Diagnostics.ProcessPriorityClass.Normal;
            }
        }

        private void radioBtnPriorityLow_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnPriorityLow.Checked)
            {
                _settings.ProcessPriority = System.Diagnostics.ProcessPriorityClass.BelowNormal;
            }
        }

        private void txtBoxAESpassLength_TextChanged(object sender, EventArgs e)
        {
            var textboxValue = txtBoxAESpassLength.Text;
            int parsed = 0;
            if (int.TryParse(textboxValue, out parsed))
            {
                _settings.AesPasswordLength = parsed;
            }
        }

        private void txtBoxArchivePassLength_TextChanged(object sender, EventArgs e)
        {
            var textboxValue = txtBoxArchivePassLength.Text;
            int parsed = 0;
            if (int.TryParse(textboxValue, out parsed))
            {
                _settings.ArchivePasswordLength = parsed;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SoundsOn = checkBox1.Checked;
        }

        private void textBoxToken_TextChanged(object sender, EventArgs e)
        {
            _settings.AccessToken = textBoxToken.Text;
        }

        private void checkBoxAskBeforeDelete_CheckedChanged(object sender, EventArgs e)
        {
            _settings.AskBeforeDelete = checkBoxAskBeforeDelete.Checked;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            var textboxValue = textBox3.Text;

            int parsed = 0;
            if (int.TryParse(textboxValue, out parsed))
            {
                _settings.GroupID = parsed;
            }
        }

        private void checkBoxOpenFolderAfterLoad_CheckedChanged(object sender, EventArgs e)
        {
            _settings.OpenFolderAfterDownload = checkBoxOpenFolderAfterLoad.Checked;
        }

        private void textBoxToken_Click(object sender, EventArgs e)
        {
            textBoxToken.SelectAll();
        }

        private void radioBtnSortByName_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SortByName = radioBtnSortByName.Checked;
        }

        private void radioBtnSortByDate_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SortByDate = radioBtnSortByDate.Checked;
        }

        private void comboBoxCompressionLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = comboBoxCompressionLvl.SelectedIndex;
            var selectedLevel = (MyCompressionLevels)selectedIndex;
            _settings.CompressionLevel = selectedLevel;
        }

        private void comboBoxCompressionLvl_MouseHover(object sender, EventArgs e)
        {
            var toolTip = new System.Windows.Forms.ToolTip();
            toolTip.AutoPopDelay = 2000;
            toolTip.InitialDelay = 100;

            toolTip.SetToolTip(comboBoxCompressionLvl, "Без сжатия: Файлы отправляются без сжатия.\r\nМинимальное: Сжатие файлов на минимальном уровне.\r\nСтандартное: Используется уровень сжатия по умолчанию.\r\nМаксимальное: Файлы сжимаются с максимально возможным уровнем сжатия.");

        }
    }
}
