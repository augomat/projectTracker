/*
 * Created by SharpDevelop.
 * User: Georg
 * Date: 03.12.2016
 * Time: 00:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using System.Windows.Forms;

namespace DexbotTimetracker2
{
	/// <summary>
	/// Description of Tracker.
	/// </summary>
	public class Tracker
	{
		private NotifyIcon trayIcon;
		
		public Tracker(NotifyIcon notifyIconExt)
		{
			trayIcon = notifyIconExt;
		}
		
		//-------------------------------------------------------------

        //private const string fileNameLog = @"C:\Users\gkapeller\AppData\Roaming\Dexpot\dexpot.log";
        //private const string fileNameCsv = @"C:\Users\gkapeller\Documents\DesktopTimes.csv";
        
        private const string fileNameLog = @"C:\Users\Georg\AppData\Roaming\Dexpot\dexpot.log";
        private const string fileNameCsv = @"C:\Users\Georg\Documents\DesktopTimes.csv";
        
        private string currentDesktop = "";
        private long lastSwitch = 0;
        private string desktopBeforeLock = "";

        public void startDesktopLogging()
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
                    lastSwitch = convertTimeToSec(DateTime.Now.Ticks);
                }
            }
        }

        private bool recordSwitch(string addInfos = "", bool zeroTime = false)
        {
            long diffSecs = (!zeroTime) ? convertTimeToSec(DateTime.Now.Ticks) - lastSwitch : 0;

            string output = String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{0};{1};{3}", 
        	                              diffSecs, currentDesktop, DateTime.Now, addInfos, new DateTime(convertTimeToTicks(lastSwitch)));
            Console.WriteLine(output);
            try
            {
                File.AppendAllLines(fileNameCsv, new String[] { output });

                trayIcon.BalloonTipTitle = "Desktop change detected";
                trayIcon.BalloonTipText = "Time on Desktop " + currentDesktop + ": " + (convertTimeToSec(DateTime.Now.Ticks) - lastSwitch).ToString();
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

        private static long convertTimeToSec(long ticks)
        {
            return (long)(ticks / TimeSpan.TicksPerMillisecond / 1000);
        }
        
        private static long convertTimeToTicks(long seconds)
        {
            return (long)(seconds * TimeSpan.TicksPerMillisecond * 1000);
        }

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                //I left my desk
                recordSwitch("locked"); //TODO do not swallow return value
                desktopBeforeLock = currentDesktop;
                currentDesktop = "0";
                lastSwitch = convertTimeToSec(DateTime.Now.Ticks);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
               recordSwitch("unlocked", true); //TODO do not swallow return value
               currentDesktop = desktopBeforeLock;
               lastSwitch = convertTimeToSec(DateTime.Now.Ticks);
            }
        }
	}
}
