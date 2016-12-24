using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class MainHandler //subscriber
    {
        
        public void start()
        {
            //RTODO init all publishers?
        }

        public void addProjectChangeNotifier(ProjectChangeNotifier notifier)
        {
            notifier.RaiseProjectChangeEvent += handleProjectChangeEvent;
        }

        void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            //RTODO locking?
            //RTODO
            Console.WriteLine("Received this message: {0}", projectChangeEvent.ToString());
        }
    }
}
