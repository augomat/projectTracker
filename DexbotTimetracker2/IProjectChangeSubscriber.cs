﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    interface IProjectChangeSubscriber
    {
        void handleProjectChangeEvent(object sender, ProjectChangeEvent projectChangeEvent);
    }
}