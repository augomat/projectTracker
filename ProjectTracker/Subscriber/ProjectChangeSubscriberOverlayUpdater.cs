using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTracker
{
    class ProjectChangeSubscriberOverlayUpdater : IProjectChangeSubscriber
    {
        public delegate void UpdateOverlay(string text);
        private UpdateOverlay ShowOverlayText;

        public ProjectChangeSubscriberOverlayUpdater(UpdateOverlay showOverlayText)
        {
            ShowOverlayText = showOverlayText;
        }

        public void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent)
        {
            if (!projectChangeEvent.Processed)
                return;

            ShowOverlayText(projectChangeEvent.NewProject);
        }
    }
}
