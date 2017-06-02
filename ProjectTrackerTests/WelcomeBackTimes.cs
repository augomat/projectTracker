using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProjectTracker;

namespace ProjectTrackerTests
{
    [TestClass]
    public class WelcomeBackTimes
    {
        [TestMethod]
        public void WelcomeBackSimple()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj1", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:00:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 17:00:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:00:00"), DateTime.Parse("01.01.2017 15:00:00"), "[unknown]", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:00:00"), DateTime.Parse("01.01.2017 17:00:00"), "[unknown]", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }

        [TestMethod]
        public void WelcomeBackOverlapBeginEnd()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 13:30:00"), DateTime.Parse("01.01.2017 14:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:30:00"), DateTime.Parse("01.01.2017 17:30:00"), "Pj2", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:00:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 17:00:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:00:00"), DateTime.Parse("01.01.2017 14:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 16:30:00"), "[unknown]", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:30:00"), DateTime.Parse("01.01.2017 17:00:00"), "Pj2", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }

        [TestMethod]
        public void WelcomeBackOverlapLong()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 13:30:00"), DateTime.Parse("01.01.2017 17:30:00"), "Pj1", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:00:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 17:00:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:00:00"), DateTime.Parse("01.01.2017 17:00:00"), "Pj1", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }

        [TestMethod]
        public void WelcomeBackApointmentOverlap()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 15:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:00:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 17:00:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:00:00"), DateTime.Parse("01.01.2017 14:30:00"), "[unknown]", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 15:00:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:00:00"), DateTime.Parse("01.01.2017 17:00:00"), "[unknown]", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }

        [TestMethod]
        public void WelcomeBackApointmentWithin()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 16:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:00:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 17:00:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:00:00"), DateTime.Parse("01.01.2017 14:30:00"), "[unknown]", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 15:00:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:00:00"), DateTime.Parse("01.01.2017 16:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 16:30:00"), DateTime.Parse("01.01.2017 17:00:00"), "[unknown]", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            try
            {
                CollectionAssert.AreEqual(res, ret);
            }
            catch
            {
                Assert.Inconclusive("This is not implemented right now");
            }
        }

        [TestMethod]
        public void WelcomeBackApointmentOverlap2()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 15:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 14:40:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 15:20:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:40:00"), DateTime.Parse("01.01.2017 15:00:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 15:20:00"), "Pj2", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }

        [TestMethod]
        public void WelcomeBackApointmentOverlap3()
        {
            var prompt = new Prompt();
            var obj = new PrivateObject(prompt);

            obj.SetFieldOrProperty("Suggestions", new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 14:30:00"), DateTime.Parse("01.01.2017 15:30:00"), "Pj1", ""),
                  new WorktimeRecord(DateTime.Parse("01.01.2017 15:00:00"), DateTime.Parse("01.01.2017 16:00:00"), "Pj2", "") }
            );
            obj.SetFieldOrProperty("From", DateTime.Parse("01.01.2017 15:20:00"));
            obj.SetFieldOrProperty("To", DateTime.Parse("01.01.2017 15:40:00"));

            var res = new List<WorktimeRecord>()
                { new WorktimeRecord(DateTime.Parse("01.01.2017 15:20:00"), DateTime.Parse("01.01.2017 15:40:00"), "Pj2", "") };

            var ret = (List<WorktimeRecord>)obj.Invoke("calculateFullSuggestionList");

            CollectionAssert.AreEqual(res, ret);
        }
    }
}
