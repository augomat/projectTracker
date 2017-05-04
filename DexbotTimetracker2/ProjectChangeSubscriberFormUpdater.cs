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
            if (projectChangeEvent.Type == ProjectChangeEvent.Types.Start)
            {
                //Hack: There is no hook after the storage engines get called, thus let's just wait a bit and then refresh
                Task.Delay(200).ContinueWith(t => { try { Presenter.Form.Invoke(new Action(Presenter.refreshGrid)); } catch { } });
            }
        }
    }
}
