using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    public interface IProjectHandler
    {
        string currentProject { get; }
        DateTime currentProjectSince { get; }
        string currentProjectComment { get; set; }

        List<string> getAvailableProjects();
        int getAvailableProjectIndex(string projectName);
        List<string> getSuggestedComments(string projectName);
    }
}
