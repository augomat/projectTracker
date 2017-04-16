using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class ProjectChangeSubscriberFormUpdater : IProjectChangeSubscriber
    {
        private Presenter Presenter;

        public ProjectChangeSubscriberFormUpdater(Presenter presenter)
        {
            Presenter = presenter;
        }
        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.GoodMorning)
            {
                Presenter.setDate(DateTime.Now);
            }
        }
    }
}
