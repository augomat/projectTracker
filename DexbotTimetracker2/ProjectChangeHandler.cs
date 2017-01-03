using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ProjectTracker
{
    class ProjectChangeHandler : ProjectChangeNotifier, IProjectChangeSubscriber, IProjectHandler
    {
        public const string PROJECT_MEETING = "Meeting";
        public const string PROJECT_PAUSE = "Pause";
        public const string PROJECT_WORKTIMEBREAK = "Worktimebreak";

        private List<ProjectChangeNotifier> projectChangeNotifiers = new List<ProjectChangeNotifier>();
        private List<ProjectChangeProcessor> projectChangeProcessors = new List<ProjectChangeProcessor>();
        private List<IProjectChangeSubscriber> projectChangeSubscribers = new List<IProjectChangeSubscriber>();
        private List<IWorktimeRecordStorage> worktimeRecordStorages = new List<IWorktimeRecordStorage>();

        public ProjectChangeHandler(ProjectChangeHandler handler = null) : base(handler)
        {
            Handler = this;
            this.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifiers.Add(this);
        }

        public void init() //does not really help to change order of logging of processed & unprocessed //RTODO
        {
            this.RaiseProjectChangeEvent -= handleProjectChangeEvent;
            this.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifiers.Remove(this);
            projectChangeNotifiers.Add(this);
        }

        public override void start() { } //no need to start anything as we'll just re-fire events

        public string currentProject { get; private set; }
        public DateTime currentProjectSince { get; private set; }

        public void addProjectChangeNotifier(ProjectChangeNotifier notifier)
        {
            notifier.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeNotifiers.Add(notifier);

            foreach (var subscriber in projectChangeSubscribers)
            {
                notifier.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }

            Thread t = new Thread(notifier.start);
            t.IsBackground = true;
            t.Start();
        }

        public void addProjectChangeProcessor(ProjectChangeProcessor processor)
        {
            processor.RaiseProjectChangeEvent += handleProjectChangeEvent;
            projectChangeProcessors.Add(processor);

            foreach (var subscriber in projectChangeSubscribers)
            {
                processor.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }
        }

        public void addProjectChangeSubscriber(IProjectChangeSubscriber subscriber)
        {
            projectChangeSubscribers.Add(subscriber);

            foreach (var notifier in projectChangeNotifiers)
            {
                notifier.RaiseProjectChangeEvent += subscriber.handleProjectChangeEvent;
            }
        }

        public void addWorktimeRecordStorage(IWorktimeRecordStorage storage)
        {
            worktimeRecordStorages.Add(storage);
        }

        public event EventHandler<Exception> RaiseStorageExceptionEvent;
        protected virtual void OnRaiseStorageExceptionEvent(Exception ex)
        {
            var exHandler = RaiseStorageExceptionEvent;
            if (exHandler != null)
            {
                exHandler(this, ex);
            }
        }

        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            //RTODO locking?
            //Console.WriteLine("Received this message: {0}", projectChangeEvent.ToString());

            if (!projectChangeEvent.Processed)
            {
                Boolean processed = false;
                foreach (var processor in projectChangeProcessors)
                {
                    if (sender != processor)
                        processed |= processor.process(projectChangeEvent);
                }
                if (!processed)
                    reRaiseEventAsProcessed(projectChangeEvent);
            }
            else
            {
                //Invoke all storages
                foreach (var storage in worktimeRecordStorages)
                {
                    try
                    {
                        foreach (var wtr in projectChangeEvent.WorktimeRecords)
                        {
                            storage.addWorktimeRecord(wtr);
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseStorageExceptionEvent(this, ex);
                    }
                }

                //Update fields
                if (projectChangeEvent.WorktimeRecord != null && projectChangeEvent.WorktimeRecords.Last().End >= currentProjectSince)
                {
                    currentProject = projectChangeEvent.NewProject;
                    currentProjectSince = projectChangeEvent.WorktimeRecords.Last().End;
                }
            }
        }

        private void reRaiseEventAsProcessed(ProjectChangeEvent projectChangeEvent)
        {
            var newEvent = new ProjectChangeEvent(projectChangeEvent);
            newEvent.Processed = true;
            OnRaiseProjectChangeEvent(newEvent);
        }

        public static List<string> getAvailableProjects()
        {
            var list = new List<string>();
            list.Add(PROJECT_MEETING);
            list.Add(PROJECT_PAUSE);
            list.Add(PROJECT_WORKTIMEBREAK);
            list.AddRange(Properties.Settings.Default.AvailableProjects.Cast<string>().ToArray());
            return list;
        }
    }
}
