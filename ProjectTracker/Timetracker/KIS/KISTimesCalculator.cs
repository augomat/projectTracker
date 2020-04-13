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

        public KISTimesCalculator() { }

        public List<KISTime> generateKISTimes(WorktimeStatistics wts, DateTime startDay)
        {
            var kisTimes = new List<KISTime>();
            
            var totalWorkTime = wts.totalWorktime;
            var currentStartTime = startDay.Date + DayStartTime;

            var kisTimeSpans = generateKisTimeSpans(wts);

            quantizeKisTimesToQuarter(kisTimeSpans, wts.totalWorktime);

            foreach (var timeSpan in kisTimeSpans)
            {
                DateTime projectEndTime = currentStartTime + timeSpan.QuantizedSpan;

                //TODO add lunchbreak

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

        private void quantizeKisTimesToQuarter(List<KISTimeSpan> timeSpans, TimeSpan totalWorktime)
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
