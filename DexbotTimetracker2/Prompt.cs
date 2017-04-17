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
    public class Prompt
    {
        private Form prompt;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button AddRowButton;

        private List<TextBox> breaks = new List<TextBox>();
        private List<TextBox> comments = new List<TextBox>();
        private List<ComboBox> projects = new List<ComboBox>();

        private const int lineHeightAdd = 25;
        private int lastLineHeight = 69 - lineHeightAdd;
        private int lastTabIndex = 0;

        private int MinutesBreak;
        private DateTime From;
        private DateTime To;
        private List<WorktimeRecord> Suggestions;

        public List<WorktimeRecord> ShowDialog(DateTime from, DateTime to, List<WorktimeRecord> suggestions = null)
        {
            MinutesBreak = (int)(Math.Floor((to - from).TotalMinutes));
            From = from;
            To = to;
            Suggestions = suggestions;

            prompt = new Form()
            {
                Width = 520,
                Height = 190 - lineHeightAdd,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "What did you do in the mean time?",
                StartPosition = FormStartPosition.CenterScreen,
            };

            label1 = new System.Windows.Forms.Label() { Left = 47, Top = 24, Text = "Break: ", Width = 40 };
            labelM = new System.Windows.Forms.Label() { Left = 91, Top = 24, Text = $"{MinutesBreak} mins" };
            label2 = new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 };
            label3 = new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Comment", Width = 60, Height = 13 };
            label4 = new System.Windows.Forms.Label() { Left = 330, Top = 53, Text = "Project", Width = 50, Height = 13 };
            AddRowButton = new System.Windows.Forms.Button() { Left = 461, Top = 68 - lineHeightAdd, Width = 23, Text = "+" };
            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };

            AddRowButton.Click += AddRowButton_Click;
            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            OkButton.Click += OkButton_Click;
            prompt.AcceptButton = OkButton;

            prompt.Controls.Add(label1);
            prompt.Controls.Add(labelM);
            prompt.Controls.Add(label2);
            prompt.Controls.Add(label3);
            prompt.Controls.Add(label4);
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(AddRowButton);

            if (suggestions != null)
                processSuggestions();
            else
                createRow();

            Task.Delay(500).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //brrrrrrrr hacky, TODO implement something so that it never looses focus (buha)
            Task.Delay(1000).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            Task.Delay(1500).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            Task.Delay(2000).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
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
            prompt.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - prompt.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - prompt.Height) / 2)
            };

            var result = prompt.ShowDialog();

            var ret = new List<WorktimeRecord>();
            if (result == DialogResult.OK)
            {
                DateTime start = from;
                for (int index = 0; index < breaks.Count; index++)
                {
                    var end = start.AddMinutes(Convert.ToInt32(breaks[index].Text));
                    ret.Add(new WorktimeRecord(start, end, projects[index].Text, comments[index].Text));
                    start = end;
                }
                ret.Last().End = to; //to compensate for additional seconds
            }
            return ret;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (getBreakMinutesLeft() != 0)
            {
                MessageBox.Show($"Total Sum of minutes must be exactly {MinutesBreak}", "Error");
                prompt.DialogResult = DialogResult.None;
            }   
        }
        
        private void AddRowButton_Click(object sender, EventArgs e)
        {
            createRow();
        }

        private void createRow()
        {
            lastLineHeight += lineHeightAdd;

            breaks.Add(new System.Windows.Forms.TextBox() { Left = 44, Top = lastLineHeight, Width = 38 });
            comments.Add(new System.Windows.Forms.TextBox() { Left = 91, Top = lastLineHeight, Width = 236 });
            projects.Add(createProjectCombobox());

            breaks.Last().TabIndex = lastTabIndex;
            comments.Last().TabIndex = lastTabIndex + 1;
            projects.Last().TabIndex = lastTabIndex + 2;
            AddRowButton.TabIndex = lastTabIndex + 3;
            OkButton.TabIndex = lastTabIndex + 4;
            lastTabIndex += 3;

            prompt.Controls.Add(breaks.Last());
            prompt.Controls.Add(comments.Last());
            prompt.Controls.Add(projects.Last());

            AddRowButton.Top += lineHeightAdd;
            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;

            breaks.Last().Text = getBreakMinutesLeft().ToString();
            breaks.Last().Validating += Break_Validating;
            breaks.Last().SelectAll();
            breaks.Last().Focus();
        }

        private void Break_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (getBreakMinutesLeft() < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show($"Total Sum of minutes must not exceed {MinutesBreak} minutes", "Error");
                }
                    
            }
            catch
            {
                e.Cancel = true;
                MessageBox.Show($"Please only input integer numbers", "Error");
            }
        }

        private ComboBox createProjectCombobox()
        {
            var combobox = new System.Windows.Forms.ComboBox() { Left = 333, Top = lastLineHeight, Width = 121 };
            combobox.Items.AddRange(ProjectChangeHandler.getAvailableProjects().Cast<string>().ToArray());
            combobox.SelectedIndex = 2; //Hack... We know this must be worktimebreaks because we added them in code...
            return combobox;
        }

        private int getBreakMinutesLeft()
        {
            int sum = 0;
            foreach(var brk in breaks)
            {
                if (brk.Text != "")
                    sum += Convert.ToInt32(brk.Text);
            }
            return MinutesBreak - sum;
        }

        private void processSuggestions()
        {
            var fullList = calculateFullSuggestionList();

            foreach (var wtr in fullList)
            {
                createRow();
                breaks.Last().Text = ((int)Math.Floor((wtr.End - wtr.Start).TotalMinutes)).ToString();
                comments.Last().Text = wtr.Comment;
                projects.Last().SelectedIndex = 3; //Hack... first customer project
            }
        }

        private List<WorktimeRecord> calculateFullSuggestionList()
        {
            DateTime currentEnd = From;
            int currentIndex = 0;

            //TODO sort suggestions

            var ret = new List<WorktimeRecord>();
            while(currentIndex < Suggestions.Count)
            {
                if (Suggestions[currentIndex].End < From)
                    continue;
                if (Suggestions[currentIndex].Start > To)
                    continue;

                if (Suggestions[currentIndex].Start > currentEnd)
                    ret.Add(new WorktimeRecord(currentEnd, Suggestions[currentIndex].Start, "[unknown]", ""));
                ret.Add(Suggestions[currentIndex]);
                currentEnd = Suggestions[currentIndex].End;
                currentIndex++;
            }
            if (currentEnd < To)
                ret.Add(new WorktimeRecord(currentEnd, To, "[unknown]", ""));
            ret.First().Start = From;
            ret.Last().End = To;
            return ret;
        }
    }
}
