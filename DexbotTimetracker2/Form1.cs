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
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace DexbotTimetracker2
{

    public partial class Form1 : Form
    {

        private NotifyIcon trayIcon;

        public Form1()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Path.GetFullPath(@"asd.ico")),
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", OnExit)
            }),
                Visible = true
            };

            InitializeComponent();
        }

        private ContextMenu trayMenu;

        protected void Displaynotify()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath(@"asd.ico"));
            notifyIcon1.Text = "Dexbot Desktoptime Tracking Utlity";
            notifyIcon1.ContextMenu = trayMenu;
            notifyIcon1.Visible = true;
        }

        private void OnExit(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }



        //-----------------------------------------------------------
        

    }
}
