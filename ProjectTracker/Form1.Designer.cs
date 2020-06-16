namespace ProjectTracker
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mins = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Project = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButtonShowKisTasks = new System.Windows.Forms.Button();
            this.ButtonUpdate = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.worktimebreakLeft = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.carryOverHours = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.countAsWorktime = new System.Windows.Forms.TextBox();
            this.worktimebreakUpdater = new System.Windows.Forms.Timer(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CorrectProject = new System.Windows.Forms.Button();
            this.trackbarLabel = new System.Windows.Forms.Label();
            this.currentProject = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.projectTrackBar = new System.Windows.Forms.TrackBar();
            this.correctProjectCombobox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.projectTrackbarUpdater = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flagConsiderOvertime = new System.Windows.Forms.CheckBox();
            this.autoFinish = new System.Windows.Forms.CheckBox();
            this.finishWTday = new System.Windows.Forms.CheckBox();
            this.SetInWorkT = new System.Windows.Forms.Button();
            this.Workbreaktime = new System.Windows.Forms.Label();
            this.ProjectTime = new System.Windows.Forms.Label();
            this.Worktime = new System.Windows.Forms.Label();
            this.PauseTime = new System.Windows.Forms.Label();
            this.PrivateTime = new System.Windows.Forms.Label();
            this.totalTime = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AnalyzeWorktimes = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.currentOvertime = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.maxWorktime = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.dexbotFilepath = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.dexbotStatus = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.timeularAPIsecret = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.timeularAPIkey = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.timeularStatus = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.hotkeySplitCurrentProject = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.hotkeyNewProject = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.hotkeyChangeComment = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.ProjectTimesSummary = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectTrackBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectTimesSummary)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(16, 26);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.StartTime,
            this.EndTime,
            this.Mins,
            this.Project,
            this.Comment,
            this.Index});
            this.dataGridView1.Location = new System.Drawing.Point(16, 56);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(691, 204);
            this.dataGridView1.TabIndex = 2;
            // 
            // Date
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Gray;
            this.Date.DefaultCellStyle = dataGridViewCellStyle1;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 70;
            // 
            // StartTime
            // 
            this.StartTime.HeaderText = "Start Time";
            this.StartTime.Name = "StartTime";
            this.StartTime.Width = 80;
            // 
            // EndTime
            // 
            this.EndTime.HeaderText = "End Time";
            this.EndTime.Name = "EndTime";
            this.EndTime.Width = 80;
            // 
            // Mins
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gray;
            this.Mins.DefaultCellStyle = dataGridViewCellStyle2;
            this.Mins.HeaderText = "Mins";
            this.Mins.Name = "Mins";
            this.Mins.ReadOnly = true;
            this.Mins.Width = 50;
            // 
            // Project
            // 
            this.Project.HeaderText = "Project";
            this.Project.Name = "Project";
            this.Project.Width = 80;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.Width = 270;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ButtonShowKisTasks);
            this.groupBox1.Controls.Add(this.ButtonUpdate);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Location = new System.Drawing.Point(20, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(730, 283);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project history";
            // 
            // ButtonShowKisTasks
            // 
            this.ButtonShowKisTasks.Location = new System.Drawing.Point(529, 27);
            this.ButtonShowKisTasks.Name = "ButtonShowKisTasks";
            this.ButtonShowKisTasks.Size = new System.Drawing.Size(97, 23);
            this.ButtonShowKisTasks.TabIndex = 4;
            this.ButtonShowKisTasks.Text = "Show KIS tasks";
            this.ButtonShowKisTasks.UseVisualStyleBackColor = true;
            // 
            // ButtonUpdate
            // 
            this.ButtonUpdate.Location = new System.Drawing.Point(632, 27);
            this.ButtonUpdate.Name = "ButtonUpdate";
            this.ButtonUpdate.Size = new System.Drawing.Size(75, 23);
            this.ButtonUpdate.TabIndex = 3;
            this.ButtonUpdate.Text = "Update";
            this.ButtonUpdate.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.worktimebreakLeft);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.carryOverHours);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.countAsWorktime);
            this.groupBox3.Location = new System.Drawing.Point(539, 439);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(211, 209);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Worktimebreaks";
            // 
            // worktimebreakLeft
            // 
            this.worktimebreakLeft.AutoSize = true;
            this.worktimebreakLeft.Location = new System.Drawing.Point(26, 176);
            this.worktimebreakLeft.Name = "worktimebreakLeft";
            this.worktimebreakLeft.Size = new System.Drawing.Size(49, 13);
            this.worktimebreakLeft.TabIndex = 5;
            this.worktimebreakLeft.Text = "00:00:00";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Worktimebreak left";
            // 
            // carryOverHours
            // 
            this.carryOverHours.Location = new System.Drawing.Point(10, 120);
            this.carryOverHours.Name = "carryOverHours";
            this.carryOverHours.Size = new System.Drawing.Size(34, 20);
            this.carryOverHours.TabIndex = 3;
            this.carryOverHours.Text = "2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(171, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "Carry over unused Worktimebreak-\r\nminutes from that many past hours";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(197, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Count that many Worktimebreak mins/h \r\nas regular work time";
            // 
            // countAsWorktime
            // 
            this.countAsWorktime.Location = new System.Drawing.Point(10, 54);
            this.countAsWorktime.Name = "countAsWorktime";
            this.countAsWorktime.Size = new System.Drawing.Size(34, 20);
            this.countAsWorktime.TabIndex = 0;
            this.countAsWorktime.Text = "10";
            this.countAsWorktime.Enter += new System.EventHandler(this.countAsWorktime_Enter);
            // 
            // worktimebreakUpdater
            // 
            this.worktimebreakUpdater.Enabled = true;
            this.worktimebreakUpdater.Interval = 1000;
            this.worktimebreakUpdater.Tick += new System.EventHandler(this.worktimebreakUpdater_Tick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.CorrectProject);
            this.groupBox4.Controls.Add(this.trackbarLabel);
            this.groupBox4.Controls.Add(this.currentProject);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.projectTrackBar);
            this.groupBox4.Controls.Add(this.correctProjectCombobox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(20, 313);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(513, 119);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Forgot to switch";
            // 
            // CorrectProject
            // 
            this.CorrectProject.Location = new System.Drawing.Point(424, 72);
            this.CorrectProject.Name = "CorrectProject";
            this.CorrectProject.Size = new System.Drawing.Size(75, 23);
            this.CorrectProject.TabIndex = 8;
            this.CorrectProject.Text = "Correct it!";
            this.CorrectProject.UseVisualStyleBackColor = true;
            // 
            // trackbarLabel
            // 
            this.trackbarLabel.Location = new System.Drawing.Point(58, 77);
            this.trackbarLabel.Name = "trackbarLabel";
            this.trackbarLabel.Size = new System.Drawing.Size(100, 30);
            this.trackbarLabel.TabIndex = 7;
            this.trackbarLabel.Text = "00:00:00 | 00:00:00\r\n00:00:00 | 00:00:00";
            // 
            // currentProject
            // 
            this.currentProject.AutoSize = true;
            this.currentProject.Location = new System.Drawing.Point(16, 48);
            this.currentProject.Name = "currentProject";
            this.currentProject.Size = new System.Drawing.Size(57, 13);
            this.currentProject.TabIndex = 6;
            this.currentProject.Text = "[unknown]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(378, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Correct Project";
            // 
            // projectTrackBar
            // 
            this.projectTrackBar.Location = new System.Drawing.Point(94, 45);
            this.projectTrackBar.Maximum = 50;
            this.projectTrackBar.Name = "projectTrackBar";
            this.projectTrackBar.Size = new System.Drawing.Size(272, 45);
            this.projectTrackBar.TabIndex = 4;
            this.projectTrackBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // correctProjectCombobox
            // 
            this.correctProjectCombobox.FormattingEnabled = true;
            this.correctProjectCombobox.Location = new System.Drawing.Point(381, 44);
            this.correctProjectCombobox.Name = "correctProjectCombobox";
            this.correctProjectCombobox.Size = new System.Drawing.Size(121, 21);
            this.correctProjectCombobox.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Current Project";
            // 
            // projectTrackbarUpdater
            // 
            this.projectTrackbarUpdater.Enabled = true;
            this.projectTrackbarUpdater.Interval = 1000;
            this.projectTrackbarUpdater.Tick += new System.EventHandler(this.projectTrackbarUpdater_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flagConsiderOvertime);
            this.groupBox2.Controls.Add(this.autoFinish);
            this.groupBox2.Controls.Add(this.finishWTday);
            this.groupBox2.Controls.Add(this.SetInWorkT);
            this.groupBox2.Controls.Add(this.Workbreaktime);
            this.groupBox2.Controls.Add(this.ProjectTime);
            this.groupBox2.Controls.Add(this.Worktime);
            this.groupBox2.Controls.Add(this.PauseTime);
            this.groupBox2.Controls.Add(this.PrivateTime);
            this.groupBox2.Controls.Add(this.totalTime);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.AnalyzeWorktimes);
            this.groupBox2.Location = new System.Drawing.Point(20, 439);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 209);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Analysis";
            // 
            // flagConsiderOvertime
            // 
            this.flagConsiderOvertime.AutoSize = true;
            this.flagConsiderOvertime.Checked = true;
            this.flagConsiderOvertime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.flagConsiderOvertime.Location = new System.Drawing.Point(385, 20);
            this.flagConsiderOvertime.Name = "flagConsiderOvertime";
            this.flagConsiderOvertime.Size = new System.Drawing.Size(111, 17);
            this.flagConsiderOvertime.TabIndex = 19;
            this.flagConsiderOvertime.Text = "consider Overtime";
            this.flagConsiderOvertime.UseVisualStyleBackColor = true;
            // 
            // autoFinish
            // 
            this.autoFinish.AutoSize = true;
            this.autoFinish.Checked = true;
            this.autoFinish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoFinish.Location = new System.Drawing.Point(263, 20);
            this.autoFinish.Name = "autoFinish";
            this.autoFinish.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.autoFinish.Size = new System.Drawing.Size(122, 17);
            this.autoFinish.TabIndex = 18;
            this.autoFinish.Text = "auto-update last day";
            this.autoFinish.UseVisualStyleBackColor = true;
            // 
            // finishWTday
            // 
            this.finishWTday.AutoSize = true;
            this.finishWTday.Checked = true;
            this.finishWTday.CheckState = System.Windows.Forms.CheckState.Checked;
            this.finishWTday.Location = new System.Drawing.Point(160, 20);
            this.finishWTday.Name = "finishWTday";
            this.finishWTday.Size = new System.Drawing.Size(102, 17);
            this.finishWTday.TabIndex = 17;
            this.finishWTday.Text = "finish day in WT";
            this.finishWTday.UseVisualStyleBackColor = true;
            // 
            // SetInWorkT
            // 
            this.SetInWorkT.Location = new System.Drawing.Point(74, 20);
            this.SetInWorkT.Name = "SetInWorkT";
            this.SetInWorkT.Size = new System.Drawing.Size(78, 23);
            this.SetInWorkT.TabIndex = 16;
            this.SetInWorkT.Text = "Log projects";
            this.SetInWorkT.UseVisualStyleBackColor = true;
            // 
            // Workbreaktime
            // 
            this.Workbreaktime.AutoSize = true;
            this.Workbreaktime.Location = new System.Drawing.Point(113, 177);
            this.Workbreaktime.Name = "Workbreaktime";
            this.Workbreaktime.Size = new System.Drawing.Size(90, 13);
            this.Workbreaktime.TabIndex = 15;
            this.Workbreaktime.Text = "[not yet analyzed]";
            // 
            // ProjectTime
            // 
            this.ProjectTime.AutoSize = true;
            this.ProjectTime.Location = new System.Drawing.Point(113, 152);
            this.ProjectTime.Name = "ProjectTime";
            this.ProjectTime.Size = new System.Drawing.Size(90, 13);
            this.ProjectTime.TabIndex = 14;
            this.ProjectTime.Text = "[not yet analyzed]";
            // 
            // Worktime
            // 
            this.Worktime.AutoSize = true;
            this.Worktime.Location = new System.Drawing.Point(101, 127);
            this.Worktime.Name = "Worktime";
            this.Worktime.Size = new System.Drawing.Size(90, 13);
            this.Worktime.TabIndex = 13;
            this.Worktime.Text = "[not yet analyzed]";
            // 
            // PauseTime
            // 
            this.PauseTime.AutoSize = true;
            this.PauseTime.Location = new System.Drawing.Point(101, 102);
            this.PauseTime.Name = "PauseTime";
            this.PauseTime.Size = new System.Drawing.Size(90, 13);
            this.PauseTime.TabIndex = 12;
            this.PauseTime.Text = "[not yet analyzed]";
            // 
            // PrivateTime
            // 
            this.PrivateTime.AutoSize = true;
            this.PrivateTime.Location = new System.Drawing.Point(101, 77);
            this.PrivateTime.Name = "PrivateTime";
            this.PrivateTime.Size = new System.Drawing.Size(90, 13);
            this.PrivateTime.TabIndex = 11;
            this.PrivateTime.Text = "[not yet analyzed]";
            // 
            // totalTime
            // 
            this.totalTime.AutoSize = true;
            this.totalTime.Location = new System.Drawing.Point(88, 52);
            this.totalTime.Name = "totalTime";
            this.totalTime.Size = new System.Drawing.Size(90, 13);
            this.totalTime.TabIndex = 10;
            this.totalTime.Text = "[not yet analyzed]";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Workbreak:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(34, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Projects:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(46, 127);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Work:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(44, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Privat:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Pause:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Total time:";
            // 
            // AnalyzeWorktimes
            // 
            this.AnalyzeWorktimes.Location = new System.Drawing.Point(7, 20);
            this.AnalyzeWorktimes.Name = "AnalyzeWorktimes";
            this.AnalyzeWorktimes.Size = new System.Drawing.Size(65, 23);
            this.AnalyzeWorktimes.TabIndex = 0;
            this.AnalyzeWorktimes.Text = "Analyze";
            this.AnalyzeWorktimes.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.currentOvertime);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.maxWorktime);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Location = new System.Drawing.Point(540, 313);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(210, 119);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Max Worktime";
            // 
            // currentOvertime
            // 
            this.currentOvertime.AutoSize = true;
            this.currentOvertime.Location = new System.Drawing.Point(25, 92);
            this.currentOvertime.Name = "currentOvertime";
            this.currentOvertime.Size = new System.Drawing.Size(49, 13);
            this.currentOvertime.TabIndex = 6;
            this.currentOvertime.Text = "00:00:00";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 72);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(135, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "Overtime (according to DB)";
            // 
            // maxWorktime
            // 
            this.maxWorktime.Location = new System.Drawing.Point(15, 38);
            this.maxWorktime.Name = "maxWorktime";
            this.maxWorktime.Size = new System.Drawing.Size(43, 20);
            this.maxWorktime.TabIndex = 6;
            this.maxWorktime.Text = "07:38";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(157, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Maximum permitted time per day";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dexbotFilepath);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.dexbotStatus);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Location = new System.Drawing.Point(20, 655);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(353, 51);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Dexbot";
            // 
            // dexbotFilepath
            // 
            this.dexbotFilepath.Location = new System.Drawing.Point(169, 17);
            this.dexbotFilepath.Name = "dexbotFilepath";
            this.dexbotFilepath.Size = new System.Drawing.Size(178, 20);
            this.dexbotFilepath.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(109, 21);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Path logfile";
            // 
            // dexbotStatus
            // 
            this.dexbotStatus.AutoSize = true;
            this.dexbotStatus.BackColor = System.Drawing.Color.Red;
            this.dexbotStatus.Location = new System.Drawing.Point(46, 21);
            this.dexbotStatus.Name = "dexbotStatus";
            this.dexbotStatus.Size = new System.Drawing.Size(46, 13);
            this.dexbotStatus.TabIndex = 1;
            this.dexbotStatus.Text = "disabled";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 21);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Status";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.timeularAPIsecret);
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.timeularAPIkey);
            this.groupBox7.Controls.Add(this.label19);
            this.groupBox7.Controls.Add(this.timeularStatus);
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Location = new System.Drawing.Point(379, 655);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(371, 51);
            this.groupBox7.TabIndex = 11;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Timeular";
            // 
            // timeularAPIsecret
            // 
            this.timeularAPIsecret.Location = new System.Drawing.Point(285, 16);
            this.timeularAPIsecret.Name = "timeularAPIsecret";
            this.timeularAPIsecret.Size = new System.Drawing.Size(75, 20);
            this.timeularAPIsecret.TabIndex = 7;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(227, 20);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(56, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "API secret";
            // 
            // timeularAPIkey
            // 
            this.timeularAPIkey.Location = new System.Drawing.Point(144, 16);
            this.timeularAPIkey.Name = "timeularAPIkey";
            this.timeularAPIkey.Size = new System.Drawing.Size(75, 20);
            this.timeularAPIkey.TabIndex = 5;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(96, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(44, 13);
            this.label19.TabIndex = 4;
            this.label19.Text = "API key";
            // 
            // timeularStatus
            // 
            this.timeularStatus.AutoSize = true;
            this.timeularStatus.BackColor = System.Drawing.Color.Red;
            this.timeularStatus.Location = new System.Drawing.Point(46, 20);
            this.timeularStatus.Name = "timeularStatus";
            this.timeularStatus.Size = new System.Drawing.Size(46, 13);
            this.timeularStatus.TabIndex = 3;
            this.timeularStatus.Text = "disabled";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "Status";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.hotkeySplitCurrentProject);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.hotkeyNewProject);
            this.groupBox8.Controls.Add(this.label17);
            this.groupBox8.Controls.Add(this.hotkeyChangeComment);
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Location = new System.Drawing.Point(20, 712);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(730, 51);
            this.groupBox8.TabIndex = 12;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Hotkeys";
            // 
            // hotkeySplitCurrentProject
            // 
            this.hotkeySplitCurrentProject.AutoSize = true;
            this.hotkeySplitCurrentProject.BackColor = System.Drawing.Color.Red;
            this.hotkeySplitCurrentProject.Location = new System.Drawing.Point(375, 21);
            this.hotkeySplitCurrentProject.Name = "hotkeySplitCurrentProject";
            this.hotkeySplitCurrentProject.Size = new System.Drawing.Size(46, 13);
            this.hotkeySplitCurrentProject.TabIndex = 5;
            this.hotkeySplitCurrentProject.Text = "disabled";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(274, 21);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(98, 13);
            this.label22.TabIndex = 4;
            this.label22.Text = "Split current project";
            // 
            // hotkeyNewProject
            // 
            this.hotkeyNewProject.AutoSize = true;
            this.hotkeyNewProject.BackColor = System.Drawing.Color.Red;
            this.hotkeyNewProject.Location = new System.Drawing.Point(220, 21);
            this.hotkeyNewProject.Name = "hotkeyNewProject";
            this.hotkeyNewProject.Size = new System.Drawing.Size(46, 13);
            this.hotkeyNewProject.TabIndex = 3;
            this.hotkeyNewProject.Text = "disabled";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(152, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(64, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "New project";
            // 
            // hotkeyChangeComment
            // 
            this.hotkeyChangeComment.AutoSize = true;
            this.hotkeyChangeComment.BackColor = System.Drawing.Color.Red;
            this.hotkeyChangeComment.Location = new System.Drawing.Point(102, 21);
            this.hotkeyChangeComment.Name = "hotkeyChangeComment";
            this.hotkeyChangeComment.Size = new System.Drawing.Size(46, 13);
            this.hotkeyChangeComment.TabIndex = 1;
            this.hotkeyChangeComment.Text = "disabled";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 21);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(90, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Change comment";
            // 
            // ProjectTimesSummary
            // 
            this.ProjectTimesSummary.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.Name = "ChartArea1";
            this.ProjectTimesSummary.ChartAreas.Add(chartArea1);
            this.ProjectTimesSummary.Location = new System.Drawing.Point(210, 480);
            this.ProjectTimesSummary.Name = "ProjectTimesSummary";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.ProjectTimesSummary.Series.Add(series1);
            this.ProjectTimesSummary.Size = new System.Drawing.Size(320, 162);
            this.ProjectTimesSummary.TabIndex = 15;
            this.ProjectTimesSummary.Text = "chart1";
            this.ProjectTimesSummary.Visible = false;
            this.ProjectTimesSummary.Click += new System.EventHandler(this.ProjectTimesSummary_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 775);
            this.Controls.Add(this.ProjectTimesSummary);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Project Tracker";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectTrackBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectTimesSummary)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.DateTimePicker dateTimePicker1;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox countAsWorktime;
        public System.Windows.Forms.TextBox carryOverHours;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label worktimebreakLeft;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer worktimebreakUpdater;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.ComboBox correctProjectCombobox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar projectTrackBar;
        private System.Windows.Forms.Label currentProject;
        private System.Windows.Forms.Label trackbarLabel;
        private System.Windows.Forms.Timer projectTrackbarUpdater;
        public System.Windows.Forms.Button CorrectProject;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Button AnalyzeWorktimes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label Workbreaktime;
        public System.Windows.Forms.Label ProjectTime;
        public System.Windows.Forms.Label Worktime;
        public System.Windows.Forms.Label PauseTime;
        public System.Windows.Forms.Label PrivateTime;
        public System.Windows.Forms.Label totalTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.Button ButtonUpdate;
        public System.Windows.Forms.Button SetInWorkT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mins;
        private System.Windows.Forms.DataGridViewTextBoxColumn Project;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        public System.Windows.Forms.CheckBox finishWTday;
        public System.Windows.Forms.CheckBox autoFinish;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.Label currentOvertime;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox maxWorktime;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.CheckBox flagConsiderOvertime;
        private System.Windows.Forms.GroupBox groupBox6;
        public System.Windows.Forms.TextBox dexbotFilepath;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.Label dexbotStatus;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox7;
        public System.Windows.Forms.TextBox timeularAPIsecret;
        private System.Windows.Forms.Label label20;
        public System.Windows.Forms.TextBox timeularAPIkey;
        private System.Windows.Forms.Label label19;
        public System.Windows.Forms.Label timeularStatus;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.Button ButtonShowKisTasks;
        private System.Windows.Forms.GroupBox groupBox8;
        public System.Windows.Forms.Label hotkeySplitCurrentProject;
        private System.Windows.Forms.Label label22;
        public System.Windows.Forms.Label hotkeyNewProject;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label hotkeyChangeComment;
        private System.Windows.Forms.Label label21;
        public System.Windows.Forms.DataVisualization.Charting.Chart ProjectTimesSummary;
    }
}

