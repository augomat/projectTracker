﻿using System;
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
        private Tracker tracker;

        //NOBODY MUST EVER SEE THIS CODE!!!!
        //I AM DEEPLY SORRY!!!

        public Form1()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Path.GetFullPath(@"asd.ico")),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Show", ShowForm),
                    new MenuItem("-"),
                    new MenuItem("Exit", OnExit)
                }),
                Visible = true,
            };
            trayIcon.DoubleClick += ShowForm;

            InitializeComponent();

            comboBox1.SelectedIndex = 1;

            tracker = new Tracker(trayIcon);
            Thread t = new Thread(tracker.startDesktopLogging);
            t.IsBackground = true;
            t.Start();
      		
      		outlooker = new OutlookAppointmentRetriever(dataGridView1);

            trayIcon.BalloonTipTitle = "Change desktop";
            trayIcon.BalloonTipText = "Please change your desktop to initialize";
            trayIcon.ShowBalloonTip(10);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ShowForm(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.Focus();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            tracker.recordAppExit();

            trayIcon.Visible = false;
            trayIcon.Dispose();
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var isSane = CheckGridSanity();

                if (isSane)
                    WriteGridToCSV();

                MessageBox.Show("Written to CSV");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.ToString());
            }
        }

        private bool CheckGridSanity()
        {
            for (var counter = 0; counter < dataGridView1.Rows.Count; counter++)
            {
                var row = dataGridView1.Rows[counter];

                if (row.Cells["DesktopNo"].Value == null)
                {
                    MessageBox.Show("DesktopNo of row " + (counter + 1) + " is not valid");
                    return false;
                }
            }
            return true;
        }

        private void WriteGridToCSV()
        {
            for (var counter = 0; counter < dataGridView1.Rows.Count; counter++)
            {
                var row = dataGridView1.Rows[counter];

                var rdiffSecs = Convert.ToInt64(row.Cells["DiffSecs"].Value);
                var rdesktopNo = row.Cells["DesktopNo"].Value.ToString();
                var rstartDate = DateTime.ParseExact(
                        row.Cells["Date"].Value.ToString() + " " + row.Cells["StartTime"].Value.ToString(),
                        "dd.MM.yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                var rendDate = DateTime.ParseExact(
                        row.Cells["Date"].Value.ToString() + " " + row.Cells["EndTime"].Value.ToString(),
                        "dd.MM.yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                var rcomment = row.Cells["Comment"].Value.ToString();

                tracker.writeCSVEntry(
                    rdiffSecs,
                    rdesktopNo,
                    rstartDate,
                    rendDate,
                    rcomment
                );
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("No valid DesktopNo given");
                return;
            }

            try
            {
                int time = (textBox2.Text != "") ? Convert.ToInt32(textBox2.Text) : 0; 
                tracker.doFakeSwitch(textBox1.Text, time);
                MessageBox.Show("Desktop corrected. It will be logged correctly whenever you switch Desktop the next time");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.ToString());
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("This will close the whole application. Confirm?", "Close Application", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
