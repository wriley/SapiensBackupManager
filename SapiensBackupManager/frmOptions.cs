using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SapiensBackupManager
{
    public partial class frmOptions : Form
    {
        frmMain parentForm;

        public frmOptions(frmMain frm)
        {
            InitializeComponent();
            parentForm = new frmMain();
            parentForm = frm;
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            textBoxBackupFolder.Text = parentForm.backupFolder;
            textBoxSaveFolder.Text = parentForm.saveFolder;
        }

        private void buttonOptionsSave_Click(object sender, EventArgs e)
        {
            parentForm.SaveSettings();
            Close();
        }

        private void buttonOptionsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonBackupFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                parentForm.backupFolder = folderBrowserDialog1.SelectedPath;
                textBoxBackupFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void buttonSaveFolder_Click(object sender, EventArgs e)
        {
            // %appdata%\majicjungle\sapiens\players\STEAMID\worlds
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.ApplicationData;
            folderBrowserDialog1.SelectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "majicjungle", "sapiens", "players");
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                parentForm.saveFolder = folderBrowserDialog1.SelectedPath;
                textBoxSaveFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
