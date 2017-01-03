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

        public void addWorktimeRecord(WorktimeRecord wtr)
        {
            if (wtr != null)
            {
                var csv = generateCSVEntry((long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds), wtr.ProjectName, wtr.Start, wtr.End, wtr.Comment, true);
                pendingCSVEntries.Enqueue(csv);
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

        public List<WorktimeRecord> getAllWorktimeRecords()
        {
            throw new NotImplementedException();
        }

        //--------------------------

        public string generateCSVEntry(long diffSecs, string currentD, DateTime start, DateTime end, string addInfos, bool screenTime)
        {
            return String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{5};{0};{1};{3}",
                                                      diffSecs, currentD, end, addInfos, start, screenTime.ToString());
        }
    }
}
