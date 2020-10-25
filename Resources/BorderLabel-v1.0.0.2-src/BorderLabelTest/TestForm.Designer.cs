/***********************************************************************
 * BorderLabel.cs - Simple Label Control with Border Effect            *
 *                                                                     *
 *   Author:      César Roberto de Souza                               *
 *   Email:       cesarsouza at gmail.com                              *
 *   Website:     http://www.comp.ufscar.br/~cesarsouza                *
 *                                                                     *      
 *  This code is distributed under the The Code Project Open License   *
 *  (CPOL) 1.02 or any later versions of this same license. By using   *
 *  this code you agree not to remove any of the original copyright,   *
 *  patent, trademark, and attribution notices and associated          *
 *  disclaimers that may appear in the Source Code or Executable Files *
 *                                                                     *
 *  The exact terms of this license can be found on The Code Project   *
 *   website: http://www.codeproject.com/info/cpol10.aspx              *
 *                                                                     *
 ***********************************************************************/

namespace BorderLabelTest
{
    partial class TestForm
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
            this.borderLabel7 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel1 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel5 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel2 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel3 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel6 = new cSouza.WinForms.Controls.BorderLabel();
            this.borderLabel4 = new cSouza.WinForms.Controls.BorderLabel();
            this.SuspendLayout();
            // 
            // borderLabel7
            // 
            this.borderLabel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.borderLabel7.AutoSize = true;
            this.borderLabel7.BorderColor = System.Drawing.Color.DarkGoldenrod;
            this.borderLabel7.BorderSize = 1.8F;
            this.borderLabel7.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel7.ForeColor = System.Drawing.Color.Goldenrod;
            this.borderLabel7.Location = new System.Drawing.Point(84, 217);
            this.borderLabel7.Name = "borderLabel7";
            this.borderLabel7.Size = new System.Drawing.Size(510, 58);
            this.borderLabel7.TabIndex = 0;
            this.borderLabel7.Text = "BorderLabel Testing";
            this.borderLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // borderLabel1
            // 
            this.borderLabel1.AutoSize = true;
            this.borderLabel1.BorderColor = System.Drawing.Color.Black;
            this.borderLabel1.BorderSize = 2F;
            this.borderLabel1.Font = new System.Drawing.Font("Cooper Black", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel1.ForeColor = System.Drawing.Color.White;
            this.borderLabel1.Location = new System.Drawing.Point(12, -3);
            this.borderLabel1.Name = "borderLabel1";
            this.borderLabel1.Size = new System.Drawing.Size(536, 91);
            this.borderLabel1.TabIndex = 0;
            this.borderLabel1.Text = "CodeProject";
            // 
            // borderLabel5
            // 
            this.borderLabel5.BackColor = System.Drawing.Color.Black;
            this.borderLabel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.borderLabel5.Font = new System.Drawing.Font("Courier New", 38.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel5.ForeColor = System.Drawing.Color.Black;
            this.borderLabel5.Location = new System.Drawing.Point(0, 341);
            this.borderLabel5.Name = "borderLabel5";
            this.borderLabel5.Size = new System.Drawing.Size(678, 56);
            this.borderLabel5.TabIndex = 0;
            this.borderLabel5.Text = "abcdefghijklmnopqrs";
            this.borderLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // borderLabel2
            // 
            this.borderLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.borderLabel2.AutoSize = true;
            this.borderLabel2.BorderColor = System.Drawing.Color.Black;
            this.borderLabel2.Font = new System.Drawing.Font("Courier New", 38.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel2.ForeColor = System.Drawing.Color.White;
            this.borderLabel2.Location = new System.Drawing.Point(48, 277);
            this.borderLabel2.Name = "borderLabel2";
            this.borderLabel2.Size = new System.Drawing.Size(614, 57);
            this.borderLabel2.TabIndex = 0;
            this.borderLabel2.Text = "abcdefghijklmnopqrs";
            this.borderLabel2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // borderLabel3
            // 
            this.borderLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.borderLabel3.AutoSize = true;
            this.borderLabel3.BorderSize = 1.3F;
            this.borderLabel3.Font = new System.Drawing.Font("Lucida Handwriting", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel3.ForeColor = System.Drawing.Color.Black;
            this.borderLabel3.Location = new System.Drawing.Point(372, 89);
            this.borderLabel3.Name = "borderLabel3";
            this.borderLabel3.Size = new System.Drawing.Size(305, 56);
            this.borderLabel3.TabIndex = 0;
            this.borderLabel3.Text = "CodeProject";
            // 
            // borderLabel6
            // 
            this.borderLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.borderLabel6.AutoSize = true;
            this.borderLabel6.BorderColor = System.Drawing.Color.Maroon;
            this.borderLabel6.BorderSize = 1.7F;
            this.borderLabel6.Font = new System.Drawing.Font("Matura MT Script Capitals", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel6.ForeColor = System.Drawing.Color.LightCoral;
            this.borderLabel6.Location = new System.Drawing.Point(310, 140);
            this.borderLabel6.Name = "borderLabel6";
            this.borderLabel6.Size = new System.Drawing.Size(393, 85);
            this.borderLabel6.TabIndex = 0;
            this.borderLabel6.Text = "CodeProject";
            this.borderLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // borderLabel4
            // 
            this.borderLabel4.AutoSize = true;
            this.borderLabel4.BorderColor = System.Drawing.Color.LightGray;
            this.borderLabel4.Font = new System.Drawing.Font("Tempus Sans ITC", 42F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.borderLabel4.ForeColor = System.Drawing.Color.Gray;
            this.borderLabel4.Location = new System.Drawing.Point(7, 108);
            this.borderLabel4.Name = "borderLabel4";
            this.borderLabel4.Size = new System.Drawing.Size(333, 73);
            this.borderLabel4.TabIndex = 0;
            this.borderLabel4.Text = "CodeProject";
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(678, 397);
            this.Controls.Add(this.borderLabel7);
            this.Controls.Add(this.borderLabel1);
            this.Controls.Add(this.borderLabel5);
            this.Controls.Add(this.borderLabel2);
            this.Controls.Add(this.borderLabel3);
            this.Controls.Add(this.borderLabel6);
            this.Controls.Add(this.borderLabel4);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(686, 424);
            this.Name = "TestForm";
            this.Text = "BorderLabel Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private cSouza.WinForms.Controls.BorderLabel borderLabel1;
        private cSouza.WinForms.Controls.BorderLabel borderLabel2;
        private cSouza.WinForms.Controls.BorderLabel borderLabel3;
        private cSouza.WinForms.Controls.BorderLabel borderLabel4;
        private cSouza.WinForms.Controls.BorderLabel borderLabel5;
        private cSouza.WinForms.Controls.BorderLabel borderLabel6;
        private cSouza.WinForms.Controls.BorderLabel borderLabel7;


    }
}

