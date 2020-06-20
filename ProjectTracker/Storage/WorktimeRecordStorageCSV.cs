using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ProjectTracker
{
    class WorktimeRecordStorageCSV : IWorktimeRecordStorage //subscriber
    {
        private static readonly String fileNameCsv;
        static WorktimeRecordStorageCSV()
        {
            fileNameCsv = Properties.Settings.Default.OutputCsvFilePath;
        }

        //--------------------------

        private Queue<string> pendingCSVEntries = new Queue<string>();

        [Obsolete("addWorktimeRecord is deprecated, please use addProjectEvent instead.")]
        public void addWorktimeRecord(WorktimeRecord wtr)
        {
            if (wtr != null)
            {
                var csv = generateCSVEntry((long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds), wtr.ProjectName, wtr.Start, wtr.End, wtr.Comment, isScreentime(wtr));
                pendingCSVEntries.Enqueue(csv);
            }

            writeOutAllPendingCSVEntries();
        }

        public void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent != null)
            {
                foreach (var wtr in projectChangeEvent.WorktimeRecords)
                {
                    var csv = generateCSVEntry((long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds), wtr.ProjectName, wtr.Start, wtr.End, wtr.Comment, isScreentime(wtr));
                    pendingCSVEntries.Enqueue(csv);
                }
            }

            writeOutAllPendingCSVEntries();
        }

        private void writeOutAllPendingCSVEntries()
        {
            while (pendingCSVEntries.Count > 0)
            {
                var csv = pendingCSVEntries.Peek();

                File.AppendAllLines(fileNameCsv, new String[] { csv });
                Console.WriteLine(csv); //RTODO log

                pendingCSVEntries.Dequeue();
            }
        }

        /*
         * very hacky implementation to determine whether something should be marked as Screentime t/f which is solely important for the CSV export/analysis spreadsheet
         * e.g. does not account for times on private screen, should be declared in settings
         * */
        private bool isScreentime(WorktimeRecord wtr)
        {
            if (wtr.ProjectName == ProjectChangeHandler.PROJECT_PAUSE || wtr.ProjectName == ProjectChangeHandler.PROJECT_WORKTIMEBREAK)
                return false;
            else
                return true;
        }

        public string generateCSVEntry(long diffSecs, string currentD, DateTime start, DateTime end, string addInfos, bool screenTime)
        {
            return String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{5};{0};{1};{3}",
                                                      diffSecs, currentD, end, addInfos, start, screenTime.ToString());
        }

        //-----------------

        public List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void ChangeStartTime(int index, DateTime newStartDate)
        {
            throw new NotImplementedException();
        }

        public void ChangeEndTime(int index, DateTime newStartDate)
        {
            throw new NotImplementedException();
        }

        public void ChangeProjectName(int index, string newProjectName)
        {
            throw new NotImplementedException();
        }

        public void ChangeProjectComment(int index, string newProjectComment)
        {
            throw new NotImplementedException();
        }

        public void updateOvertimes(Dictionary<string, TimeSpan> overtimes)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, TimeSpan> getOvertimes()
        {
            throw new NotImplementedException();
        }

        public List<String> getRecentProjects()
        {
            return new List<String>();
        }

        public List<string> getRecentComments(string projectName)
        {
            return new List<String>();
        }
    }
}
