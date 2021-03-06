﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Timetracker.KIS
{

    class KISTimesCalculator
    {
        private TimeSpan DayStartTime = new TimeSpan(7, 30, 0);
        private TimeSpan LunchTimerangeStart = new TimeSpan(11, 30, 0);
        private TimeSpan LunchTimerangeEnd = new TimeSpan(13, 30, 0);
        private TimeSpan Lunchtime = new TimeSpan(0, 30, 0);

        public const string LunchtimeProjectname = "Lunchtime";

        public KISTimesCalculator() { }

        public List<KISTime> generateKISTimes(WorktimeStatistics wts, DateTime startDay)
        {
            var totalWorkTime = wts.totalWorktime;
            var currentStartTime = startDay.Date + DayStartTime;

            groupByKisProjectsTimes(wts);

            var kisTimeSpans = generateKisTimeSpans(wts);
            quantizeKisTimeSpansToQuarter(kisTimeSpans, wts.totalWorktime);

            var kisTimes = generateKisTimes(kisTimeSpans, currentStartTime);
            kisTimes = addLunchBreak(kisTimes);

            return kisTimes;
        }

        private void groupByKisProjectsTimes(WorktimeStatistics wts)
        {
            var newRelativeProjectTimes = new Dictionary<string, float>();
            var newComments = new Dictionary<string, Dictionary<string, TimeSpan>>();

            foreach (var project in wts.relativeProjectTimes)
            {
                string projectName = project.Key;
                string kisProjectName = mapToKisProject(projectName);
                float projectFraction = project.Value;

                if (!newRelativeProjectTimes.ContainsKey(kisProjectName))
                    newRelativeProjectTimes[kisProjectName] = 0.0f;
                newRelativeProjectTimes[kisProjectName] += projectFraction;

                if (!newComments.ContainsKey(kisProjectName))
                    newComments[kisProjectName] = wts.projectComments[projectName];
                else
                {
                    foreach (var comment in wts.projectComments[projectName])
                    {
                        if (!newComments[kisProjectName].ContainsKey(comment.Key))
                            newComments[kisProjectName][comment.Key] = new TimeSpan();
                        newComments[kisProjectName][comment.Key] += comment.Value;
                    }

                }
            }

            wts.relativeProjectTimes = newRelativeProjectTimes;
            wts.projectComments = newComments;
        }

        private List<KISTimeSpan> generateKisTimeSpans(WorktimeStatistics wts)
        {
            var kisTimeSpans = new List<KISTimeSpan>();
            
            foreach (var project in wts.relativeProjectTimes)
            {
                string projectName = project.Key;
                float projectFraction = project.Value;

                TimeSpan projectSpan = TimeSpan.FromTicks(Convert.ToInt64(wts.totalWorktime.Ticks * (projectFraction / 100)));

                kisTimeSpans.Add(new KISTimeSpan(
                    projectSpan,
                    projectName,
                    wts.projectComments.ContainsKey(projectName) ? new HashSet<string>(wts.projectComments[projectName].Keys.ToArray()) : null)
                );
            }

            return kisTimeSpans;
        }

        private void quantizeKisTimeSpansToQuarter(List<KISTimeSpan> timeSpans, TimeSpan totalWorktime)
        {
            const float quantizer = 15;

            var totalWorktimeTmp = new KISTimeSpan(totalWorktime, "Fake Total timespan");
            quantize(totalWorktimeTmp, quantizer);
            var totalWorktimeTargetMins = totalWorktimeTmp.getQuantizedMinutes();

            //first round: quantize 
            //TODO LINQ
            foreach (var span in timeSpans)
            {
                quantize(span, quantizer);
            }

            //check if we reached exactly the target 
            //TODO LINQ
            var countMins = 0;
            foreach (var span in timeSpans)
            {
                countMins += span.getQuantizedMinutes();
            }
            if (countMins != totalWorktimeTargetMins)
            {

                //second round: make adaptions so we fit the target by adapting the most sensible projects (i.e. the ones with the biggest deviations)
                //TODO linq
                var errorList = new Dictionary<string, KISTimeSpan>(); // key = projectname
                foreach (var pq in timeSpans)
                    errorList.Add(pq.ProjectTrackerProject, pq);

                var errorListSorted = errorList.OrderByDescending(x => Math.Abs(x.Value.getQuantizationErrorInMins()));

                foreach (var pql in errorListSorted)
                {
                    if (countMins > totalWorktimeTargetMins)
                    {
                        errorList[pql.Key].setQuantizedMinutes(errorList[pql.Key].getQuantizedMinutes() - (int)quantizer); //TODO wäh
                        countMins -= (int)quantizer;
                    }
                    else if (countMins < totalWorktimeTargetMins)
                    {
                        errorList[pql.Key].setQuantizedMinutes(errorList[pql.Key].getQuantizedMinutes() + (int)quantizer); //TODO wäh
                        countMins += (int)quantizer;
                    }
                }
            }
        }

        private static void quantize(KISTimeSpan span, float quantizer)
        {
            if ((span.getMinutes() % quantizer) > quantizer / 2)
                span.setQuantizedMinutes((int)(Math.Ceiling(span.getMinutes() / quantizer) * quantizer));
            else
                span.setQuantizedMinutes((int)(Math.Floor(span.getMinutes() / quantizer) * quantizer));
        }

        private static List<KISTime> generateKisTimes(List<KISTimeSpan> kisTimeSpans, DateTime currentStartTime)
        {
            var kisTimes = new List<KISTime>();

            foreach (var timeSpan in kisTimeSpans)
            {
                DateTime projectEndTime = currentStartTime + timeSpan.QuantizedSpan;

                kisTimes.Add(new KISTime(
                    currentStartTime,
                    projectEndTime,
                    timeSpan.ProjectTrackerProject,
                    new TimeSpan(0, timeSpan.getQuantizationErrorInMins(), 0),
                    timeSpan.Comments)
                );

                currentStartTime = projectEndTime;
            }

            return kisTimes;
        }

        private List<KISTime> addLunchBreak(List<KISTime> kisTimes)
        {
            var lunchtimeAdded = false;
            var newTimes = new List<KISTime>();

            if (kisTimes.Count == 0)
                return kisTimes;
            
            //Check whether we actually need to introduce a lunchbreak
            var totalTimespan = kisTimes.Last().End - kisTimes.First().Start;
            var minWorktimeForLuchbreak = LunchTimerangeStart - DayStartTime; //maybe this is not actually true....
            if (totalTimespan < minWorktimeForLuchbreak)
                return kisTimes;

            //First round: Check whether any KISTime end in the desired lunchtime
            foreach (var kisTime in kisTimes)
            {
                if (!lunchtimeAdded
                    && kisTime.End.TimeOfDay >= LunchTimerangeStart
                    && kisTime.End.TimeOfDay <= LunchTimerangeEnd - Lunchtime)
                {
                    newTimes.Add(new KISTime(
                        kisTime.End,
                        kisTime.End + Lunchtime,
                        LunchtimeProjectname,
                        new TimeSpan(0, 0, 0),
                        kisTime.Comments));
                    lunchtimeAdded = true;
                }

                if (lunchtimeAdded)
                    newTimes.Add(new KISTime(
                        kisTime.Start + Lunchtime,
                        kisTime.End + Lunchtime,
                        kisTime.ProjectTrackerProject,
                        kisTime.QuantizationError,
                        kisTime.Comments));
                else
                    newTimes.Add(kisTime);
            }

            if (lunchtimeAdded)
                return newTimes;

            //Second round: Split the overlapping project
            newTimes.Clear();
            foreach (var kisTime in kisTimes)
            {
                if (!lunchtimeAdded
                    && kisTime.End.TimeOfDay >= LunchTimerangeStart
                    && kisTime.Start.TimeOfDay <= LunchTimerangeEnd - Lunchtime)
                {
                    var startDate = kisTime.Start - kisTime.Start.TimeOfDay;
                    newTimes.Add(new KISTime(
                        kisTime.Start,
                        startDate + LunchTimerangeStart,
                        kisTime.ProjectTrackerProject,
                        new TimeSpan(0, 0, 0), //quantization error is done only once, we leave it for the latter part
                        kisTime.Comments)); 
                    newTimes.Add(new KISTime(
                        startDate + LunchTimerangeStart,
                        startDate + LunchTimerangeStart + Lunchtime,
                        LunchtimeProjectname,
                        new TimeSpan(0, 0, 0)));
                    newTimes.Add(new KISTime(
                        startDate + LunchTimerangeStart + Lunchtime,
                        kisTime.End + Lunchtime,
                        kisTime.ProjectTrackerProject,
                        kisTime.QuantizationError,
                        kisTime.Comments));
                    lunchtimeAdded = true;
                }
                else
                {
                    if (lunchtimeAdded)
                        newTimes.Add(new KISTime(
                            kisTime.Start + Lunchtime,
                            kisTime.End + Lunchtime,
                            kisTime.ProjectTrackerProject,
                            kisTime.QuantizationError,
                            kisTime.Comments));
                    else
                        newTimes.Add(kisTime);
                }
            }

            if (!lunchtimeAdded)
                throw new Exception("No lunchtime could be added"); //Should not happen

            return newTimes;
        }

        private string mapToKisProject(string projectName)
        {
            // TODO: This should of course come from a mapping table, best probably a separate db-lite config
            if (projectName == "Confluence")
                return "Confluence";
            else
                return "AL";
        }
    }

    public class KISTime
    {
        public DateTime Start;
        public DateTime End;
        public TimeSpan QuantizationError;
        public String ProjectTrackerProject;
        public HashSet<string> Comments;

        public KISTime() { }

        public KISTime(DateTime start, DateTime end, String project, TimeSpan quantizationError, HashSet<string> comments = null)
        {
            Start = start;
            End = end;
            ProjectTrackerProject = project;
            QuantizationError = quantizationError;
            Comments = comments ?? new HashSet<string>();
        }
        public TimeSpan getLength()
        {
            return End - Start;
        }
    }

    public class KISTimeSpan
    {
        public TimeSpan Span;
        public TimeSpan QuantizedSpan;
        public String ProjectTrackerProject;
        public HashSet<string> Comments;

        public KISTimeSpan() { }

        public KISTimeSpan(TimeSpan span, String project, HashSet<string> comments = null)
        {
            Span = span;
            ProjectTrackerProject = project;
            Comments = comments ?? new HashSet<string>();
        }

        public int getMinutes()
        {
            return (int)Span.TotalMinutes;
        }

        public void setMinutes(int mins)
        {
            var OneMin = new TimeSpan(0, 1, 0);
            Span = TimeSpan.FromTicks(OneMin.Ticks * mins);
        }

        public int getQuantizedMinutes()
        {
            return (int)QuantizedSpan.TotalMinutes;
        }

        public void setQuantizedMinutes(int mins)
        {
            var OneMin = new TimeSpan(0, 1, 0);
            QuantizedSpan = TimeSpan.FromTicks(OneMin.Ticks * mins);
        }

        public int getQuantizationErrorInMins()
        {
            return (int)(QuantizedSpan - Span).TotalMinutes;
        }
    }
}
