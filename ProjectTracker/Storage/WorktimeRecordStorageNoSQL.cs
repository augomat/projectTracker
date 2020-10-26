using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace ProjectTracker
{
    public class WorktimeRecordStorageNoSQL : IWorktimeRecordStorage
    {
        private readonly string DATABASE_FILE = @"data.db";

        public void handleProjectChangeEvent(ProjectChangeEvent projectChangeEvent)
        {
            if (projectChangeEvent == null)
                return;

            foreach (var wtr in projectChangeEvent.WorktimeRecords)
            {
                addWorktimeRecord(wtr);
            }
        }

        public void addWorktimeRecord(WorktimeRecord worktimeRecord)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                wtrs.Insert(worktimeRecord);
            }
        }

        public List<WorktimeRecord> getAllWorktimeRecords(DateTime from, DateTime to)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                return wtrs.Find(wtr => (wtr.Start >= from && wtr.End <= to))
                            .OrderBy(wtr => wtr.Start)
                            .OrderBy(wtr => wtr.End)
                            .ToList();
            }
        } 

        public void ChangeProjectComment(int id, string newProjectComment)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var current = wtrs.Find(wtr2 => wtr2.storageID == id).FirstOrDefault();

                if (current == null)
                    throw new Exception("This WorktimeRecord does not exist");

                current.Comment = newProjectComment;
                wtrs.Update(current);
            }
        }

        public void ChangeProjectName(int id, string newProjectName)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var current = wtrs.Find(wtr2 => wtr2.storageID == id).FirstOrDefault();

                if (current == null)
                    throw new Exception("This WorktimeRecord does not exist");

                current.ProjectName = newProjectName;
                wtrs.Update(current);
            }
        }

        public void ChangeStartTime(int id, DateTime newStartDate)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var current = wtrs.Find(wtr2 => wtr2.storageID == id).FirstOrDefault();

                if (current.Start.Date != newStartDate.Date)
                    throw new Exception("Only times (not dates, i.e. overnighters) can be changed");

                if (newStartDate > current.End)
                    throw new Exception("Begin time cannot be greater than end time");

                if (newStartDate.TimeOfDay > current.Start.TimeOfDay) //shortening
                {
                    var newWtr = new WorktimeRecord(current.Start, newStartDate, "undefined", current.Comment);
                    wtrs.Insert(newWtr);

                    current.Start = newStartDate;
                    wtrs.Update(current);
                }
                else //lengthening
                {
                    if (newStartDate.Hour < 4)
                        throw new Exception("Day must not start before 4am");

                    var previous = wtrs.Find(wtr => wtr.storageID != id
                        && wtr.End <= current.Start
                        && wtr.End >= current.Start.Date) //weird same day conditions because comparing .Date for some reason doesn't work
                        .OrderByDescending(wtr => wtr.End)
                        .FirstOrDefault();

                    if (previous != null)
                    {
                        if (previous.End > newStartDate)
                            throw new Exception("Lengthening only allowed in gaps");
                    }                    

                    current.Start = newStartDate;
                    wtrs.Update(current);
                }
            }
        }

        public void ChangeEndTime(int id, DateTime newEndDate)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var current = wtrs.Find(wtr2 => wtr2.storageID == id).FirstOrDefault();

                if (current == null)
                    throw new Exception("This WorktimeRecord does not exist");

                if (current.End.Date != newEndDate.Date)
                    throw new Exception("Only times (not dates, i.e. overnighters) can be changed");

                if (newEndDate < current.Start)
                    throw new Exception("Begin time cannot be greater than end time");

                if (newEndDate.TimeOfDay < current.End.TimeOfDay) //shortening
                {
                    var newWtr = new WorktimeRecord(newEndDate, current.End, "undefined", current.Comment);
                    wtrs.Insert(newWtr);

                    current.End = newEndDate;
                    wtrs.Update(current);
                }
                else //lengthening
                {
                    //TODO support overnighters

                    var nextDay = current.End.Date.Add(new TimeSpan(1, 0, 0, 0));
                    var next = wtrs.Find(wtr => wtr.storageID != id 
                        && wtr.Start >= current.End
                        && wtr.Start < nextDay) //weird same day conditions because comparing .Date for some reason doesn't work
                        .OrderBy(wtr => wtr.Start)
                        .FirstOrDefault();

                    if (next != null)
                    {
                        if (next.Start < newEndDate)
                            throw new Exception("Lengthening only allowed in gaps");
                    }

                    current.End = newEndDate;
                    wtrs.Update(current);
                }
            }          
        }

        public Dictionary<string, TimeSpan> getOvertimes()
        {
            Dictionary<string, TimeSpan> overtimeDict = new Dictionary<string, TimeSpan>();
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var currentOvertimes = db.GetCollection<OvertimeEntity>("overtimes");
                foreach(var ov in currentOvertimes.Find(ovl => 1 == 1).ToList())
                {
                    overtimeDict[ov.Id] = ov.Time;
                }
            }
            return overtimeDict;
        }

        public void updateOvertimes(Dictionary<string, TimeSpan> overtimes)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var currentOvertimes = db.GetCollection<OvertimeEntity>("overtimes");

                for (var overtimeIndex = 0; overtimeIndex < overtimes.Keys.Count; overtimeIndex++)
                {
                    var overtimePjName = overtimes.Keys.ToArray().ElementAt(overtimeIndex);
                    var overtimePjTime = overtimes.Values.ToArray().ElementAt(overtimeIndex);

                    var currentOv = currentOvertimes.Find(ov => ov.Id == overtimePjName).FirstOrDefault();
                    if (currentOv != null)
                    {
                        currentOv.Time = overtimePjTime;
                        currentOvertimes.Update(currentOv);
                    }
                    else
                    {
                        var newOv = new OvertimeEntity();
                        newOv.Id = overtimePjName;
                        newOv.Time = overtimePjTime;
                        currentOvertimes.Insert(newOv);
                    }
                }
            }
        }

        public List<String> getRecentProjects()
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var thirtyDaysAgo = DateTime.Now.Add(new TimeSpan(-30, 0, 0, 0));

                return wtrs
                    .Find(wtr => wtr.Start >= thirtyDaysAgo)
                    .Select(wtr => wtr.ProjectName)
                    .Where(name => !String.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();
            }
        }

        public List<String> getRecentComments(string projectName)
        {
            using (var db = new LiteDatabase(DATABASE_FILE))
            {
                var wtrs = db.GetCollection<WorktimeRecord>("worktimeRecords");
                var thirtyDaysAgo = DateTime.Now.Add(new TimeSpan(-30, 0, 0, 0));

                return wtrs
                    .Find(wtr => wtr.Start >= thirtyDaysAgo
                        && wtr.ProjectName == projectName)
                    .Select(wtr => wtr.Comment)
                    .Where(name => !String.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();
            }
        }
    }

    public class OvertimeEntity
    {
        [BsonId]
        public string Id { get; set; }
        public TimeSpan Time { get; set; }
    }
}
