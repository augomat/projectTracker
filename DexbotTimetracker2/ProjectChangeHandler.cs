using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectTracker
{
    class ProjectChangeHandler : ProjectChangeNotifier, IProjectChangeSubscriber
    {
        private List<ProjectChangeNotifier> projectChangeNotifier = new List<ProjectChangeNotifier>();
        private List<IProjectChangeSubscriber> projectChangeSubscriber = new List<IProjectChangeSubscriber>();
        private List<IWorktimeRecordStorage> worktimeRecordStorages = new List<IWorktimeRecordStorage>();

        public ProjectChangeHandler(ProjectChangeHandler handler = null) : base(handler)
        {
            this.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifier.Add(this);
        }

        public override void start() { } //no need to start anything as we'll just re-fire events

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

            processProjectChangeEvent(projectChangeEvent);

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

        //-------------------------------------

        public void processProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
        {
            //TODO get this out of the handler into a processer
            //TODO a event processing chain would be cool

            if (projectChangeEvent.Processed)
                return;

            //??? Maybe make processor first in chain and then invoke the rest (ie re-refire) such that the others do not need (cannot) ignore unprocessed events
            //if (isNewDay(currentProjectSince))
            {
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                   ProjectChangeEvent.Types.Start,
                   "Good morning",
                   new WorktimeRecord(
                       currentProjectSince,
                       DateTime.Now,
                       currentProject,
                       "New day begun"),
                   true
                   )
               );
            }
        }

        private static bool isNewDay(DateTime lastSwitched)
        {
            var TodayAt4am = DateTime.Now.Date + new TimeSpan(4, 0, 0);
            return lastSwitched < TodayAt4am && TodayAt4am < DateTime.Now;
        }
    }
}
