using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class MainHandler //subscriber
    {
        private List<IWorktimeRecordStorage> worktimeRecordStorages;
        private Form1 Form; //RTODO

        public MainHandler(Form1 form)
        {
            Form = form;
            worktimeRecordStorages = new List<IWorktimeRecordStorage>();
        }

        public void addProjectChangeNotifier(ProjectChangeNotifier notifier)
        {
            notifier.RaiseProjectChangeEvent += handleProjectChangeEvent;
        }

        public void addWorktimeRecordStorage(IWorktimeRecordStorage storage)
        {
            worktimeRecordStorages.Add(storage);
        }

        void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            //RTODO locking?
            //RTODO
            Console.WriteLine("Received this message: {0}", projectChangeEvent.ToString());

            foreach (var storage in worktimeRecordStorages)
            {
                storage.addWorktimeRecord(projectChangeEvent.WorktimeRecord);
            }

            //RTODO aussi
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Init)
            {
                Form.trayIcon.BalloonTipTitle = "Project change detected";
                Form.trayIcon.BalloonTipText = "Desktop initialized";
                Form.trayIcon.ShowBalloonTip(10);
            }
            else if (projectChangeEvent.WorktimeRecord != null)
            {
                var wtr = projectChangeEvent.WorktimeRecord;
                var project = wtr.ProjectName;
                var timePassed = (long)System.Math.Abs((wtr.End - wtr.Start).TotalSeconds);
                Form.trayIcon.BalloonTipTitle = "Project change";
                Form.trayIcon.BalloonTipText = "Time on last project [" + project + "]: " + (timePassed / 60).ToString() + " mins (" + timePassed.ToString() + " secs)";
                Form.trayIcon.ShowBalloonTip(10);
            }
            else
            {
                Console.WriteLine("Possibly wrong projectChangeEvent detected and ignored");
            }
            
        }
    }
}
