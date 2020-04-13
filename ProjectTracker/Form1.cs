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
using System.Configuration;
using ProjectTracker.Util;

namespace ProjectTracker
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        enum HotKeyActionId
        {
            ChangeCurrentComment = 0,
            NewProject = 1,
            EditCurrentProject = 2
        }

        public NotifyIcon TrayIcon;

        public Presenter Presenter { get;  set; }
        
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
            maxWorktime.Text = Properties.Settings.Default.maxWorktime;
            finishWTday.Checked = Properties.Settings.Default.flagFinishWTDay;
            autoFinish.Checked = Properties.Settings.Default.flagAutoFinishWT;
            dexbotFilepath.Text = Properties.Settings.Default.DexbotLogFilePath;
            timeularAPIkey.Text = Properties.Settings.Default.timeularAPIkey;
            timeularAPIsecret.Text = Properties.Settings.Default.timeularAPIsecret;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            RegisterHotKey(this.Handle, (int)HotKeyActionId.ChangeCurrentComment, (int)KeyModifier.Alt, Keys.D0.GetHashCode());
            RegisterHotKey(this.Handle, (int)HotKeyActionId.NewProject, (int)KeyModifier.Alt, Keys.D9.GetHashCode());
            RegisterHotKey(this.Handle, (int)HotKeyActionId.EditCurrentProject, (int)KeyModifier.Alt, Keys.D8.GetHashCode());
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void ShowForm(object sender, EventArgs e) 
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
                UnregisterHotKey(this.Handle, 0);
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

            trackbarLabel.Text = String.Format("        {0}\n{1} | {2}", 
                 projectTimes.Item1.ToLongTimeString(), timeSpanBegin.ToString(), timeSpanEnd.ToString());
        }

        private void updateCurrentProject()
        {
            if (Presenter.currentProjectVisible())
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["EndTime"].Value = DateTime.Now.ToLongTimeString();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Mins"].Value = Math.Round((DateTime.Now - Presenter.currentProjectSince).TotalMinutes, 1).ToString();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Comment"].Value = Presenter.currentProjectComment;
            }
        }

        private void projectTrackbarUpdater_Tick(object sender, EventArgs e)
        {
            currentProject.Text = Presenter.currentProject ?? "[not initialized]";

            updateTrackbarLabel();

            updateCurrentProject();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                if (id == (int)HotKeyActionId.ChangeCurrentComment)
                    Presenter.ShowDialogAddComment();
                else if (id == (int)HotKeyActionId.NewProject)
                    Presenter.ShowDialogNewProject();
                else if (id == (int)HotKeyActionId.EditCurrentProject)
                    Presenter.ShowDialogSplitCurrentProject();

            }
        }
    }
}
