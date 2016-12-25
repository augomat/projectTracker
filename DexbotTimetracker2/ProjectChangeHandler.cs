using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectTracker
{
    class ProjectChangeHandler : IProjectChangeSubscriber
    {
        private List<ProjectChangeNotifier> projectChangeNotifier = new List<ProjectChangeNotifier>();
        private List<IProjectChangeSubscriber> projectChangeSubscriber = new List<IProjectChangeSubscriber>();
        private List<IWorktimeRecordStorage> worktimeRecordStorages = new List<IWorktimeRecordStorage>();

        public string currentProject { get; private set; }
        public DateTime currentProjectSince { get; private set; }

        public void addProjectChangeNotifier(ProjectChangeNotifier notifier)
        {
            notifier.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifier.Add(notifier);

            foreach (var subscriber in projectChangeSubscriber)
            {
                notifier.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }

            Thread t = new Thread(notifier.start);
            t.IsBackground = true;
            t.Start();
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

            //Invoke all storages
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

            //Update fields
            //RTODO only do this in case(?) storage was ok? or always?
            if (projectChangeEvent.WorktimeRecord != null)
            {
                currentProject = projectChangeEvent.WorktimeRecord.ProjectName;
                currentProjectSince = projectChangeEvent.WorktimeRecord.End;
            }
        }
    }
}
