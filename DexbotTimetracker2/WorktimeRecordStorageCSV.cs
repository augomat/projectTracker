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

        public void addWorktimeRecord(WorktimeRecord wtr)
        {
            if (wtr != null)
                writeCSVEntry((long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds), wtr.ProjectName, wtr.Start, wtr.End, wtr.Comment, true); //RTODO screentime
        }

        public List<WorktimeRecord> getAllWorktimeRecords()
        {
            throw new NotImplementedException();
        }

        //--------------------------

        public void writeCSVEntry(long diffSecs, string currentD, DateTime start, DateTime end, string addInfos, bool screenTime)
        {
            string output = String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{5};{0};{1};{3}",
                                                      diffSecs, currentD, end, addInfos, start, screenTime.ToString());


            File.AppendAllLines(fileNameCsv, new String[] { output });
            Console.WriteLine(output); //RTODO log
        }
    }
}
