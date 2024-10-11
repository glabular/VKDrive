namespace VKDrive.WinForms;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        buttonAddFile = new Button();
        listView1 = new ListView();
        columnHeader1 = new ColumnHeader();
        columnHeader2 = new ColumnHeader();
        columnHeader3 = new ColumnHeader();
        button_Delete_Selected = new Button();
        button_AddFolder = new Button();
        panel1 = new Panel();
        buttonSettings = new Button();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // buttonAddFile
        // 
        buttonAddFile.Location = new Point(7, 345);
        buttonAddFile.Margin = new Padding(2);
        buttonAddFile.Name = "buttonAddFile";
        buttonAddFile.Size = new Size(76, 24);
        buttonAddFile.TabIndex = 0;
        buttonAddFile.Text = "Add file";
        buttonAddFile.UseVisualStyleBackColor = true;
        buttonAddFile.Click += buttonAddFile_Click;
        // 
        // listView1
        // 
        listView1.AllowDrop = true;
        listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
        listView1.FullRowSelect = true;
        listView1.GridLines = true;
        listView1.Location = new Point(2, 2);
        listView1.Margin = new Padding(2);
        listView1.Name = "listView1";
        listView1.Size = new Size(445, 324);
        listView1.TabIndex = 1;
        listView1.UseCompatibleStateImageBehavior = false;
        listView1.View = View.Details;
        listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
        listView1.DragDrop += listView1_DragDrop;
        listView1.DragEnter += listView1_DragEnter;
        listView1.DragLeave += listView1_DragLeave;
        listView1.MouseDoubleClick += listView1_MouseDoubleClick;
        // 
        // columnHeader1
        // 
        columnHeader1.Text = "Name";
        columnHeader1.Width = 150;
        // 
        // columnHeader2
        // 
        columnHeader2.Text = "Size";
        columnHeader2.Width = 80;
        // 
        // columnHeader3
        // 
        columnHeader3.Text = "Added";
        columnHeader3.Width = 100;
        // 
        // button_Delete_Selected
        // 
        button_Delete_Selected.Location = new Point(279, 345);
        button_Delete_Selected.Margin = new Padding(2);
        button_Delete_Selected.Name = "button_Delete_Selected";
        button_Delete_Selected.Size = new Size(97, 23);
        button_Delete_Selected.TabIndex = 2;
        button_Delete_Selected.Text = "Delete";
        button_Delete_Selected.UseVisualStyleBackColor = true;
        button_Delete_Selected.Click += button_Delete_Selected_Click;
        // 
        // button_AddFolder
        // 
        button_AddFolder.Location = new Point(87, 345);
        button_AddFolder.Margin = new Padding(2);
        button_AddFolder.Name = "button_AddFolder";
        button_AddFolder.Size = new Size(76, 24);
        button_AddFolder.TabIndex = 3;
        button_AddFolder.Text = "Add folder";
        button_AddFolder.UseVisualStyleBackColor = true;
        button_AddFolder.Click += button_AddFolder_Click;
        // 
        // panel1
        // 
        panel1.BackColor = Color.Transparent;
        panel1.Controls.Add(listView1);
        panel1.Location = new Point(7, 6);
        panel1.Margin = new Padding(2);
        panel1.Name = "panel1";
        panel1.Size = new Size(449, 328);
        panel1.TabIndex = 4;
        panel1.Paint += panel1_Paint;
        // 
        // buttonSettings
        // 
        buttonSettings.Location = new Point(381, 346);
        buttonSettings.Name = "buttonSettings";
        buttonSettings.Size = new Size(75, 23);
        buttonSettings.TabIndex = 5;
        buttonSettings.Text = "Settings";
        buttonSettings.UseVisualStyleBackColor = true;
        buttonSettings.Click += buttonSettings_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(467, 380);
        Controls.Add(buttonSettings);
        Controls.Add(button_AddFolder);
        Controls.Add(button_Delete_Selected);
        Controls.Add(buttonAddFile);
        Controls.Add(panel1);
        Margin = new Padding(2);
        Name = "Form1";
        Text = "VKDrive";
        panel1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Button buttonAddFile;
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private Button button_Delete_Selected;
    private Button button_AddFolder;
    private Panel panel1;
    private Button buttonSettings;
}
