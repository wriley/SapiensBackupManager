using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SapiensBackupManager
{
    public partial class frmMain : Form
    {
        public string backupFolder;
        public string saveFolder;
        private string saveGamePath;
        private string timestampString = "yyyyMMdd-HHmmss";
        private List<TreeNode> unselectableSaveNodes = new List<TreeNode>();
        private List<TreeNode> unselectableBackupNodes = new List<TreeNode>();
        private List<SapiensSaveGame> mySaveGames = new List<SapiensSaveGame>();
        private List<SapiensSaveGame> backupSaveGames = new List<SapiensSaveGame>();

        private struct SapiensSaveGame
        {
            public string directoryName;
            public string worldName;
            public string seed;
            public double worldTime;
            public long creationTime;
            public long lastPlayedTime;
            public string lastPlayedVersion;
            public string backupName;
        }

        public class SapienInfo
        {
            public Value0 value0 { get; set; }
        }

        public class Value0
        {
            public int cereal_class_version { get; set; }
            public string worldName { get; set; }
            public string seed { get; set; }
            public double worldTime { get; set; }
            public double creationTime { get; set; }
            public double lastPlayedTime { get; set; }
            public string lastPlayedVersion { get; set; }
            public int versionCompatibilityIndex { get; set; }
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();

            if (!Directory.Exists(backupFolder) || !Directory.Exists(saveFolder))
            {
                MessageBox.Show("You need to set your options");
                frmOptions frmOptions = new frmOptions(this);
                frmOptions.Show(this);
            }
            saveGamePath = saveFolder;
            RefreshLists();
        }

        private void LoadSettings()
        {
            backupFolder = Properties.Settings.Default.backupFolder;
            saveFolder = Properties.Settings.Default.saveFolder;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.backupFolder = backupFolder;
            Properties.Settings.Default.saveFolder = saveFolder;
            Properties.Settings.Default.Save();
            saveGamePath = saveFolder;
            GetBackupFiles();
            RefreshLists();
        }

        private void DebugLog(string msg)
        {
            textBoxDebug.AppendText(msg + "\r\n");
        }

        private string getNodeText(XmlDocument document, string name)
        {
            XmlNode node = document.DocumentElement.SelectSingleNode(name);
            if(node == null)
            {
                return "N/A";
            } else
            {
                return node.InnerText;
            }
        }

        private DateTime LocalDateTimeFromUnix(long time)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(time);
            return dateTimeOffset.DateTime.ToLocalTime();
        }

        private void GetSaveGames()
        {
            if(!Directory.Exists(saveGamePath))
            {
                DebugLog("Save game path not found, check options!");
                return;
            }
            mySaveGames = new List<SapiensSaveGame>();
            string[] dirs = Directory.GetDirectories(saveGamePath);
            foreach (string dir in dirs)
            {
                DebugLog("Examining: " + dir);
                if (Directory.Exists(dir))
                {
                    string dirName = new DirectoryInfo(dir).Name;
                    Regex r = new Regex("^[a-f0-9]+$");
                    Match m = r.Match(dirName);
                    if (m.Success)
                    {
                        DebugLog("Found save game directory: " + dirName);
                        string gameJSONFile = dir + Path.DirectorySeparatorChar + "info.json";

                        if (File.Exists(gameJSONFile))
                        {
                            DebugLog("Found JSON " + gameJSONFile);
                            SapienInfo source = new SapienInfo();
                            string json = File.ReadAllText(gameJSONFile);
                            source = JsonSerializer.Deserialize<SapienInfo>(json);
                            SapiensSaveGame save = new SapiensSaveGame();                           
                            DirectoryInfo di = new DirectoryInfo(dir);
                            save.directoryName = di.Name;
                            save.worldName = source.value0.worldName;
                            save.seed = source.value0.seed;
                            save.worldTime = source.value0.worldTime;
                            save.creationTime = Convert.ToInt32(source.value0.creationTime);
                            save.lastPlayedTime = Convert.ToInt32(source.value0.lastPlayedTime);
                            save.lastPlayedVersion = source.value0.lastPlayedVersion;
                            DebugLog("worldName: " + save.worldName);
                            DebugLog("adding " + save.directoryName);
                            mySaveGames.Add(save);
                        }
                    }
                }
            }
            mySaveGames = mySaveGames.OrderBy(sel => sel.directoryName, new OrdinalStringComparer()).ToList();
            treeViewSavegames.BeginUpdate();
            treeViewSavegames.Nodes.Clear();
            unselectableSaveNodes.Clear();
            for (int i = 0; i < mySaveGames.Count; i++)
            {
                DebugLog("looking at " + mySaveGames[i].directoryName);
                TreeNode newParentNode = treeViewSavegames.Nodes.Add(String.Format("{0}: {1}", mySaveGames[i].worldName, LocalDateTimeFromUnix(mySaveGames[i].lastPlayedTime)));
                TreeNode newChildNode = newParentNode.Nodes.Add(String.Format("Directory: {0}", mySaveGames[i].directoryName));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Created: {0}", LocalDateTimeFromUnix(mySaveGames[i].creationTime)));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Last Played: {0}", LocalDateTimeFromUnix(mySaveGames[i].lastPlayedTime)));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Version: {0}", mySaveGames[i].lastPlayedVersion));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Seed: {0}", mySaveGames[i].seed));
                unselectableSaveNodes.Add(newChildNode);
                DateTime latest = GetLatestZipDate(mySaveGames[i].directoryName);
                //DebugLog("latest: " + latest.ToString());
                if(latest.CompareTo(LocalDateTimeFromUnix(mySaveGames[i].lastPlayedTime)) >= 0)
                {
                    newParentNode.ForeColor = System.Drawing.Color.Green;
                    //newParentNode.NodeFont = new System.Drawing.Font(treeViewBackups.Font, System.Drawing.FontStyle.Bold);
                }
            }
            treeViewSavegames.EndUpdate();
        }

        private void GetBackupFiles()
        {
            if (!Directory.Exists(backupFolder))
            {
                DebugLog("Backup folder not found, check options!");
                return;
            }
            string[] backupFiles = Directory.GetFiles(backupFolder);
            backupSaveGames = new List<SapiensSaveGame>();
            Regex r = new Regex(@"^[\w\s]+_([a-f0-9]+)_[0-9]{8}-[0-9]{6}.zip$");
            Match m;
            foreach (string backupFile in backupFiles)
            {
                if (File.Exists(backupFile))
                {
                    string fileName = new FileInfo(backupFile).Name;
                    DebugLog("Checking file " + backupFile);
                    m = r.Match(fileName);
                    if (m.Success && m.Groups[1].Value != null)
                    {
                        DebugLog("Getting info from zip " + fileName);
                        using (ZipInputStream s = new ZipInputStream(File.OpenRead(backupFile)))
                        {
                            ZipEntry theEntry;
                            while ((theEntry = s.GetNextEntry()) != null)
                            {
                                if (theEntry.Name == "info.json")
                                {
                                    SapiensSaveGame save = new SapiensSaveGame();
                                    SapienInfo source = new SapienInfo();
                                    source = JsonSerializer.Deserialize<SapienInfo>(s);
                                    save.backupName = fileName;
                                    save.directoryName = m.Groups[1].Value;
                                    save.worldName = source.value0.worldName;
                                    save.seed = source.value0.seed;
                                    save.worldTime = source.value0.worldTime;
                                    save.creationTime = Convert.ToInt32(source.value0.creationTime);
                                    save.lastPlayedTime = Convert.ToInt32(source.value0.lastPlayedTime);
                                    save.lastPlayedVersion = source.value0.lastPlayedVersion;
                                    backupSaveGames.Add(save);
                                }
                            }
                            s.Close();
                        }
                    }
                }
            }
            backupSaveGames = backupSaveGames.OrderBy(sel => sel.backupName, new OrdinalStringComparer()).ToList();
            treeViewBackups.BeginUpdate();
            treeViewBackups.Nodes.Clear();
            unselectableBackupNodes.Clear();
            for (int i = 0; i < backupSaveGames.Count; i++)
            {
                TreeNode newParentNode = treeViewBackups.Nodes.Add(String.Format("{0}", backupSaveGames[i].backupName));
                TreeNode newChildNode = newParentNode.Nodes.Add(String.Format("Directory: {0}", backupSaveGames[i].directoryName));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Created: {0}", LocalDateTimeFromUnix(backupSaveGames[i].creationTime)));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Last Played: {0}", LocalDateTimeFromUnix(backupSaveGames[i].lastPlayedTime)));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Version: {0}", backupSaveGames[i].lastPlayedVersion));
                unselectableSaveNodes.Add(newChildNode);
                newChildNode = newParentNode.Nodes.Add(String.Format("Seed: {0}", backupSaveGames[i].seed));
                unselectableSaveNodes.Add(newChildNode);
            }
            treeViewBackups.EndUpdate();
        }

        private DateTime GetSaveGameDate(string save)
        {
            for (int i = 0; i < mySaveGames.Count; i++)
            {
                if (mySaveGames[i].directoryName == save)
                {
                    return DateTime.FromOADate(mySaveGames[i].lastPlayedTime);
                }
            }
            return DateTime.MinValue;
        }

        private DateTime GetLatestZipDate(string save)
        {
            DateTime latest = DateTime.MinValue;
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < backupSaveGames.Count; i++)
            {
                Regex r = new Regex(@"^[\w\s]+_([a-f0-9]+)_([0-9]{8}-[0-9]{6}).zip$");
                Match m = r.Match(backupSaveGames[i].backupName);
                if (m.Success)
                {
                    if (m.Groups[1].Value == save)
                    {
                        DateTime zipDate = new DateTime();
                        DateTime.TryParseExact(m.Groups[2].Value, "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out zipDate);
                        if (zipDate != DateTime.MinValue && zipDate.CompareTo(lastDate) == 1)
                        {
                            latest = zipDate;
                        }
                    }
                }
            }
            return latest;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            textBoxDebug.SelectionStart = textBoxDebug.Text.Length;
            textBoxDebug.ScrollToCaret();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions frmOptions = new frmOptions(this);
            frmOptions.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frmAbout = new frmAbout();
            frmAbout.ShowDialog(this);
        }

        private void buttonBackup_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(backupFolder))
            {
                DebugLog("Backup folder not found, check options!");
                return;
            }
            if (treeViewSavegames.SelectedNode != null)
            {

                string worldText = treeViewSavegames.SelectedNode.Text;
                int i = worldText.IndexOf(':');
                string worldName = worldText.Substring(0, i);
                string dirText = treeViewSavegames.SelectedNode.Nodes[0].Text;
                i = dirText.IndexOf(':');
                string directoryName = dirText.Substring(i+2, dirText.Length-i-2);
                showUI(false);
                SaveGame(worldName, directoryName);
                showUI(true);
                RefreshLists();
            }
            else
            {
                DebugLog("No save game selected to backup!");
            }
        }

        private void buttonRestore_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(saveGamePath))
            {
                DebugLog("Save game path not found, check options!");
                return;
            }
            if (treeViewBackups.SelectedNode != null)
            {
                string backupName = treeViewBackups.SelectedNode.Text;
                showUI(false);
                RestoreGame(backupName);
                showUI(true);
                RefreshLists();
            }
            else
            {
                DebugLog("No backup file selected to restore!");
            }
        }

        private void SaveGame(string worldName, string directoryName)
        {
            DebugLog("Saving game " + worldName + " " + directoryName);
            string mySaveGameDir = saveGamePath + Path.DirectorySeparatorChar + directoryName;
            if (Directory.Exists(mySaveGameDir))
            {
                string dateString = DateTime.Now.ToString(timestampString);
                string zipFilePath = backupFolder + Path.DirectorySeparatorChar + worldName + "_" + directoryName + "_" + dateString + ".zip";
                DebugLog("zipping to " + zipFilePath);
                ZipFolder(mySaveGameDir, zipFilePath);
                GetBackupFiles();
                DebugLog("SaveGame complete");
            }
            else
            {
                DebugLog("Error: Directory not found " + mySaveGameDir);
            }
        }

        private void ZipFolder(string sourceDir, string zipFile)
        {
            try
            {
                string[] filenames = Directory.GetFiles(sourceDir);

                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFile)))
                {
                    s.SetLevel(3);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        var entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                DebugLog("Exception during zip file creation: " + ex.Message);
            }
        }

        private void UnzipFile(string zipFile, string targetDir)
        {
            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);

                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(Path.Combine(targetDir, directoryName));
                        }

                        if (fileName != string.Empty)
                        {
                            var filePath = Path.Combine(targetDir, fileName);
                            using (FileStream streamWriter = File.Create(filePath))
                            {
                                int size = 2048;
                                byte[] data = new byte[size];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog("Exception while unzipping file: " + ex.Message);
            }
        }

        private void RestoreGame(string backupName)
        {
            DebugLog("Restoring game " + backupName);
            string dirNameFull = new FileInfo(backupName).Name;
            Regex r = new Regex(@"^[\w\s]+_([a-f0-9]+)_[0-9]{8}-[0-9]{6}.zip$");
            Match m = r.Match(dirNameFull);
            DebugLog("dirNameFull " + dirNameFull);
            if (m.Success)
            {
                if (m.Groups[1].Value != null)
                {
                    string dirName = m.Groups[1].Value;
                    string mySaveGameDir = saveGamePath + Path.DirectorySeparatorChar + dirName;
                    DebugLog("dirName " + dirName);
                    if (Directory.Exists(mySaveGameDir))
                    {
                        DebugLog(dirName + " already exists!");
                        DialogResult result = MessageBox.Show(dirName + " already exists, overwrite?", "Overwrite Save?", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            // file system calls can be delayed so wait for folder to be deleted
                            var fi = new System.IO.FileInfo(mySaveGameDir);
                            if (fi.Exists)
                            {
                                Directory.Delete(mySaveGameDir, true);
                                fi.Refresh();
                                while(fi.Exists)
                                {
                                    System.Threading.Thread.Sleep(100);
                                    fi.Refresh();
                                }
                            }
                        }
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    Directory.CreateDirectory(mySaveGameDir);
                    string zipFilePath = backupFolder + Path.DirectorySeparatorChar + backupName;
                    DebugLog("Unzipping from " + zipFilePath);
                    UnzipFile(zipFilePath, mySaveGameDir);
                    GetSaveGames();
                }
                else
                {
                    DebugLog("Unable to determine save game folder name from " + dirNameFull);
                }
                DebugLog("RestoreGame complete");
            }
            else
            {
                DebugLog("Unable to parse save game folder name from " + dirNameFull);
            }
        }

        private void buttonRemoveBackup_Click(object sender, EventArgs e)
        {
            if (treeViewBackups.SelectedNode != null)
            {
                string backupName = treeViewBackups.SelectedNode.Text;
                string zipFilePath = backupFolder + Path.DirectorySeparatorChar + backupName;
                if (File.Exists(zipFilePath))
                {
                    DialogResult result = MessageBox.Show("Remove backup file " + backupName + "?", "Remove backup?", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(zipFilePath);
                        GetBackupFiles();
                        DebugLog("RemoveBackup complete");
                    }
                }
            }
            else
            {
                DebugLog("No backup selected to remove!");
            }
        }

        private void treeViewSavegames_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (unselectableSaveNodes.Contains(e.Node))
            {
                e.Cancel = true;
            }
        }

        private void treeViewBackups_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (unselectableBackupNodes.Contains(e.Node))
            {
                e.Cancel = true;
            }
        }

        private void showUI(bool show)
        {
            treeViewSavegames.Enabled = show;
            treeViewBackups.Enabled = show;
            buttonBackup.Enabled = show;
            buttonRestore.Enabled = show;
            buttonRemoveBackup.Enabled = show;
            buttonOpenBackupLocation.Enabled = show;
            buttonRefresh.Enabled = show;
        }

        // https://www.codeproject.com/Questions/852563/How-to-open-file-explorer-at-given-location-in-csh
        private void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe");
                startInfo.Arguments = folderPath;

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }

        private void buttonOpenBackupLocation_Click(object sender, EventArgs e)
        {
            OpenFolder(backupFolder);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshLists();
        }

        private void RefreshLists()
        {
            showUI(false);
            GetBackupFiles();
            GetSaveGames();
            showUI(true);
        }

        private void buttonOpenWorldsLocation_Click(object sender, EventArgs e)
        {
            OpenFolder(saveFolder);
        }
    }

    // https://code.msdn.microsoft.com/windowsdesktop/Ordinal-String-Sorting-1cbac582
    public class OrdinalStringComparer : IComparer<string>
    {
        private bool _ignoreCase = true;

        /// <summary>
        /// Creates an instance of <c>OrdinalStringComparer</c> for case-insensitive string comparison.
        /// </summary>
        public OrdinalStringComparer()
            : this(true)
        {
        }

        /// <summary>
        /// Creates an instance of <c>OrdinalStringComparer</c> for case comparison according to the value specified in input.
        /// </summary>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        public OrdinalStringComparer(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Compares two strings and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare.</param>
        /// <returns>A signed integer that indicates the relative values of x and y, as in the Compare method in the <c>IComparer&lt;T&gt;</c> interface.</returns>
        public int Compare(string x, string y)
        {
            // check for null values first: a null reference is considered to be less than any reference that is not null
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            StringComparison comparisonMode = _ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;

            string[] splitX = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
            string[] splitY = Regex.Split(y.Replace(" ", ""), "([0-9]+)");

            int comparer = 0;

            for (int i = 0; comparer == 0 && i < splitX.Length; i++)
            {
                if (splitY.Length <= i)
                {
                    comparer = 1; // x > y
                }

                int numericX = -1;
                int numericY = -1;
                if (int.TryParse(splitX[i], out numericX))
                {
                    if (int.TryParse(splitY[i], out numericY))
                    {
                        comparer = numericX - numericY;
                    }
                    else
                    {
                        comparer = 1; // x > y
                    }
                }
                else
                {
                    comparer = String.Compare(splitX[i], splitY[i], comparisonMode);
                }
            }

            return comparer;
        }
    }
}
