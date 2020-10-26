using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjectTracker
{
    public partial class Overlay : Form
    {
        public Overlay()
        {
            InitializeComponent();
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            this.ControlBox = false;
            this.Text = String.Empty;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            positionOverlay();
        }

        private void positionOverlay()
        {
            Screen mainScreen = null;
            foreach (var singleScreen in Screen.AllScreens) //TODO agh, wäääh, use lambda, linq, whatever for this!!
            {
                if (singleScreen.Primary)
                    mainScreen = singleScreen;
            }
            if (mainScreen == null)
                mainScreen = Screen.FromControl(this);
            Rectangle workingArea = mainScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point()
            {
                X = mainScreen.WorkingArea.Right - this.Width,
                Y = mainScreen.WorkingArea.Bottom - this.Height
            };
        }

        public void setOverlayText(string text)
        {
            borderLabel1.Text = text;
        }
    }
}
