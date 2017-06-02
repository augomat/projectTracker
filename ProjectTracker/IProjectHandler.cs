using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    interface IProjectHandler
    {
        string currentProject { get; }
        DateTime currentProjectSince { get; }
    }
}
