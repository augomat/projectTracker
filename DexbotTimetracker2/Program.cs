using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace DexbotTimetracker2
{
    static class Program
    {
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //TODO run this in a separate thread as menu doesn't work because constructor of context never returns
            Application.Run(new MyCustomApplicationContext());
        }
    }

    public class MyCustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public MyCustomApplicationContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(Path.GetFullPath(@"asd.ico")),
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", Exit)
            }),
                Visible = true
            };

            startDesktopLogging();
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }

        //-------------------------------------------------------------

        private const string fileNameLog = @"C:\Users\gkapeller\AppData\Roaming\Dexpot\dexpot.log";
        private const string fileNameCsv = @"C:\Users\gkapeller\Documents\DesktopTimes.csv";

        private string currentDesktop = "";
        private long lastSwitch = 0;

        private void startDesktopLogging()
        {
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

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

                bool setCurrentVars = true;
                if (!string.IsNullOrEmpty(currentDesktop))
                {
                    setCurrentVars = recordSwitch();
                }

                if (setCurrentVars)
                {
                    currentDesktop = desktopTo;
                    lastSwitch = convertTime(DateTime.Now.Ticks);
                }
            }
        }

        private bool recordSwitch()
        {
            string output = String.Format("{2:d};{2:t};{0};{1}", convertTime(DateTime.Now.Ticks) - lastSwitch, currentDesktop, DateTime.Now);
            Console.WriteLine(output);
            try
            {
                File.AppendAllLines(fileNameCsv, new String[] { output });

                trayIcon.BalloonTipTitle = "Desktop change detected";
                trayIcon.BalloonTipText = "Time on Desktop " + currentDesktop + ": " + (convertTime(DateTime.Now.Ticks) - lastSwitch).ToString();
                trayIcon.ShowBalloonTip(10);

                return true;
            } catch (Exception e)
            {
                trayIcon.BalloonTipTitle = "Exception occurred";
                trayIcon.BalloonTipText = e.ToString();
                trayIcon.ShowBalloonTip(10);

                return false;
            }
        }

        private static int convertTime(long ticks)
        {
            return (int)(ticks / TimeSpan.TicksPerMillisecond / 1000);
        }

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                //I left my desk
                recordSwitch();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                lastSwitch = convertTime(DateTime.Now.Ticks);
            }
        }
    }
}


