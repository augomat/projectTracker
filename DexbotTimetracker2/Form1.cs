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
using System.Configuration;
using ProjectTracker.Util;

namespace ProjectTracker
{
    public partial class Form1 : Form
    {

        public NotifyIcon TrayIcon;

        private Presenter Presenter;

        //NOBODY MUST EVER SEE THIS CODE!!!!
        //I AM DEEPLY SORRY!!!

        
        public Form1()
        {   
            // Initialize Tray Icon
            TrayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Path.GetFullPath(@"asd.ico")),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Show", ShowForm),
                    new MenuItem("-"),
                    new MenuItem("Exit", OnExit)
                }),
                Visible = true,
            };
            TrayIcon.DoubleClick += ShowForm;

            InitializeComponent();

#if DEBUG
            WindowState = FormWindowState.Normal;
#endif

            countAsWorktime.Text = Properties.Settings.Default.countAsWorktimebreakMins.ToString();
            carryOverHours.Text = Properties.Settings.Default.carryOverWorktimeCountHours.ToString();
            
            Presenter = new Presenter(this);
            ProjectChangeHandler mainHandler = new ProjectChangeHandler();
            var worktimebreakHandler = new ProjectChangeProcessorWorktimebreaks(mainHandler);
            var projectCorrectionHandler = new ProjectChangeNotifierCorrection(mainHandler);
            var inMemoryRecordStorage = new WorktimeRecordStorageInMemory();

            //Change notifiers
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierDexpot(mainHandler));
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierLockscreen(mainHandler));
            mainHandler.addProjectChangeNotifier(new ProjectChangeNotifierAppExit(mainHandler));
            mainHandler.addProjectChangeNotifier(projectCorrectionHandler);

            //Change processors
            mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorNewDay(mainHandler));
            mainHandler.addProjectChangeProcessor(worktimebreakHandler);
            //mainHandler.addProjectChangeProcessor(new ProjectChangeProcessorLongerThan10secs(mainHandler));

            //Change subscribers
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberBalloonInformant(Presenter.showNotification));
            mainHandler.addProjectChangeSubscriber(new ProjectChangeSubscriberLogger());

            //Storages
            mainHandler.addWorktimeRecordStorage(new WorktimeRecordStorageCSV());
            mainHandler.addWorktimeRecordStorage(inMemoryRecordStorage);
            mainHandler.RaiseStorageExceptionEvent += new StorageExceptionBalloonInformant(Presenter.showNotification).handleStorageException;
            
            //Presenter
            Presenter.WorktimeAnalyzer = new WorktimeAnalyzer(inMemoryRecordStorage, mainHandler, projectCorrectionHandler);
            Presenter.WorktimebreakHandler = worktimebreakHandler;
            Presenter.ProjectCorrectionHandler = projectCorrectionHandler;
            Presenter.ProjectHandler = mainHandler;
            Presenter.WorktimeRecords = inMemoryRecordStorage.worktimeRecords;
            Presenter.storage = inMemoryRecordStorage;

            //mainHandler.init();

            //RTODO
            TrayIcon.BalloonTipTitle = "Change desktop";
            TrayIcon.BalloonTipText = "Please change your desktop to initialize";
            TrayIcon.ShowBalloonTip(10);
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
            Properties.Settings.Default.lastAppExit = DateTime.Now;
            Properties.Settings.Default.Save();

            TrayIcon.Visible = false;
            TrayIcon.Dispose();
        }

        //----------------------------------

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

        private void worktimebreakUpdater_Tick(object sender, EventArgs e)
        {
            worktimebreakLeft.Text = Presenter.getAvailableWorktimebreak().ToString();
        }

        private void countAsWorktime_Enter(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            int VisibleTime = 5000;  //in milliseconds

            ToolTip tt = new ToolTip();
            tt.Show("Possible Values: 0-60\nChanging this value will result in an immediate recalculation of available worktimebreak for the current project.", TB, 0, TB.Height, VisibleTime);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            const int sideOffset = 8;
            const int dontKnowOffset = 6;
            var posLeftTick = projectTrackBar.Location.X + sideOffset;
            var pxPerTick = (projectTrackBar.Width - 2 * sideOffset) / (projectTrackBar.Maximum - projectTrackBar.Minimum);
            trackbarLabel.Left = posLeftTick - (trackbarLabel.Width / 2) + (projectTrackBar.Value - projectTrackBar.Minimum)*pxPerTick + dontKnowOffset;

            updateTrackbarLabel();
        }

        public float getTrackerbarPercentage()
        {
            return (projectTrackBar.Value - projectTrackBar.Minimum) / (float)projectTrackBar.Maximum;
        }

        private void updateTrackbarLabel()
        {
            var percentage = getTrackerbarPercentage();
            var projectTimes = Presenter.getProjectCorrections(percentage);

            var timeSpanBegin = TimeSpan.FromSeconds((long)(projectTimes.Item1 - Presenter.currentProjectSince).TotalSeconds);
            var timeSpanEnd = TimeSpan.FromSeconds((long)(DateTime.Now - projectTimes.Item1).TotalSeconds);

            trackbarLabel.Text = String.Format("{0} | {1}\n{2} | {3}", 
                projectTimes.Item1.ToLongTimeString(), projectTimes.Item2.ToLongTimeString(), timeSpanBegin.ToString(), timeSpanEnd.ToString());
        }

        private void projectTrackbarUpdater_Tick(object sender, EventArgs e)
        {
            currentProject.Text = Presenter.currentProject ?? "[not initialized]";

            updateTrackbarLabel();
        }
    }
}
