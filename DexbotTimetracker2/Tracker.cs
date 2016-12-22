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
using System.Configuration;

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
		
        static Tracker()
        {
            fileNameCsv = System.Configuration.ConfigurationManager.AppSettings["OutputCsvFilePath"];
            fileNameLog = System.Configuration.ConfigurationManager.AppSettings["DexbotLogFilePath"];
        }

        private static readonly String fileNameLog;
        private static readonly String fileNameCsv;

        public string currentDesktop = "";
        public long lastSwitchPassedSecs = 0;
        private string desktopBeforeLock = "";

        public int countAsWorktimebreakMins { get; set; } = 0; //todo validations
        public int carryOverWorktimeCountHours { get; set; } = 0; //todo validations
        private int freeWorktimeBreakSecs = 0;

        public void startDesktopLogging()
        {
            if (string.IsNullOrEmpty(fileNameCsv) || string.IsNullOrEmpty(fileNameLog))
            {
                MessageBox.Show("fileNameCsv or fileNameLog not defined. Please use a valid app.config");
                //TODO programm will continue to run after this, show a wrong message bubble and do nothing. Quick fix with added a close-delegate in form.OnLoad didn't work (probably because form is already loaded?!)
                return;
            }

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
                    setCurrentVars = recordOnScreenSwitch();
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
                    lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks);
                }
            }
        }

        private bool recordOnScreenSwitch(string addInfos = "")
        {
            long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs;

            try
            {
                writeCSVEntry(diffSecs, currentDesktop, new DateTime(convertSecToTicks(lastSwitchPassedSecs)), DateTime.Now, addInfos, true);

                updateFreeWorktimeBreak();

                var timePassed = (convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs);
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

        private bool recordBackFromLockscreen(string addInfos = "", bool wasWorkbreak = false)
        {
            long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs;

            try
            {
                if (wasWorkbreak)
                {
                    //this is actually big bullshit as if writeCSVEntry fails, the free time will be updated but the lastSwitchPassedSecs not, so basically
                    //there you have a method pump up your freetime to the max. oh, how i love methods with side effects
                    var normalBreakSecs = updateFreeWorktimeBreak(true);
                    if (normalBreakSecs > 0)
                    {
                        //write worktimebreak
                        var worktimebreakEnd = new DateTime(convertSecToTicks(convertTicksToSec(DateTime.Now.Ticks) - normalBreakSecs));
                        writeCSVEntry(diffSecs - normalBreakSecs, "0", new DateTime(convertSecToTicks(lastSwitchPassedSecs)), worktimebreakEnd, "worktimebreak: " + addInfos, false);

                        //write normal break
                        writeCSVEntry(normalBreakSecs, "-1", worktimebreakEnd, DateTime.Now, "break: " + addInfos, false);

                        trayIcon.BalloonTipTitle = "Welcome back, all workbreaktime consumed";
                        trayIcon.BalloonTipText = "Total break: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)";
                        trayIcon.ShowBalloonTip(10);
                    }
                    else
                    {
                        writeCSVEntry(diffSecs, "0", new DateTime(convertSecToTicks(lastSwitchPassedSecs)), DateTime.Now, "worktimebreak: " + addInfos, false);

                        trayIcon.BalloonTipTitle = "Welcome back, workbreak left: " + (freeWorktimeBreakSecs).ToString() + " secs";
                        trayIcon.BalloonTipText = "Total break: " + (diffSecs / 60).ToString() + " mins (" + diffSecs.ToString() + " secs)";
                        trayIcon.ShowBalloonTip(10);
                    }
                }
                else
                {
                    writeCSVEntry(diffSecs, currentDesktop, new DateTime(convertSecToTicks(lastSwitchPassedSecs)), DateTime.Now, "off screen: " + addInfos, false);

                    updateFreeWorktimeBreak(true);

                    var timePassed = (convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs);
                    trayIcon.BalloonTipTitle = "Welcome back, workbreak left: " + (freeWorktimeBreakSecs).ToString() + " secs";
                    trayIcon.BalloonTipText = "Time on Desktop [" + currentDesktop + "]: " + (timePassed / 60).ToString() + " mins (" + timePassed.ToString() + " secs)";
                    trayIcon.ShowBalloonTip(10);
                }
                
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
                writeCSVEntry(0, "-1", new DateTime(convertSecToTicks(lastSwitchPassedSecs)), DateTime.Now, "New Day begun", false);

                freeWorktimeBreakSecs = 0;

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
            long diffSecs = convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs;

            try
            {
                if (!string.IsNullOrEmpty(currentDesktop))
                    writeCSVEntry(diffSecs, currentDesktop, new DateTime(convertSecToTicks(lastSwitchPassedSecs)), DateTime.Now, "Project Tracker exited", true);

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
                writeCSVEntry(convertTicksToSec(DateTime.Now.Ticks) - TimeInMins * 60 - lastSwitchPassedSecs, 
                    currentDesktop, 
                    new DateTime(convertSecToTicks(lastSwitchPassedSecs)), 
                    new DateTime(convertSecToTicks(convertTicksToSec(DateTime.Now.Ticks) - TimeInMins*60)), 
                    "Forgot to switch", true);

                //update lastSwitch with diff
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks) - TimeInMins * 60;
            }

            //set the currentDesktop to the intentional desktop so the next switch writes out the data as if the switch had occured to the intentional desktop
            currentDesktop = DesktopNo;
        }

        public void writeCSVEntry(long diffSecs, string currentD, DateTime start, DateTime end, string addInfos, bool screenTime)
        {
            string output = String.Format("{2:d};{4:HH:mm:ss};{2:HH:mm:ss};{5};{0};{1};{3}",
                                                      diffSecs, currentD, end, addInfos, start, screenTime.ToString());


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
            if (e.Reason == SessionSwitchReason.SessionLock 
                || e.Reason == SessionSwitchReason.RemoteDisconnect)
            {
                //I left my desk
                recordOnScreenSwitch("locked"); //TODO do not swallow return value
                desktopBeforeLock = currentDesktop;
                currentDesktop = "-1"; //break, no meeting - TODO make this enum
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                //I returned to my desk
                var lastSwitched = new DateTime(convertSecToTicks(lastSwitchPassedSecs));
                var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);

                //check whether todays 4am is within the locked interval and if so, do not count it as a break
                if (lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now)
                {
                    recordStartOfDay(); //TODO do not swallow return value
                }
                else
                {
                    Tuple<string, string> promptValues = Prompt.ShowDialog("Computer unlocked", "What did you do in the mean time?");
                    var promptString = promptValues.Item1;
                    var promptDesktop = promptValues.Item2;

                    currentDesktop = promptDesktop;

                    recordBackFromLockscreen(promptString, (promptDesktop == "0") ? true : false); //TODO do not swallow return value
                }
                
                currentDesktop = desktopBeforeLock;
                lastSwitchPassedSecs = convertTicksToSec(DateTime.Now.Ticks);
            }
        }

        private int updateFreeWorktimeBreak(bool wasAway = false)
        {
            var secsPassed = (convertTicksToSec(DateTime.Now.Ticks) - lastSwitchPassedSecs); //really bad, implies that lastSwitchPassedSecs is reset after calling updateFreeWorktime

            if (!wasAway)
            {
                //when working on screen, add gained seconds to total available worktimebreak 
                var currentWorktimebreakSecs = WorktimeSecsToWorktimebreakSecs(secsPassed);

                double factor = countAsWorktimebreakMins / 60.0;
                var maxWorktimebreakSecs = (int)System.Math.Ceiling(carryOverWorktimeCountHours * 60.0 * 60.0 * factor); //todo round

                freeWorktimeBreakSecs += currentWorktimebreakSecs;
                freeWorktimeBreakSecs = Math.Min(maxWorktimebreakSecs, freeWorktimeBreakSecs);
                return 0;
            }
            else
            {
                //when coming back, subtract from available worktimebreak and return secs that do not fall in worktimebreak
                freeWorktimeBreakSecs -= (int)secsPassed;
                var notInWorkbreakSecs = Math.Min(0, freeWorktimeBreakSecs)*-1;
                freeWorktimeBreakSecs = Math.Max(0, freeWorktimeBreakSecs);
                return notInWorkbreakSecs;
            }
        }

        private int WorktimeSecsToWorktimebreakSecs(long worktime)
        {
            double factor = countAsWorktimebreakMins / 60.0;
            return (int)System.Math.Ceiling(worktime * factor); //TODO round
        }

    }
}
