using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProjectTracker;
using WorkTracker;

namespace ProjectTrackerTests
{
    [TestClass]
    public class WTQuantizerTests
    {
        private Project pj1 = new Project("Pj1", "Pj1", false);
        private Project pj2 = new Project("Pj2", "Pj2", false);
        private Project pj3 = new Project("Pj3", "Pj3", false);
        private Project pj4 = new Project("Pj4", "Pj4", false);

        [TestMethod]
        public void WTQuantizerSimple()
        {
            var wtupdater = new WorktrackerUpdater();
            var obj = new PrivateObject(wtupdater);

            Dictionary<Project, float> wtprojects = new Dictionary<Project, float>
            {
                { pj1, 10f },
                { pj2, 10f },
                { pj3, 10f },
                { pj4, 70f },
            };
            Dictionary<Project, int> wtresults = new Dictionary<Project, int>
            {
                { pj1, 10 },
                { pj2, 10 },
                { pj3, 10 },
                { pj4, 70 },
            };

            var ret = (Dictionary<Project, int>)obj.Invoke("quantizeProjectsTo5", wtprojects);
            DictionaryComparer.AssertEqual(ret, wtresults);
        }

        [TestMethod]
        public void WTQuantizerSimple2()
        {
            var wtupdater = new WorktrackerUpdater();
            var obj = new PrivateObject(wtupdater);

            Dictionary<Project, float> wtprojects = new Dictionary<Project, float>
            {
                { pj1, 18f },
                { pj2, 12f },
                { pj3, 15f },
                { pj4, 55f },
            };
            Dictionary<Project, int> wtresults = new Dictionary<Project, int>
            {
                { pj1, 20 },
                { pj2, 10 },
                { pj3, 15 },
                { pj4, 55 },
            };

            var ret = (Dictionary<Project, int>)obj.Invoke("quantizeProjectsTo5", wtprojects);
            DictionaryComparer.AssertEqual(ret, wtresults);
        }

        [TestMethod]
        public void WTQuantizerNo100Percent()
        {
            var wtupdater = new WorktrackerUpdater();
            var obj = new PrivateObject(wtupdater);

            Dictionary<Project, float> wtprojects = new Dictionary<Project, float>
            {
                { pj1, 2f },
                { pj2, 96f },
                { pj3, 1f },
                { pj4, 1f },
            };
            Dictionary<Project, int> wtresults = new Dictionary<Project, int>
            {
                { pj1, 5 },
                { pj2, 95 },
                { pj3, 0 },
                { pj4, 0 },
            };

            var ret = (Dictionary<Project, int>)obj.Invoke("quantizeProjectsTo5", wtprojects);
            DictionaryComparer.AssertEqual(ret, wtresults);
        }
    }
}
