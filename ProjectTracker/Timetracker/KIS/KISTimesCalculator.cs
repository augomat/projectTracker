using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Timetracker.KIS
{

    class KISTimesCalculator
    {
        public TimeSpan DayStartTime = new TimeSpan(8, 0, 0);
        public TimeSpan LunchTimerangeStart = new TimeSpan(11, 30, 0);
        public TimeSpan LunchTimerangeEnd = new TimeSpan(13, 30, 0);
        public TimeSpan Lunchtime = new TimeSpan(0, 30, 0);

        public KISTimesCalculator() { }

        public List<KISTime> generateKISTimes(WorktimeStatistics wts, DateTime startDay)
        {
            var totalWorkTime = wts.totalWorktime;
            var currentStartTime = startDay.Date + DayStartTime;

            var kisTimeSpans = generateKisTimeSpans(wts);
            quantizeKisTimeSpansToQuarter(kisTimeSpans, wts.totalWorktime);

            var kisTimes = generateKisTimes(kisTimeSpans, currentStartTime);
            kisTimes = addLunchBreak(kisTimes);

            return kisTimes;
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
                    new TimeSpan(0, timeSpan.getQuantizationErrorInMins(), 0))
                    );

                currentStartTime = projectEndTime;
            }

            return kisTimes;
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
                    projectName)
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

        private List<KISTime> addLunchBreak(List<KISTime> kisTimes)
        {
            var lunchtimeAdded = false;
            var newTimes = new List<KISTime>();

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
                        "Lunchtime",
                        new TimeSpan(0, 0, 0)));
                    lunchtimeAdded = true;
                }

                if (lunchtimeAdded)
                    newTimes.Add(new KISTime(
                        kisTime.Start + Lunchtime,
                        kisTime.End + Lunchtime,
                        kisTime.ProjectTrackerProject,
                        kisTime.QuantizationError));
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
                        new TimeSpan(0, 0, 0))); //quantization error is done only once, we leave it for the latter part
                    newTimes.Add(new KISTime(
                        startDate + LunchTimerangeStart,
                        startDate + LunchTimerangeStart + Lunchtime,
                        "Lunchtime",
                        new TimeSpan(0, 0, 0)));
                    newTimes.Add(new KISTime(
                        startDate + LunchTimerangeStart + Lunchtime,
                        kisTime.End + Lunchtime,
                        kisTime.ProjectTrackerProject,
                        kisTime.QuantizationError));
                    lunchtimeAdded = true;
                }
                else
                {
                    if (lunchtimeAdded)
                        newTimes.Add(new KISTime(
                            kisTime.Start + Lunchtime,
                            kisTime.End + Lunchtime,
                            kisTime.ProjectTrackerProject,
                            kisTime.QuantizationError));
                    else
                        newTimes.Add(kisTime);
                }
            }

            if (!lunchtimeAdded)
                throw new Exception("No lunchtime could be added"); //Should not happen

            return newTimes;
        }
    }

    public class KISTime
    {
        public DateTime Start;
        public DateTime End;
        public TimeSpan QuantizationError;
        public String ProjectTrackerProject;        

        public KISTime() { }

        public KISTime(DateTime start, DateTime end, String project, TimeSpan quantizationError)
        {
            Start = start;
            End = end;
            ProjectTrackerProject = project;
            QuantizationError = quantizationError;
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

        public KISTimeSpan() { }

        public KISTimeSpan(TimeSpan span, String project)
        {
            Span = span;
            ProjectTrackerProject = project;
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
