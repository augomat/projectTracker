namespace ProjectTracker.Timetracker.KIS
{
    partial class DialogKisTasks
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.kisTasksView = new System.Windows.Forms.DataGridView();
            this.closeButton = new System.Windows.Forms.Button();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Hours = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantized = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Project = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.kisTasksView)).BeginInit();
            this.SuspendLayout();
            // 
            // kisTasksView
            // 
            this.kisTasksView.AllowUserToAddRows = false;
            this.kisTasksView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.kisTasksView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.StartTime,
            this.EndTime,
            this.Hours,
            this.Quantized,
            this.Project,
            this.Comment});
            this.kisTasksView.Location = new System.Drawing.Point(12, 12);
            this.kisTasksView.MultiSelect = false;
            this.kisTasksView.Name = "kisTasksView";
            this.kisTasksView.Size = new System.Drawing.Size(1025, 250);
            this.kisTasksView.TabIndex = 3;
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(962, 268);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
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
            // Hours
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gray;
            this.Hours.DefaultCellStyle = dataGridViewCellStyle2;
            this.Hours.HeaderText = "Hours";
            this.Hours.Name = "Hours";
            this.Hours.ReadOnly = true;
            this.Hours.Width = 50;
            // 
            // Quantized
            // 
            this.Quantized.HeaderText = "Quantized";
            this.Quantized.Name = "Quantized";
            this.Quantized.Width = 70;
            // 
            // Project
            // 
            this.Project.HeaderText = "Project";
            this.Project.Name = "Project";
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.Width = 510;
            // 
            // DialogKisTasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(1051, 304);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.kisTasksView);
            this.Name = "DialogKisTasks";
            this.Text = "Show KIS tasks";
            ((System.ComponentModel.ISupportInitialize)(this.kisTasksView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView kisTasksView;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hours;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantized;
        private System.Windows.Forms.DataGridViewTextBoxColumn Project;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}