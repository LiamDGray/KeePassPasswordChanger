using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace KeePassPasswordChanger
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            labelVersion.Text = FileVersionInfo.GetVersionInfo("KeePassPasswordChanger.dll").ProductVersion;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel) sender).Text);
        }
    }
}
