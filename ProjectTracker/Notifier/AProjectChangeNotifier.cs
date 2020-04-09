using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    abstract public class AProjectChangeNotifier //publisher
    {
        public ProjectChangeHandler Handler;
        protected AProjectChangeNotifier(ProjectChangeHandler handler)
        {
            Handler = handler;
        }

        public event EventHandler<ProjectChangeEvent> RaiseProjectChangeEvent;
        protected virtual void OnRaiseProjectChangeEvent(ProjectChangeEvent changeEvent)
        {
            EventHandler<ProjectChangeEvent> eventHandler = RaiseProjectChangeEvent;
            if (eventHandler != null)
            {
                eventHandler(this, changeEvent);
            }
        }

        public abstract void start();
    }
}
