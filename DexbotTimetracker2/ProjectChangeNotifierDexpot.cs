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

        //-----------------------------------------------------

        public string currentDesktop = "";
        public long lastSwitchPassedSecs = 0;

        public override void start()
        {
            //Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch); //RTODO other PjChangeNotif!

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

                if (!string.IsNullOrEmpty(currentDesktop))
                {
                    long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs;
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        "Desktop Change detected",
                        "Time on Desktop [" + currentDesktop + "]: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)",
                        new WorktimeRecord(
                            new DateTime(convertSecToTicks(lastSwitchPassedSecs)),
                            DateTime.Now,
                            currentDesktop,
                            "")
                        )
                    );
                }
                else
                {
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Init,
                        "Desktop Change detected",
                        "Desktop initialized",
                        null
                        )
                    );
                }
                
                currentDesktop = desktopTo;
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks);
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
