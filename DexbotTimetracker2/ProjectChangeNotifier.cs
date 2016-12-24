using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    abstract class ProjectChangeNotifier //publisher
    {
        public event EventHandler<ProjectChangeEvent> RaiseProjectChangeEvent;
        protected virtual void OnRaiseProjectChangeEvent(ProjectChangeEvent changeEvent)
        {
            EventHandler<ProjectChangeEvent> handler = RaiseProjectChangeEvent;
            if (handler != null)
            {
                handler(this, changeEvent);
            }
        }

        public abstract void start();
    }
}
