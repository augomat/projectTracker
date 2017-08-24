﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using ProjectTracker;

namespace ProjectTrackerTests
{
    [TestClass]
    public class OvertimeUndertime
    {
        [TestMethod]
        public void WorkOvertime1Project()
        {
            var mainHandler = new ProjectChangeHandler();
            var projectCorrectionHandler = new ProjectChangeNotifierCorrection(mainHandler);
            var wtanalyzer = new WorktimeAnalyzer(new WorktimeRecordStorageInMemory(), mainHandler, projectCorrectionHandler);
            var obj = new PrivateObject(wtanalyzer);

            var origOvertime = ProjectTracker.Properties.Settings.Default.maxWorktime;
            var availableProject = ProjectTracker.Properties.Settings.Default.AvailableProjects;

            if (availableProject.Count < 2)
                throw new Exception("Project must have at least 2 available projects for this test to run");

            try
            {
                ProjectTracker.Properties.Settings.Default.maxWorktime = "08:00";

                var data = new WorktimeStatistics();
                data.projectTimes = new System.Collections.Generic.Dictionary<string, TimeSpan>
                {
                    {availableProject[0], new TimeSpan(9,0,0)},
                    {availableProject[1], new TimeSpan(1,0,0)}
                };
                data.totalTime = new TimeSpan(10, 0, 0);
                data.totalWorktime = new TimeSpan(10, 0, 0);
                data.totalProjectTime = new TimeSpan(10, 0, 0);

                var expectedStats = new WorktimeStatistics();
                expectedStats.projectTimes = new System.Collections.Generic.Dictionary<string, TimeSpan>
                {
                    {availableProject[0], new TimeSpan(7,0,0)},
                    {availableProject[1], new TimeSpan(1,0,0)}
                };
                expectedStats.relativeProjectTimes = new System.Collections.Generic.Dictionary<string, float>
                {
                    {availableProject[0], 87.5f},
                    {availableProject[1], 12.5f}
                };
                expectedStats.totalTime = new TimeSpan(8, 0, 0);
                expectedStats.totalWorktime = new TimeSpan(8, 0, 0);
                expectedStats.totalProjectTime = new TimeSpan(8, 0, 0);

                var expectedOvertime = new Dictionary<string, TimeSpan> { { availableProject[0], new TimeSpan(2, 0, 0) } };

                object[] args = new object[] { data };
                var ret = (Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>>)obj.Invoke("calculateOvertimeForTesting", args);
                var retStats = ret.Item1;
                var retOvertime = ret.Item2;

                CollectionComparer.AssertDictionaryEqual(retStats.projectTimes, expectedStats.projectTimes);
                CollectionComparer.AssertDictionaryEqual(retStats.relativeProjectTimes, expectedStats.relativeProjectTimes);
                Assert.AreEqual(expectedStats.totalTime, retStats.totalTime);
                Assert.AreEqual(expectedStats.totalWorktime, retStats.totalWorktime);
                Assert.AreEqual(expectedStats.totalProjectTime, retStats.totalProjectTime);
                CollectionComparer.AssertDictionaryEqual(retOvertime, expectedOvertime);

            }
            finally
            {
                ProjectTracker.Properties.Settings.Default.maxWorktime = origOvertime;
            }
        }

        [TestMethod]
        public void WorkOvertime2Project()
        {
            var mainHandler = new ProjectChangeHandler();
            var projectCorrectionHandler = new ProjectChangeNotifierCorrection(mainHandler);
            var wtanalyzer = new WorktimeAnalyzer(new WorktimeRecordStorageInMemory(), mainHandler, projectCorrectionHandler);
            var obj = new PrivateObject(wtanalyzer);

            var origOvertime = ProjectTracker.Properties.Settings.Default.maxWorktime;
            var availableProject = ProjectTracker.Properties.Settings.Default.AvailableProjects;

            if (availableProject.Count < 2)
                throw new Exception("Project must have at least 2 available projects for this test to run");

            try
            {
                ProjectTracker.Properties.Settings.Default.maxWorktime = "04:00";

                var data = new WorktimeStatistics();
                data.projectTimes = new System.Collections.Generic.Dictionary<string, TimeSpan>
                {
                    {availableProject[0], new TimeSpan(2,0,0)},
                    {availableProject[1], new TimeSpan(6,0,0)}
                };
                data.totalTime = new TimeSpan(8, 0, 0);
                data.totalWorktime = new TimeSpan(8, 0, 0);
                data.totalProjectTime = new TimeSpan(8, 0, 0);

                var expectedStats = new WorktimeStatistics();
                expectedStats.projectTimes = new System.Collections.Generic.Dictionary<string, TimeSpan>
                {
                    {availableProject[0], new TimeSpan(0,0,0)},
                    {availableProject[1], new TimeSpan(4,0,0)}
                };
                expectedStats.relativeProjectTimes = new System.Collections.Generic.Dictionary<string, float>
                {
                    {availableProject[0], 0f},
                    {availableProject[1], 100f}
                };
                expectedStats.totalTime = new TimeSpan(4, 0, 0);
                expectedStats.totalWorktime = new TimeSpan(4, 0, 0);
                expectedStats.totalProjectTime = new TimeSpan(4, 0, 0);

                var expectedOvertime = new Dictionary<string, TimeSpan> {
                    { availableProject[0], new TimeSpan(2, 0, 0) },
                    { availableProject[1], new TimeSpan(2, 0, 0) }
                };

                object[] args = new object[] { data };
                var ret = (Tuple<WorktimeStatistics, Dictionary<string, TimeSpan>>)obj.Invoke("calculateOvertimeForTesting", args);
                var retStats = ret.Item1;
                var retOvertime = ret.Item2;

                CollectionComparer.AssertDictionaryEqual(retStats.projectTimes, expectedStats.projectTimes);
                CollectionComparer.AssertDictionaryEqual(retStats.relativeProjectTimes, expectedStats.relativeProjectTimes);
                Assert.AreEqual(expectedStats.totalTime, retStats.totalTime);
                Assert.AreEqual(expectedStats.totalWorktime, retStats.totalWorktime);
                Assert.AreEqual(expectedStats.totalProjectTime, retStats.totalProjectTime);
                CollectionComparer.AssertDictionaryEqual(retOvertime, expectedOvertime);
            }
            finally
            {
                ProjectTracker.Properties.Settings.Default.maxWorktime = origOvertime;
            }
        }
    }
}
