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

        private const string fileNameLog = @"C:\Users\gkapeller\AppData\Roaming\Dexpot\dexpot.log";
        private const string fileNameCsv = @"C:\Users\gkapeller\Documents\DesktopTimes.csv";
        
        //private const string fileNameLog = @"C:\Users\Georg\AppData\Roaming\Dexpot\dexpot.log";
        //private const string fileNameCsv = @"C:\Users\Georg\Documents\DesktopTimes.csv";

        public string currentDesktop = "";
        public long lastSwitchSecs = 0;
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
                else
                {
                    trayIcon.BalloonTipTitle = "Desktop change detected";
                    trayIcon.BalloonTipText = "Desktop initialized";
                    trayIcon.ShowBalloonTip(10);
                }

                if (setCurrentVars)
                {
                    currentDesktop = desktopTo;
                    lastSwitchSecs = convertTicksToSec(DateTime.Now.Ticks);
                }
            }
        }

        private bool recordSwitch(string addInfos = "")
        {
            long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchSecs;

            try
            {
                writeCSVEntry(diffSecs, currentDesktop, new DateTime(convertSecToTicks(lastSwitchSecs)), DateTime.Now, addInfos);

                var timePassed = (convertTicksToSec(DateTime.Now.Ticks) - lastSwitchSecs);

                trayIcon.BalloonTipTitle = "Desktop change detected";
                trayIcon.BalloonTipText = "Time on Desktop [" + currentDesktop + "]: " + (timePassed/60).ToString() + " mins (" + timePassed.ToString() + " secs)" ;
                trayIcon.ShowBalloonTip(10);

                return true;
            }
            catch (Exception e)
            {
                trayIcon.BalloonTipTitle = "Exception occurred";
                trayIcon.BalloonTipText = e.ToString();
                trayIcon.ShowBalloonTip(10);

                return false;
            }
        }

        private bool recordStartOfDay()
        {
            try
            {
                writeCSVEntry(0, "-1", new DateTime(convertSecToTicks(lastSwitchSecs)), DateTime.Now, "New Day begun");

                trayIcon.BalloonTipTitle = "Desktop change detected";
                trayIcon.BalloonTipText = "Good morning";
                trayIcon.ShowBalloonTip(10);

                return true;
            }
            catch (Exception e)
            {
                trayIcon.BalloonTipTitle = "Exception occurred";
                trayIcon.BalloonTipText = e.ToString();
                trayIcon.ShowBalloonTip(10);

                return false;
            }
        }

        public bool recordAppExit()
        {
            long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchSecs;

            try
            {
                writeCSVEntry(diffSecs, currentDesktop, new DateTime(convertSecToTicks(lastSwitchSecs)), DateTime.Now, "Project Tracker exited");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void doFakeSwitch(string DesktopNo, int TimeInMins)
        {
            if (TimeInMins > 0)
            {
                //TODO check time < trackertime

                //write out difference to CSV
                writeCSVEntry(convertTicksToSec(DateTime.Now.Ticks) - TimeInMins * 60 - lastSwitchSecs, 
                    currentDesktop, 
                    new DateTime(convertSecToTicks(lastSwitchSecs)), 
                    new DateTime(convertSecToTicks(convertTicksToSec(DateTime.Now.Ticks) - TimeInMins*60)), 
                    "Forgot to switch");

                //update lastSwitch with diff
                lastSwitchSecs = convertTicksToSec(DateTime.Now.Ticks) - TimeInMins * 60;
            }

            //set the currentDesktop to the intentional desktop so the next switch writes out the data as if the switch had occured to the intentional desktop
            currentDesktop = DesktopNo;
        }

        public void writeCSVEntry(long diffSecs, string currentD, DateTime start, DateTime end, string addInfos)
        {
            string output = String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{0};{1};{3}",
                                                      diffSecs, currentD, end, addInfos, start);


            File.AppendAllLines(fileNameCsv, new String[] { output });
            Console.WriteLine(output);
        }

        private static long convertTicksToSec(long ticks)
        {
            return (long)(ticks / TimeSpan.TicksPerMillisecond / 1000);
        }
        
        private static long convertSecToTicks(long seconds)
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
                currentDesktop = "-1"; //break, no meeting - TODO make this enum
                lastSwitchSecs = convertTicksToSec(DateTime.Now.Ticks);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                var lastSwitched = new DateTime(convertSecToTicks(lastSwitchSecs));
                var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);

                //check whether todays 4am is within the locked interval and if so, do not count it as a break
                if (lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now)
                {
                    recordStartOfDay(); //TODO do not swallow return value
                }
                else
                {
                    Tuple<string, bool> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                    var promptString = promptValues.Item1;
                    var promptIsMeeting = promptValues.Item2;

                    if (promptIsMeeting && currentDesktop == "-1")
                        currentDesktop = "-2"; //meeting, no break - TODO make this an enum

                    recordSwitch("unlocked: " + promptString); //TODO do not swallow return value
                }
                
                currentDesktop = desktopBeforeLock;
                lastSwitchSecs = convertTicksToSec(DateTime.Now.Ticks);
            }
        }
	}
}
