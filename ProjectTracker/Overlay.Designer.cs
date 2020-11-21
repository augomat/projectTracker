namespace ProjectTracker
{
    partial class Overlay
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
            this.borderLabel1 = new cSouza.WinForms.Controls.BorderLabel();
            this.SuspendLayout();
            // 
            // borderLabel1
            // 
            this.borderLabel1.BorderSize = 0.25F;
            this.borderLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel1.Location = new System.Drawing.Point(12, 9);
            this.borderLabel1.Name = "borderLabel1";
            this.borderLabel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.borderLabel1.Size = new System.Drawing.Size(282, 33);
            this.borderLabel1.TabIndex = 2;
            this.borderLabel1.Text = "[ProjectName]";
            this.borderLabel1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 51);
            this.Controls.Add(this.borderLabel1);
            this.Name = "Overlay";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion
        private cSouza.WinForms.Controls.BorderLabel borderLabel1;
    }
}