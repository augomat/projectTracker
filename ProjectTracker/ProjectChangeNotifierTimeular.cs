using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProjectTracker
{
    class ProjectChangeNotifierTimeular : ProjectChangeNotifier
    {
        public ProjectChangeNotifierTimeular(ProjectChangeHandler handler) : base(handler)
        {
        }

        public override void start()
        {
            throw new NotImplementedException();
        }
    }
}
