using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HddTaskMonitor
{
    public partial class Form1 : Form
    {
        private Logic _logic;

        public Form1()
        {
            InitializeComponent();
            // TODO: don't hardcode
            _logic = new Logic("c", Color.White, Color.Red);

            Update();
            notifyIcon1.Visible = true;
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            // doesn't work properly
            //contextMenuStrip1.Show(MousePosition);
        }

        private void Update()
        {
            (Icon icon, string tooltip) = _logic.GetHddInfo();
            notifyIcon1.Icon = icon;
            notifyIcon1.Text = tooltip;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Update();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // we might want to do something to prevent multiple settings from showing up
            var settings = new Settings();
            settings.Show();
        }
    }
}
