using SharedEntities.Settings;
using System.Diagnostics;

namespace VKDrive.WinForms;

public partial class Form1 : Form
{
    private readonly ApiService _apiService;
    private readonly Settings _settings;
    private float dashOffset = 0f;
    private System.Windows.Forms.Timer _timer;
    private bool isMouseOverPanel = false;

    public Form1()
    {
        InitializeComponent();
        InitializeTimer();
        _settings = SettingsManager.LoadSettings();
        _apiService = new ApiService();
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        PopulateListView().ContinueWith(OnMyAsyncMethodFailed, TaskContinuationOptions.OnlyOnFaulted);
    }

    public static void OnMyAsyncMethodFailed(Task task)
    {
        Exception ex = task.Exception;
        // Deal with exceptions here however you want
        MessageBox.Show(ex.Message);
    }

    private async Task PopulateListView()
    {
        try
        {
            var entries = await _apiService.GetEntriesAsync();
            if (entries != null && entries.Count != 0)
            {
                listView1.Items.Clear();

                foreach (var entry in entries)
                {
                    var item = new ListViewItem(entry.OriginalName);
                    item.SubItems.Add(entry.GetReadableSize());
                    item.SubItems.Add(entry.CreationDate.ToShortDateString());
                    item.Tag = entry.UniqueName;
                    listView1.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("No entries found.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error loading data", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void buttonAddFile_Click(object sender, EventArgs e)
    {
        var maxFilesAllowed = 5;

        using var openFileDialog = new OpenFileDialog
        {
            Filter = "All Files (*.*)|*.*",
            Multiselect = true,
            Title = "Select up to 5 files..."
        };

        var result = openFileDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            var selectedFileNames = openFileDialog.FileNames;

            if (selectedFileNames.Length > maxFilesAllowed)
            {
                MessageBox.Show($"Please select up to {maxFilesAllowed} files only.", "Selection Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var filePath in selectedFileNames)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    var apiResult = await _apiService.AddEntryAsync(filePath);
                    MessageBox.Show(apiResult);
                }
                else
                {
                    MessageBox.Show("Invalid file path.");
                }
            }

            await PopulateListView();
        }
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private async void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        listView1.Enabled = false;

        if (listView1.SelectedItems.Count > 0)
        {
            var selectedItem = listView1.SelectedItems[0];
            var uniqueName = selectedItem.Tag as string;

            if (!string.IsNullOrEmpty(uniqueName))
            {
                var path = await _apiService.DownloadEntryAsync(uniqueName);

                if (File.Exists(path) || Directory.Exists(path))
                {
                    if (_settings.OpenFolderAfterDownload)
                    {
                        Process.Start("explorer.exe", $"/select,{path}");
                    }
                }
                else
                {
                    MessageBox.Show("Couldn't download the entry or the file/directory doesn't exist.", "Error!");
                }
            }
        }

        listView1.Enabled = true;
    }

    private async void button_Delete_Selected_Click(object sender, EventArgs e)
    {
        if (listView1.SelectedItems.Count > 0)
        {
            var selectedItem = listView1.SelectedItems[0];
            var uniqueName = selectedItem.Tag as string;

            var confirmResult = MessageBox.Show($"Are you sure you want to delete the selected item?\n{selectedItem.Name}",
                                                "Confirm Deletion",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                if (await _apiService.DeleteEntryAsync(uniqueName))
                {
                    listView1.Items.Remove(selectedItem);
                }
            }
        }
        else
        {
            MessageBox.Show("Please select an item to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async void button_AddFolder_Click(object sender, EventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();
        var result = folderBrowserDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            // Get all files in the selected folder
            var selectedFolderPath = folderBrowserDialog.SelectedPath;

            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                var apiResult = await _apiService.AddEntryAsync(selectedFolderPath);
                MessageBox.Show(apiResult);
            }
            else
            {
                MessageBox.Show("Invalid file path.");
            }

            await PopulateListView();
        }
    }

    private void listView1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            isMouseOverPanel = true;
            e.Effect = DragDropEffects.Copy;
            _timer.Start();
        }
    }

    private void listView1_DragLeave(object sender, EventArgs e)
    {
        isMouseOverPanel = false;
        _timer.Stop();
        panel1.Invalidate();
    }

    private async void listView1_DragDrop(object sender, DragEventArgs e)
    {
        _timer.Stop();
        isMouseOverPanel = false;
        panel1.Invalidate();
        e.Effect = DragDropEffects.None;

        if (e.Data is not IDataObject data
            || data.GetData(DataFormats.FileDrop) is not string[] droppedData
            || droppedData.Length == 0)
        {
            MessageBox.Show("No files or folders were dropped.", "Invalid Drop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (droppedData.Length > 5)
        {
            var confirmResult = MessageBox.Show(
                $"You have dropped {droppedData.Length} items. Only 5 items can be processed at a time. Do you want to proceed with the first 5 items?",
                "Too Many Items",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmResult == DialogResult.No)
            {
                // Exit if the user chooses not to proceed
                return;
            }
        }

        // Take the first 5 items if more than 5 are dropped
        var selectedItems = droppedData.Take(5).ToList();
        foreach (var item in selectedItems)
        {
            var apiResult = await _apiService.AddEntryAsync(item);
            MessageBox.Show(apiResult);
        }

        await PopulateListView();
    }

    private void InitializeTimer()
    {
        _timer = new System.Windows.Forms.Timer();
        _timer.Interval = 30; // Adjust the interval for speed of dash movement
        _timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        dashOffset -= 1f; // Adjust this value for the movement speed of the dashes
        if (dashOffset <= -4f) // Reset the dash offset to keep it within the dash pattern range
        {
            dashOffset = 0f;
        }

        panel1.Invalidate(); // Force the panel to repaint
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        if (isMouseOverPanel)
        {
            var pen = new Pen(Color.Black, 4)
            {
                DashPattern = [2, 2],
                DashOffset = dashOffset
            };
            e.Graphics.DrawRectangle(pen, 2, 2, panel1.Width - 4, panel1.Height - 4);
        }
    }

    private void buttonSettings_Click(object sender, EventArgs e)
    {
        var settingsWindow = new SettingsForm();
        settingsWindow.ShowDialog();
    }
}
