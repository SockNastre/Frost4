using InitFS_Editor.Classes;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace InitFS_Editor
{
    public partial class Settings : Form
    {
        private Ini SettingsIni;

        public Settings(Ini settingsIni)
        {
            InitializeComponent();

            SettingsIni = settingsIni;
            checkBoxUseOriginalDiceKeys.Checked = bool.Parse(SettingsIni.Data["Rebuild Options"]["IsUsingOriginalDICEKeys"]);
            checkBoxAutoOpenInitFS.Checked = bool.Parse(SettingsIni.Data["Tool"]["AutoLoadInitFS"]);
            textBoxAutoLoadInitFSPath.Text = SettingsIni.Data["Tool"]["AutoLoadInitFSPath"];
        }

        private void Settings_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("\"Use original DICE keys\" - When rebuilding initfs the keys that were part of" +
                "the original header will be used on the rebuilt initfs instead of padded zeroes.\n\n" +
                "\"Auto-open InitFS file on startup\" - Automatically opens the selected" +
                "initfs on startup of the tool. Tool must be restarted for this to take effect.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonAutoOpenInitFSBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialogInitFS.ShowDialog() == DialogResult.OK)
            {
                textBoxAutoLoadInitFSPath.Text = openFileDialogInitFS.FileName;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SettingsIni.Data["Rebuild Options"]["IsUsingOriginalDICEKeys"] = checkBoxUseOriginalDiceKeys.Checked.ToString();
            SettingsIni.Data["Tool"]["AutoLoadInitFS"] = checkBoxAutoOpenInitFS.Checked.ToString();
            SettingsIni.Data["Tool"]["AutoLoadInitFSPath"] = textBoxAutoLoadInitFSPath.Text;

            this.Close();
        }
    }
}