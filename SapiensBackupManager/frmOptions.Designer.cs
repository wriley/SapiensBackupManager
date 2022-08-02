namespace SapiensBackupManager
{
    partial class frmOptions
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
            this.labelBackupFolder = new System.Windows.Forms.Label();
            this.textBoxBackupFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonBackupFolder = new System.Windows.Forms.Button();
            this.buttonOptionsSave = new System.Windows.Forms.Button();
            this.buttonOptionsCancel = new System.Windows.Forms.Button();
            this.textBoxSaveFolder = new System.Windows.Forms.TextBox();
            this.labelSaveFolder = new System.Windows.Forms.Label();
            this.buttonSaveFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelBackupFolder
            // 
            this.labelBackupFolder.AutoSize = true;
            this.labelBackupFolder.Location = new System.Drawing.Point(98, 15);
            this.labelBackupFolder.Name = "labelBackupFolder";
            this.labelBackupFolder.Size = new System.Drawing.Size(76, 13);
            this.labelBackupFolder.TabIndex = 0;
            this.labelBackupFolder.Text = "Backup Folder";
            // 
            // textBoxBackupFolder
            // 
            this.textBoxBackupFolder.Location = new System.Drawing.Point(180, 12);
            this.textBoxBackupFolder.Name = "textBoxBackupFolder";
            this.textBoxBackupFolder.ReadOnly = true;
            this.textBoxBackupFolder.Size = new System.Drawing.Size(464, 20);
            this.textBoxBackupFolder.TabIndex = 1;
            // 
            // buttonBackupFolder
            // 
            this.buttonBackupFolder.Location = new System.Drawing.Point(650, 10);
            this.buttonBackupFolder.Name = "buttonBackupFolder";
            this.buttonBackupFolder.Size = new System.Drawing.Size(24, 23);
            this.buttonBackupFolder.TabIndex = 2;
            this.buttonBackupFolder.Text = "...";
            this.buttonBackupFolder.UseVisualStyleBackColor = true;
            this.buttonBackupFolder.Click += new System.EventHandler(this.buttonBackupFolder_Click);
            // 
            // buttonOptionsSave
            // 
            this.buttonOptionsSave.Location = new System.Drawing.Point(306, 172);
            this.buttonOptionsSave.Name = "buttonOptionsSave";
            this.buttonOptionsSave.Size = new System.Drawing.Size(75, 23);
            this.buttonOptionsSave.TabIndex = 3;
            this.buttonOptionsSave.Text = "Save";
            this.buttonOptionsSave.UseVisualStyleBackColor = true;
            this.buttonOptionsSave.Click += new System.EventHandler(this.buttonOptionsSave_Click);
            // 
            // buttonOptionsCancel
            // 
            this.buttonOptionsCancel.Location = new System.Drawing.Point(387, 172);
            this.buttonOptionsCancel.Name = "buttonOptionsCancel";
            this.buttonOptionsCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonOptionsCancel.TabIndex = 4;
            this.buttonOptionsCancel.Text = "Cancel";
            this.buttonOptionsCancel.UseVisualStyleBackColor = true;
            this.buttonOptionsCancel.Click += new System.EventHandler(this.buttonOptionsCancel_Click);
            // 
            // textBoxSaveFolder
            // 
            this.textBoxSaveFolder.Location = new System.Drawing.Point(180, 38);
            this.textBoxSaveFolder.Name = "textBoxSaveFolder";
            this.textBoxSaveFolder.ReadOnly = true;
            this.textBoxSaveFolder.Size = new System.Drawing.Size(464, 20);
            this.textBoxSaveFolder.TabIndex = 6;
            // 
            // labelSaveFolder
            // 
            this.labelSaveFolder.AutoSize = true;
            this.labelSaveFolder.Location = new System.Drawing.Point(35, 41);
            this.labelSaveFolder.Name = "labelSaveFolder";
            this.labelSaveFolder.Size = new System.Drawing.Size(139, 13);
            this.labelSaveFolder.TabIndex = 5;
            this.labelSaveFolder.Text = "Save Games Folder (worlds)";
            // 
            // buttonSaveFolder
            // 
            this.buttonSaveFolder.Location = new System.Drawing.Point(650, 36);
            this.buttonSaveFolder.Name = "buttonSaveFolder";
            this.buttonSaveFolder.Size = new System.Drawing.Size(24, 23);
            this.buttonSaveFolder.TabIndex = 7;
            this.buttonSaveFolder.Text = "...";
            this.buttonSaveFolder.UseVisualStyleBackColor = true;
            this.buttonSaveFolder.Click += new System.EventHandler(this.buttonSaveFolder_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 207);
            this.Controls.Add(this.buttonSaveFolder);
            this.Controls.Add(this.textBoxSaveFolder);
            this.Controls.Add(this.labelSaveFolder);
            this.Controls.Add(this.buttonOptionsCancel);
            this.Controls.Add(this.buttonOptionsSave);
            this.Controls.Add(this.buttonBackupFolder);
            this.Controls.Add(this.textBoxBackupFolder);
            this.Controls.Add(this.labelBackupFolder);
            this.Name = "frmOptions";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelBackupFolder;
        private System.Windows.Forms.TextBox textBoxBackupFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonBackupFolder;
        private System.Windows.Forms.Button buttonOptionsSave;
        private System.Windows.Forms.Button buttonOptionsCancel;
        private System.Windows.Forms.TextBox textBoxSaveFolder;
        private System.Windows.Forms.Label labelSaveFolder;
        private System.Windows.Forms.Button buttonSaveFolder;
    }
}