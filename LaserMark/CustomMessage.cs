using System;
using System.Configuration;
using System.Windows.Forms;

namespace LaserMark
{
    public partial class CustomMessage : Form
    {
        public CustomMessage()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            config.AppSettings.Settings.Clear();
            config.AppSettings.Settings.Add("Application", "lasterMark");
            config.Save(ConfigurationSaveMode.Minimal);

            InitializeComponent();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}