using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class MainHandler : IProjectChangeSubscriber
    {
        private List<ProjectChangeNotifier> projectChangeNotifier = new List<ProjectChangeNotifier>();
        private List<IProjectChangeSubscriber> projectChangeSubscriber = new List<IProjectChangeSubscriber>();

        private List<IWorktimeRecordStorage> worktimeRecordStorages = new List<IWorktimeRecordStorage>();

        private Form1 Form; //RTODO

        public MainHandler(Form1 form)
        {
            Form = form;
        }

        public void addProjectChangeNotifier(ProjectChangeNotifier notifier)
        {
            notifier.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifier.Add(notifier);

            foreach (var subscriber in projectChangeSubscriber)
            {
                notifier.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }
        }
        
        public void addProjectChangeSubscriber(IProjectChangeSubscriber subscriber)
        {
            projectChangeSubscriber.Add(subscriber);

            foreach (var notifier in projectChangeNotifier)
            {
                notifier.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }
        }

        public void addWorktimeRecordStorage(IWorktimeRecordStorage storage)
        {
            worktimeRecordStorages.Add(storage);
        }

        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            //RTODO locking?
            Console.WriteLine("Received this message: {0}", projectChangeEvent.ToString());

            foreach (var storage in worktimeRecordStorages)
            {
                try
                {
                    storage.addWorktimeRecord(projectChangeEvent.WorktimeRecord);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Upsi"); //RTODO
                    throw ex;
                }
                
            }
        }
    }
}
