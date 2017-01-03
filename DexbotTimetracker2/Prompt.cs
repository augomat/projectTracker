using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ProjectTracker
{
    public static class Prompt
    {
        public static Tuple<string, string> ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
            };
            var comboBox1 = new System.Windows.Forms.ComboBox() { DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Left = 50, Top = 70, AutoSize = true, TabIndex = 1 };
            //this.comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(ProjectChangeHandler.getAvailableProjects().Cast<string>().ToArray());
            comboBox1.SelectedIndex = 2;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400, TabIndex = 0 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, TabIndex = 3, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };
            CheckBox noBreak = new CheckBox() { Left = 50, Top = 70, AutoSize = true, TabIndex = 1, Text = "No break (use for meetings etc)" };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            //prompt.Controls.Add(noBreak);
            prompt.Controls.Add(comboBox1);
            prompt.AcceptButton = confirmation;

            Task.Delay(500).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //brrrrrrrr hacky, TODO implement something so that it never looses focus (buha)
            Task.Delay(1000).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            prompt.TopMost = true;

            //center it on main screen
            Screen mainScreen = null;
            foreach (var singleScreen in Screen.AllScreens) //TODO agh, wäääh, use lambda, linq, whatever for this!!
            {
                if (singleScreen.Primary)
                    mainScreen = singleScreen;
            }
            if (mainScreen == null)
                mainScreen = Screen.FromControl(prompt);

            Rectangle workingArea = mainScreen.WorkingArea;
            prompt.StartPosition = FormStartPosition.Manual;
            //prompt.Location = new Point(10, 10);
            prompt.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - prompt.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - prompt.Height) / 2)
            };

            var result = prompt.ShowDialog();

            if (result == DialogResult.OK)
                return new Tuple<String, String>(textBox.Text, comboBox1.Text);
            else
                return new Tuple<String, String>("", "");
        }
    }
}
