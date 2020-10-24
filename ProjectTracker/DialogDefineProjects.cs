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
    public class DialogDefineProjects
    {
        private const string GAP_PROJECTNAME = "[unknown-gap1]";

        private Form prompt;

        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button AddRowButton;

        private List<TextBox> minutes = new List<TextBox>();
        private List<TextBox> comments = new List<TextBox>();
        private List<ComboBox> projects = new List<ComboBox>();

        private Label labelNow;
        private Label labelProject;
        private TextBox currentComment;
        private ComboBox currentProject;

        private const int lineHeightAdd = 25;
        private int lastLineHeight = 69 - lineHeightAdd;
        private int nextTabIndex = 0;

        private int MinutesTotal;
        private DateTime From;
        private DateTime To;
        private List<WorktimeRecord> Suggestions;
        private IProjectHandler Handler;

        public List<WorktimeRecord> ShowDialogMeantime(DateTime from, DateTime to, ProjectChangeHandler handler, List<WorktimeRecord> suggestions = null)
        {
            MinutesTotal = (int)(Math.Floor((to - from).TotalMinutes));
            From = from;
            To = to;
            Suggestions = suggestions;
            Handler = handler;

            generateForm("What did you do in the mean time?");

            AddRowButton = new System.Windows.Forms.Button() { Left = 461, Top = 68 - lineHeightAdd, Width = 23, Text = "+" };
            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };

            AddRowButton.Click += AddRowButton_ClickMeantime;
            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            OkButton.Click += OkButton_Click;
            prompt.AcceptButton = OkButton;

            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 47, Top = 24, Text = "Break: ", Width = 40 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 24, Text = $"{MinutesTotal} mins" });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 });
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);
            prompt.Controls.Add(AddRowButton);

            if (suggestions != null)
                processSuggestions();
            else
                createRow();

            createRowNewProject(false, Handler.currentProjectComment);

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            var ret = new List<WorktimeRecord>();
            if (result == DialogResult.OK)
            {
                DateTime start = from;
                for (int index = 0; index < minutes.Count; index++)
                {
                    var end = start.AddMinutes(Convert.ToInt32(minutes[index].Text));
                    ret.Add(new WorktimeRecord(start, end, projects[index].Text, comments[index].Text));
                    start = end;
                }
                ret.Last().End = to; //to compensate for additional seconds

                //Also pass information regarding the current project
                ret.Add(new WorktimeRecord(DateTime.MinValue, DateTime.MinValue, currentProject.Text, currentComment.Text)); //what a hack
            }
            return ret;
        }

        public void ShowDialogChangeCurrentProject(IProjectHandler handler)
        {
            Handler = handler;

            generateForm("CHANGE your CURRENT project");

            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };
            CancelButton = new System.Windows.Forms.Button() { Left = 327, Top = 112 - lineHeightAdd, Width = 75, Text = "Cancel" };

            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            CancelButton.DialogResult = DialogResult.Cancel;
            CancelButton.Click += (sender, e) => { prompt.Close(); };
            prompt.AcceptButton = OkButton;
            prompt.CancelButton = CancelButton;

            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 });
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);

            createRowNewProject(false, Handler.currentProjectComment);

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            if (result == DialogResult.OK)
            {
                Handler.changeCurrentProjectRetrospectively(currentProject.Text, currentComment.Text);
            }
        }

        public WorktimeRecord ShowDialogNewProject(IProjectHandler handler)
        {
            Handler = handler;

            generateForm("CREATE a NEW project from now on");

            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };
            CancelButton = new System.Windows.Forms.Button() { Left = 327, Top = 112 - lineHeightAdd, Width = 75, Text = "Cancel" };

            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            CancelButton.DialogResult = DialogResult.Cancel;
            CancelButton.Click += (sender, e) => { prompt.Close(); };
            prompt.AcceptButton = OkButton;
            prompt.CancelButton = CancelButton;

            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 });
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);

            createRowNewProject(false);

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            if (result == DialogResult.OK)
                return new WorktimeRecord(Handler.currentProjectSince, DateTime.Now, currentProject.Text, currentComment.Text);
            else
                return null;
        }

        public List<WorktimeRecord> ShowDialogSplitCurrentProject(IProjectHandler handler, List<WorktimeRecord> suggestions = null)
        {
            Handler = handler;
            From = Handler.currentProjectSince;
            To = DateTime.Now;
            MinutesTotal = (int)(Math.Floor((To - From).TotalMinutes));
            Suggestions = suggestions;
            Handler = handler;

            generateForm("Split/Edit current project");

            AddRowButton = new System.Windows.Forms.Button() { Left = 461, Top = 68 - lineHeightAdd, Width = 23, Text = "+" };
            OkButton = new System.Windows.Forms.Button() { Left = 410, Top = 112 - lineHeightAdd, Width = 75, Text = "OK" };
            CancelButton = new System.Windows.Forms.Button() { Left = 327, Top = 112 - lineHeightAdd, Width = 75, Text = "Cancel" };

            AddRowButton.Click += AddRowButton_ClickSplit;
            OkButton.Click += (sender, e) => { prompt.Close(); };
            OkButton.DialogResult = DialogResult.OK;
            CancelButton.DialogResult = DialogResult.Cancel;
            CancelButton.Click += (sender, e) => { prompt.Close(); };
            prompt.AcceptButton = OkButton;
            prompt.CancelButton = CancelButton;

            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 47, Top = 24, Text = "Length: ", Width = 40 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 24, Text = $"{MinutesTotal} mins" });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 41, Top = 53, Text = "Minutes", Width = 50, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 221, Top = 53, Text = "Comment", Width = 60, Height = 13 });
            prompt.Controls.Add(new System.Windows.Forms.Label() { Left = 91, Top = 53, Text = "Project", Width = 50, Height = 13 });
            prompt.Controls.Add(OkButton);
            prompt.Controls.Add(CancelButton);
            prompt.Controls.Add(AddRowButton);

            if (suggestions != null)
                processSuggestions();
            else
                createRow();

            createRowNewProject(false);

            continuallyFocusDialog();
            centerDialogOnMainscreen();
            var result = prompt.ShowDialog();

            var ret = new List<WorktimeRecord>();
            if (result == DialogResult.OK)
            {
                DateTime start = From;
                for (int index = 0; index < minutes.Count; index++)
                {
                    var end = start.AddMinutes(Convert.ToInt32(minutes[index].Text));
                    ret.Add(new WorktimeRecord(start, end, projects[index].Text, comments[index].Text));
                    start = end;
                }

                ret.Add(new WorktimeRecord(start, DateTime.Now, currentProject.Text, currentComment.Text));
            }
            return ret;
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
            if (getMinutesLeft() != 0)
            {
                MessageBox.Show($"Total Sum of minutes must be exactly {MinutesTotal}", "Error");
                prompt.DialogResult = DialogResult.None;
            }   
        }
        
        private void AddRowButton_ClickMeantime(object sender, EventArgs e)
        {
            //Not exactly beautiful solution but it works
            removeRowNewProject();
            createRow();
            createRowNewProject(false);
        }

        private void AddRowButton_ClickSplit(object sender, EventArgs e)
        {
            //Not exactly beautiful solution but it works
            removeRowNewProject();
            createRow();
            createRowNewProject(false);
        }

        /**
         * Normal row with Project / Comment / Add-Button
         */
        private void createRow()
        {
            lastLineHeight += lineHeightAdd;

            minutes.Add(new System.Windows.Forms.TextBox() { Left = 44, Top = lastLineHeight, Width = 38 });
            comments.Add(createCommentTextfield(221, lastLineHeight, 236));
            projects.Add(createProjectCombobox());

            minutes.Last().TabIndex = nextTabIndex++;
            projects.Last().TabIndex = nextTabIndex++;
            comments.Last().TabIndex = nextTabIndex++;
            AddRowButton.TabIndex = nextTabIndex++;
            OkButton.TabIndex = nextTabIndex++;
            if (CancelButton != null)
                CancelButton.TabIndex = nextTabIndex++;

            prompt.Controls.Add(minutes.Last());
            prompt.Controls.Add(comments.Last());
            prompt.Controls.Add(projects.Last());

            AddRowButton.Top += lineHeightAdd;
            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;

            minutes.Last().Text = getMinutesLeft().ToString();
            //minutes.Last().Validating += Break_Validating;
            minutes.Last().SelectAll();
            minutes.Last().Focus();
        }

        /**
         * Row with just a comment-field
         */
        private void createRowCurrentProject()
        {
            lastLineHeight += lineHeightAdd;

            labelNow = new System.Windows.Forms.Label() { Left = 44, Top = lastLineHeight, Width = 38, Text = "Now" };
            string lastComment = currentComment != null ? currentComment.Text : "";
            currentComment = createCommentTextfield(221, lastLineHeight, 236);
            currentComment.Text = lastComment != "" ? lastComment : Handler.currentProjectComment;
            labelProject = new System.Windows.Forms.Label() { Left = 91, Top = lastLineHeight, Width = 121, Text = Handler.currentProject };

            if (AddRowButton != null)
                AddRowButton.TabIndex = nextTabIndex++;
            
            currentComment.TabIndex = nextTabIndex++;
            OkButton.TabIndex = nextTabIndex++;
            if (CancelButton != null)
                CancelButton.TabIndex = nextTabIndex++;

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

        /**
         * Row with Project & Comment
         */
        private void createRowNewProject(bool reverseTabOrder = true, string comment = "")
        {
            lastLineHeight += lineHeightAdd;

            labelNow = new System.Windows.Forms.Label() { Left = 44, Top = lastLineHeight, Width = 38, Text = "Now" };
            currentComment = createCommentTextfield(221, lastLineHeight, 236);
            currentComment.Text = comment;
            currentProject = createProjectCombobox();
            currentProject.SelectedIndex = -1;
            currentProject.Text = Handler.currentProject;

            if (reverseTabOrder)
            {
                currentComment.TabIndex = nextTabIndex++;
                currentProject.TabIndex = nextTabIndex++;
            }
            else
            {
                currentProject.TabIndex = nextTabIndex++;
                currentComment.TabIndex = nextTabIndex++;
            }
            OkButton.TabIndex = nextTabIndex++;
            if (CancelButton != null)
                CancelButton.TabIndex = nextTabIndex++;

            prompt.Controls.Add(labelNow);
            prompt.Controls.Add(currentComment);
            prompt.Controls.Add(currentProject);

            OkButton.Top += lineHeightAdd;
            prompt.Height += lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top += lineHeightAdd;
        }
        private void removeRowNewProject()
        {
            prompt.Controls.Remove(labelNow);
            prompt.Controls.Remove(currentComment);
            prompt.Controls.Remove(currentProject);
            nextTabIndex -= 2;

            OkButton.Top -= lineHeightAdd;
            prompt.Height -= lineHeightAdd;
            if (CancelButton != null)
                CancelButton.Top -= lineHeightAdd;

            lastLineHeight -= lineHeightAdd;
        }

        private ComboBox createProjectCombobox()
        {
            var combobox = new System.Windows.Forms.ComboBox() { Left = 91, Top = lastLineHeight, Width = 121, AutoCompleteMode = AutoCompleteMode.Append, AutoCompleteSource = AutoCompleteSource.ListItems};
            combobox.Items.AddRange(Handler.getAvailableProjects().Cast<string>().ToArray());
            combobox.SelectedIndex = Handler.getAvailableProjectIndex(ProjectChangeHandler.PROJECT_WORKTIMEBREAK); 
            return combobox;
        }
        
        private TextBox createCommentTextfield(int Left, int Top, int Width)
        {
            var textfield = new System.Windows.Forms.TextBox()
            {
                Left = Left,
                Top = Top,
                Width = Width,
                Text = "",
                AutoCompleteMode = AutoCompleteMode.Append,
                AutoCompleteSource = AutoCompleteSource.CustomSource
            };
            textfield.GotFocus += CommentTextfield_GotFocus;

            return textfield;
        }

        private void CommentTextfield_GotFocus(object sender, EventArgs e)
        {
            string projectName = "";
            if (sender == currentComment && currentProject != null)
                projectName = currentProject.Text;
            else
            {
                var commentIndex = comments.IndexOf((TextBox)sender);
                if (commentIndex != -1)
                    projectName = projects[commentIndex].Text;
            }

            var suggestions = Handler.getSuggestedComments(projectName);
            var suggestionList = new AutoCompleteStringCollection();
            suggestionList.AddRange(suggestions.ToArray());

            ((TextBox)sender).AutoCompleteCustomSource = suggestionList;
        }

        private int getMinutesLeft()
        {
            int sum = 0;
            foreach(var brk in minutes)
            {
                if (brk.Text != "")
                    sum += Convert.ToInt32(brk.Text);
            }
            return MinutesTotal - sum;
        }

        private void processSuggestions()
        {
            var fullList = calculateFullSuggestionList();

            foreach (var wtr in fullList)
            {
                createRow();
                minutes.Last().Text = ((int)Math.Floor((wtr.End - wtr.Start).TotalMinutes)).ToString();
                comments.Last().Text = wtr.Comment;
                if (wtr.ProjectName == GAP_PROJECTNAME)
                    projects.Last().SelectedIndex = Handler.getAvailableProjectIndex(ProjectChangeHandler.PROJECT_WORKTIMEBREAK);
                else
                    projects.Last().SelectedIndex = ProjectChangeHandler.getFirstCustomProjectIndex();
            }

            //Hacky the hack, adjust last value if we floored too much
            var breakLeft = getMinutesLeft();
            minutes.Last().Text = (Convert.ToInt32(minutes.Last().Text) + breakLeft).ToString();
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
                    ret.Add(new WorktimeRecord(currentEnd, currentSugg.Start, GAP_PROJECTNAME, ""));
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
                ret.Add(new WorktimeRecord(currentEnd, To, GAP_PROJECTNAME, ""));

            ret.First().Start.AddSeconds(From.Second);
            ret.Last().End.AddSeconds(To.Second);
            return ret;
        }
    }
}
