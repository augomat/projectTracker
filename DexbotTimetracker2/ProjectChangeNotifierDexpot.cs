using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ProjectTracker.Util;
using System.IO;
using System.Text.RegularExpressions;

namespace ProjectTracker
{
    class ProjectChangeNotifierDexpot : ProjectChangeNotifier //publisher
    {
        private static readonly String fileNameLog;

        private DexpotSettings DexpotSettings = new DexpotSettings();

        static ProjectChangeNotifierDexpot()
        {
            fileNameLog = Properties.Settings.Default.DexbotLogFilePath;
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

                if (!string.IsNullOrEmpty(Handler.currentProject)) //TODO - should that really be decided here?
                {
                    long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - convertTicksToSec(Handler.currentProjectSince.Ticks);
                    OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                        ProjectChangeEvent.Types.Change,
                        desktopToProjectName(desktopTo),
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
                        desktopToProjectName(desktopTo),
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

        private string desktopToProjectName(string desktopName)
        {
            try
            {
                return DexpotSettings.desktopToProjectName[desktopName];
            }
            catch
            {
                return desktopName;
            }
            
        }
    }

    public class DexpotSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public SerializableDictionary<string, string> desktopToProjectName
        {
            get { return (SerializableDictionary<string, string>)this["desktopToProjectName"]; }
            set { }
        }
    }
}
