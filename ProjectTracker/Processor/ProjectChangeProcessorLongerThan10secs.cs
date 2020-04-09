using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeProcessorLongerThan10secs : AProjectChangeProcessor
    {
        const string messageLonger = "Longer than 10 secs";

        public ProjectChangeProcessorLongerThan10secs(ProjectChangeHandler handler) : base(handler)
        { }
        
        public override bool process(ProjectChangeEvent projectChangeEvent)
        {
            //This is just a Testclass for testing the processor messaging system

            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Change && isLongerThan10Secs(Handler.currentProjectSince))
            {
                OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                               ProjectChangeEvent.Types.Test,
                               projectChangeEvent.NewProject,
                               messageLonger,
                               projectChangeEvent.WorktimeRecord
                               )
                           );
                return true;
            }
            return false;
        }

        private static bool isLongerThan10Secs(DateTime lastSwitched)
        {
            return (DateTime.Now.Subtract(new TimeSpan(0, 0, 10)) > lastSwitched) ? true : false;
        }
    }
}
