using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeSubscriberLogger : IProjectChangeSubscriber
    {
        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            Console.WriteLine("[{0}] (ChangeEvent) - {1}", DateTime.Now.ToString(), projectChangeEvent.ToString());
            //RTODO log to file
        }

        //RTODO create own logger class and fwd events to this class
    }
}
