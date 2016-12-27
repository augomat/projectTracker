using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

namespace ProjectTracker
{
    class ProjectChangeNotifierDexpot : ProjectChangeNotifier //publisher
    {
        private static readonly String fileNameLog;

        static ProjectChangeNotifierDexpot()
        {
            fileNameLog = System.Configuration.ConfigurationManager.AppSettings["DexbotLogFilePath"];
        }

        public ProjectChangeNotifierDexpot(ProjectChangeHandler handler) : base(handler) { }

        //-----------------------------------------------------

        public override void start()
        {
            using (StreamReader reader = new StreamReader(new FileStream(fileNameLog,
                     FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                //start at the end of the file
                long lastMaxOffset = reader.BaseStream.Length;

                while (true)
                {
                    System.Threading.Thread.Sleep(100);

                    //if the file size has not changed, idle
                    if (reader.BaseStream.Length == lastMaxOffset)
                        continue;

                    //seek to the last max offset
                    reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                    //read out of the file until the EOF
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                        processLine(line);

                    //update the last max offset
                    lastMaxOffset = reader.BaseStream.Position;
                }
            }
        }

        private void processLine(string line)
        {
            if (line.Contains(">>> Desktopwechsel"))
            {
                var match = Regex.Match(line, @"Desktopwechsel von (\d) nach (\d)");
                var desktopFrom = match.Groups[1].Value;
                var desktopTo = match.Groups[2].Value;

                if (!string.IsNullOrEmpty(Handler.currentProject))
                {
                    long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - convertTicksToSec(Handler.currentProjectSince.Ticks);
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        "Desktop Change detected",
                        new WorktimeRecord(
                            new DateTime(Handler.currentProjectSince.Ticks),
                            DateTime.Now,
                            Handler.currentProject,
                            "")
                        )
                    );
                }
                else
                {
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Init,
                        "Desktop initialized",
                        new WorktimeRecord(
                            DateTime.Now,
                            DateTime.Now,
                            desktopTo,
                            "Desktop initialized")
                        )
                    );
                }
            }
        }
        
        private static long convertTicksToSec(long ticks)
        {
            return (long)(ticks / TimeSpan.TicksPerMillisecond / 1000);
        }

        private static long convertSecToTicks(long seconds)
        {
            return (long)(seconds * TimeSpan.TicksPerMillisecond * 1000);
        }
    }
}
