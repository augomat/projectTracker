﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;

namespace ProjectTracker.Util
{
	/// <summary>
	/// Description of OutlookAppointmentRetriever.
	/// </summary>
	public class OutlookAppointmentRetriever
	{
		public static List<WorktimeRecord> retrieveAppointments(DateTime from, DateTime to)
		{
		    var oApp = new Microsoft.Office.Interop.Outlook.Application();
			Outlook.Folder calFolder = oApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar) as Outlook.Folder;
		    Outlook.Items rangeAppts = GetAppointmentsInRange(calFolder, from, to);

            var ret = new List<WorktimeRecord>();
		    if (rangeAppts != null)
		    {
		        foreach (Outlook.AppointmentItem appt in rangeAppts)
		        {
                    //MessageBox.Show("Subject: " + appt.Subject + " Start: " + appt.Start.ToString("g"));
                    ret.Add(new WorktimeRecord(appt.Start, appt.End, "[unknown]", appt.Subject));
                }
		    }
            return ret;
		}
		
		private static Outlook.Items GetAppointmentsInRange(Outlook.Folder folder, DateTime startTime, DateTime endTime)
		{
            //string filter = "[Start] >= '" + startTime.ToString("g") + "' AND [End] <= '" + endTime.ToString("g") + "'";
            string filter = "[Start] <= '" + endTime.ToString("g") + "' AND [End] >= '" + startTime.ToString("g") + "'"; //also consider overlapping appointments

            try
            {
		        Outlook.Items calItems = folder.Items;
		        calItems.IncludeRecurrences = true;
		        calItems.Sort("[Start]", Type.Missing);
		        Outlook.Items restrictItems = calItems.Restrict(filter);
		        if (restrictItems.Count > 0)
		        {
		            return restrictItems;
		        }
		        else
		        {
		            return null;
		        }
		    }
		    catch
            {
                return null;
            }
		}

	}
	
}