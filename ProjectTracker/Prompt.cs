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
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button AddRowButton;

        private List<TextBox> breaks = new List<TextBox>();
        private List<TextBox> comments = new List<TextBox>();
        private List<ComboBox> projects = new List<ComboBox>();

        private Label labelNow;
        private TextBox currentComment;
        private Label labelProject;
        private ComboBox newProject;

        private const int lineHeightAdd = 25;
        private int lastLineHeight = 69 - lineHeightAdd;
        private int nextTabIndex = 0;

        private int MinutesBreak;
        private DateTime From;
        private DateTime To;
        private List<WorktimeRecord> Suggestions;
        private IProjectHandler Handler;

        public List<WorktimeRecord> ShowDialogMeantime(DateTime from, DateTime to, ProjectChangeHandler handler, List<WorktimeRecord> suggestions = null)
        {
            MinutesBreak = (int)(Math.Floor((to - from).TotalMinutes));
            From = from;
            To = to;
            Suggestions = suggestions;
            Handler = handler;

            generateForm("What did you do in the mean time?");

            label1 = new System.Windows.Forms.Label() { Left = 47, Top = 24, Text = "Break: ", Width = 40 };
            labelM = new System.Windows.Forms.Label() { Left = 91, Top = 24, Text = $"{MinutesBreak} mins" };
            label2 = new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 };
            label3 = new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 };
            label4 = new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 };
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
            prompt.Controls.Add(CancelButton);
            prompt.Controls.Add(AddRowButton);

            if (suggestions != null)
                processSuggestions();
            else
                createRow();

            createRowCurrentProject();

            continuallyFocusDialog();
            centerDialogOnMainscreen();
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

                Handler.currentProjectComment = currentComment.Text; //mmmh...not very convinced by this design
            }
            return ret;
        }

        public void ShowDialogChangeCurrentComment(IProjectHandler handler)
        {
            Handler = handler;

            generateForm("Comment your current project");

            label2 = new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 };
            label3 = new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 };
            label4 = new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 };
            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };
            CancelButton = new System.Windows.Forms.Button() { Left = 327, Top = 112 - lineHeightAdd, Width = 75, Text = "Cancel" };

            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            CancelButton.DialogResult = DialogResult.Cancel;
            CancelButton.Click += (sender, e) => { prompt.Close(); };
            prompt.AcceptButton = OkButton;
            prompt.CancelButton = CancelButton;

            prompt.Controls.Add(label2);
            prompt.Controls.Add(label3);
            prompt.Controls.Add(label4);
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);

            createRowCurrentProject();

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            if (result == DialogResult.OK)
            {
                Handler.currentProjectComment = currentComment.Text; //mmmh...not very convinced by this design
            }
        }

        public WorktimeRecord ShowDialogNewProject(IProjectHandler handler)
        {
            Handler = handler;

            generateForm("Create a new project from now on");

            label2 = new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 };
            label3 = new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 };
            label4 = new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 };
            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };
            CancelButton = new System.Windows.Forms.Button() { Left = 327, Top = 112 - lineHeightAdd, Width = 75, Text = "Cancel" };

            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            CancelButton.DialogResult = DialogResult.Cancel;
            CancelButton.Click += (sender, e) => { prompt.Close(); };
            prompt.AcceptButton = OkButton;
            prompt.CancelButton = CancelButton;

            prompt.Controls.Add(label2);
            prompt.Controls.Add(label3);
            prompt.Controls.Add(label4);
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);

            createRowNewProject();

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            if (result == DialogResult.OK)
                return new WorktimeRecord(Handler.currentProjectSince, DateTime.Now, newProject.Text, currentComment.Text);
            else
                return null;
        }

        private void generateForm(string message)
        {
            prompt = new Form()
            {
                Width = 520,
                Height = 190 - lineHeightAdd,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = message,
                StartPosition = FormStartPosition.CenterScreen,
            };
        }

        private void centerDialogOnMainscreen()
        {
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
        }

        private void continuallyFocusDialog()
        {
            Task.Delay(500).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //brrrrrrrr hacky, TODO implement something so that it never looses focus (buha)
            Task.Delay(1000).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            Task.Delay(1500).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            Task.Delay(2000).ContinueWith(t => { try { prompt.Invoke(new Action(prompt.Activate)); } catch { } }); //BRRRRRRRRRRRRR
            prompt.TopMost = true;
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
            //Not exactly beautiful solution but it works
            removeRowCurrentProject();
            createRow();
            createRowCurrentProject();
        }

        private void createRow()
        {
            lastLineHeight += lineHeightAdd;

            breaks.Add(new System.Windows.Forms.TextBox() { Left = 44, Top = lastLineHeight, Width = 38 });
            comments.Add(new System.Windows.Forms.TextBox() { Left = 221, Top = lastLineHeight, Width = 236 });
            projects.Add(createProjectCombobox());

            breaks.Last().TabIndex = nextTabIndex;
            projects.Last().TabIndex = nextTabIndex + 1;
            comments.Last().TabIndex = nextTabIndex + 2;
            nextTabIndex += 3;
            AddRowButton.TabIndex = nextTabIndex;
            OkButton.TabIndex = nextTabIndex + 1;

            prompt.Controls.Add(breaks.Last());
            prompt.Controls.Add(comments.Last());
            prompt.Controls.Add(projects.Last());

            AddRowButton.Top += lineHeightAdd;
            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;

            breaks.Last().Text = getBreakMinutesLeft().ToString();
            //breaks.Last().Validating += Break_Validating;
            breaks.Last().SelectAll();
            breaks.Last().Focus();
        }

        private void createRowCurrentProject()
        {
            lastLineHeight += lineHeightAdd;

            labelNow = new System.Windows.Forms.Label() { Left = 44, Top = lastLineHeight, Width = 38, Text = "Now" };
            currentComment = new System.Windows.Forms.TextBox() { Left = 221, Top = lastLineHeight, Width = 236, Text = currentComment == null ? Handler.currentProjectComment : currentComment.Text }; //Handler.currentProjectComment is actually wrong and a big hack! we rely on the lockscreenNotifier to not change the project! //we should actually use values from the original event here
            labelProject = new System.Windows.Forms.Label() { Left = 91, Top = lastLineHeight, Width = 121, Text = Handler.currentProject };

            if (AddRowButton != null)
            {
                AddRowButton.TabIndex = nextTabIndex;
                nextTabIndex += 1;
            }
            currentComment.TabIndex = nextTabIndex;
            OkButton.TabIndex = nextTabIndex + 1;

            prompt.Controls.Add(labelNow);
            prompt.Controls.Add(currentComment);
            prompt.Controls.Add(labelProject);

            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;
        }

        private void removeRowCurrentProject()
        {
            prompt.Controls.Remove(labelNow);
            prompt.Controls.Remove(currentComment);
            prompt.Controls.Remove(labelProject);
            nextTabIndex -= 1;

            OkButton.Top -= lineHeightAdd;
            prompt.Height -= lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;

            lastLineHeight -= lineHeightAdd;
        }

        private void createRowNewProject()
        {
            lastLineHeight += lineHeightAdd;

            labelNow = new System.Windows.Forms.Label() { Left = 44, Top = lastLineHeight, Width = 38, Text = "Now" };
            currentComment = new System.Windows.Forms.TextBox() { Left = 221, Top = lastLineHeight, Width = 236, Text = "" };
            newProject = createProjectCombobox();
            newProject.SelectedIndex = -1;
            newProject.Text = Handler.currentProject;

            currentComment.TabIndex = nextTabIndex;
            newProject.TabIndex = nextTabIndex + 1;
            OkButton.TabIndex = nextTabIndex + 2;

            prompt.Controls.Add(labelNow);
            prompt.Controls.Add(currentComment);
            prompt.Controls.Add(newProject);

            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;
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
            var combobox = new System.Windows.Forms.ComboBox() { Left = 91, Top = lastLineHeight, Width = 121 };
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

            //Hacky the hack, adjust last value if we floored too much
            var breakLeft = getBreakMinutesLeft();
            breaks.Last().Text = (Convert.ToInt32(breaks.Last().Text) + breakLeft).ToString();
        }

        private List<WorktimeRecord> calculateFullSuggestionList()
        {
            DateTime currentEnd = From;
            int currentIndex = 0;

            Suggestions.Sort((x, y) => x.Start.CompareTo(y.Start));

            var ret = new List<WorktimeRecord>();
            while(currentIndex < Suggestions.Count)
            {
                var currentSugg = Suggestions[currentIndex];

                //Discard all suggestions not between From and To
                if (currentSugg.End < From)
                {
                    currentIndex++;
                    continue;
                }
                if (currentSugg.Start > To)
                {
                    currentIndex++;
                    continue;
                }

                //Add unknown period if there is a gap
                if (currentSugg.Start > currentEnd)
                {
                    ret.Add(new WorktimeRecord(currentEnd, currentSugg.Start, "[unknown-gap1]", ""));
                    currentEnd = currentSugg.Start;
                }     

                //In case the next suggestions overlaps
                if (currentIndex + 1 < Suggestions.Count 
                    && Suggestions[currentIndex+1].Start < currentSugg.End)
                {
                    var nextSuggestion = Suggestions[currentIndex + 1];

                    if (nextSuggestion.Start >= currentEnd) //In case currentEnd starts before overlap
                    {
                        ret.Add(new WorktimeRecord(currentEnd, Suggestions[currentIndex + 1].Start, currentSugg.ProjectName, currentSugg.Comment));
                        currentEnd = Suggestions[currentIndex + 1].Start;
                    }
                    else //currentEnd starts within overlap -> skip to next element
                    {
                        currentIndex++;
                        continue;
                    }
                }  
                else //no overlaps
                {
                    var start = (currentSugg.Start > From) ? currentSugg.Start : From;
                    var end = (currentSugg.End < To) ? currentSugg.End : To;

                    ret.Add(new WorktimeRecord(start, end, currentSugg.ProjectName, currentSugg.Comment));
                    currentEnd = end;
                }

                currentIndex++;
            }

            //Add unknown period if there is a remaining gap at the end
            if (currentEnd < To)
                ret.Add(new WorktimeRecord(currentEnd, To, "[unknown-gap2]", ""));

            ret.First().Start.AddSeconds(From.Second);
            ret.Last().End.AddSeconds(To.Second);
            return ret;
        }
    }
}
