using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        private OutlookAppointmentRetriever outlooker;

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
            
            var tracker = new Tracker(trayIcon);
            Thread t = new Thread(tracker.startDesktopLogging);
            t.IsBackground = true;
            t.Start();
      		
      		outlooker = new OutlookAppointmentRetriever(dataGridView1);
        }

        private void OnExit(object sender, EventArgs e)
        {
        	//doesn't really work
            trayIcon.Visible = false;
            //trayIcon.Dispose(); //??
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            outlooker.retrieveAppointments(dateTimePicker1.Value);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           // updateDiffSecs();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            updateDiffSecs();
        }

        //----------------------------------

        private void updateDiffSecs()
        {
            try
            {
                for (var counter = 0; counter < dataGridView1.Rows.Count; counter++)
                {
                    var start = DateTime.Parse(dataGridView1.Rows[counter].Cells["StartTime"].Value.ToString());
                    var end = DateTime.Parse(dataGridView1.Rows[counter].Cells["EndTime"].Value.ToString());

                    dataGridView1.Rows[counter].Cells["DiffSecs"].Value = (end - start).TotalSeconds.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.ToString());
            }
        }

        
    }
}
