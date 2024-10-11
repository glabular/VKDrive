using SharedEntities.Models;
using SharedEntities.Settings;
using System.Text.Json;

namespace VKDrive.WinForms;

public partial class SettingsForm : Form
{
    private Settings _settings;

    public SettingsForm()
    {
        InitializeComponent();
        _settings = SettingsManager.LoadSettings();
        BindSettingsToUI();
    }

    private void BindSettingsToUI()
    {
        radioButtonLowPriority.Checked = _settings.ProcessPriority == System.Diagnostics.ProcessPriorityClass.BelowNormal;
        radioButtonNormalPriority.Checked = _settings.ProcessPriority == System.Diagnostics.ProcessPriorityClass.Normal;
        textBoxChunkSize.Text = _settings.ChunkToUploadSizeInMegabytes.ToString();
        checkBoxOpenFolderAfterDownload.Checked = _settings.OpenFolderAfterDownload;
        checkBoxAskDeletion.Checked = _settings.AskBeforeDelete;
        checkBoxSoundsOn.Checked = _settings.SoundsOn;
        textBoxGroupId.Text = _settings.GroupID.ToString();
        textBoxApiKey.Text = _settings.VkAccessToken;
        textBoxArchivePasswordLength.Text = _settings.ArchivePasswordLength.ToString();

        comboBoxCompressionLevel.DropDownStyle = ComboBoxStyle.DropDownList;
        PopulateCompressionLevelCombobox();
        comboBoxCompressionLevel.SelectedIndex = GetSelectedIndex(_settings.CompressionLevel);
    }

    private void buttonSaveSettings_Click(object sender, EventArgs e)
    {
        if (_settings.GroupID <= 0)
        {
            MessageBox.Show("Group ID must be a positive integer.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.VkAccessToken))
        {
            MessageBox.Show("Access token cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var securePasswordLength = 16;
        if (_settings.ArchivePasswordLength < securePasswordLength)
        {
            MessageBox.Show($"Password length must be at least {securePasswordLength} characters.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            SettingsManager.SaveSettings(_settings);
            MessageBox.Show("Settings saved successfully.");
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateCompressionLevelCombobox()
    {
        var levels = new string[4]
        {
            "No compression",
            "Minimal",
            "Default",
            "Best"
        };

        foreach (var value in levels)
        {
            comboBoxCompressionLevel.Items.Add(value);
        }
    }

    private static int GetSelectedIndex(MyCompressionLevel level)
    {
        return level switch
        {
            MyCompressionLevel.None => 0,
            MyCompressionLevel.Minimal => 1,
            MyCompressionLevel.Default => 2,
            MyCompressionLevel.Best => 3,
            _ => 404,
        };
    }

    private void textBoxGroupId_TextChanged(object sender, EventArgs e)
    {
        var textboxValue = textBoxGroupId.Text;

        if (textboxValue.StartsWith('0'))
        {
            textBoxGroupId.Clear();
            return;
        }

        if (int.TryParse(textboxValue, out var parsed))
        {
            _settings.GroupID = parsed;
        }
        else
        {
            if (textBoxGroupId.TextLength > 0)
            {
                MessageBox.Show("The input is not a valid group ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxGroupId.Clear();
            }
        }
    }

    private void textBoxArchivePasswordLength_TextChanged(object sender, EventArgs e)
    {
        var textboxValue = textBoxArchivePasswordLength.Text;

        if (textboxValue.StartsWith('0'))
        {
            textBoxArchivePasswordLength.Clear();
            return;
        }

        if (int.TryParse(textboxValue, out var parsed))
        {
            _settings.ArchivePasswordLength = parsed;
        }
        else
        {
            if (textBoxArchivePasswordLength.TextLength > 0)
            {
                MessageBox.Show("The input is not a valid password length.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxArchivePasswordLength.Clear();
            }
        }
    }

    private void textBoxChunkSize_TextChanged(object sender, EventArgs e)
    {
        var textboxValue = textBoxChunkSize.Text;

        if (textboxValue.StartsWith('0'))
        {
            textBoxChunkSize.Clear();
            return;
        }

        if (int.TryParse(textboxValue, out var parsed))
        {
            if (parsed > 200)
            {
                MessageBox.Show("VK API restricts file size to be more than 200 MB.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxChunkSize.SelectAll();
            }
            else
            {
                _settings.ChunkToUploadSizeInMegabytes = parsed;
            }
        }
        else
        {
            if (textBoxChunkSize.TextLength > 0)
            {
                MessageBox.Show("The input is not a valid file size.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxChunkSize.Clear();
            }
        }
    }

    private void textBoxGroupId_KeyPress(object sender, KeyPressEventArgs e)
    {
        AllowDigitsOnlyInput(e);
    }

    private void textBoxArchivePasswordLength_KeyPress(object sender, KeyPressEventArgs e)
    {
        AllowDigitsOnlyInput(e);
    }

    private void textBoxChunkSize_KeyPress(object sender, KeyPressEventArgs e)
    {
        AllowDigitsOnlyInput(e);
    }

    private static void AllowDigitsOnlyInput(KeyPressEventArgs e)
    {
        var isDigitOrControl = char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar);

        if (!isDigitOrControl)
        {
            // this will prevent the character from being entered
            e.Handled = true;
        }
    }

    private void radioButtonNormalPriority_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ProcessPriority = System.Diagnostics.ProcessPriorityClass.Normal;
    }

    private void radioButtonLowPriority_CheckedChanged(object sender, EventArgs e)
    {
        _settings.ProcessPriority = System.Diagnostics.ProcessPriorityClass.BelowNormal;
    }

    private void textBoxApiKey_Click(object sender, EventArgs e)
    {
        textBoxApiKey.SelectAll();
    }

    private void checkBoxOpenFolderAfterDownload_CheckedChanged(object sender, EventArgs e)
    {
        _settings.OpenFolderAfterDownload = checkBoxOpenFolderAfterDownload.Checked;
    }

    private void checkBoxSoundsOn_CheckedChanged(object sender, EventArgs e)
    {
        _settings.SoundsOn = checkBoxSoundsOn.Checked;
    }

    private void checkBoxAskDeletion_CheckedChanged(object sender, EventArgs e)
    {
        _settings.AskBeforeDelete = checkBoxAskDeletion.Checked;
    }

    private void textBoxApiKey_TextChanged(object sender, EventArgs e)
    {
        _settings.VkAccessToken = textBoxApiKey.Text.Trim();
    }

    private void buttonCancelSettings_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void comboBoxCompressionLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedIndex = comboBoxCompressionLevel.SelectedIndex;
        var selectedLevel = (MyCompressionLevel)selectedIndex;
        _settings.CompressionLevel = selectedLevel;
    }
}
