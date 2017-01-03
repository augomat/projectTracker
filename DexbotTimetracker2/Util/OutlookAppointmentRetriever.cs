/*
 * Created by SharpDevelop.
 * User: Georg
 * Date: 03.12.2016
 * Time: 02:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;

namespace ProjectTracker
{
	/// <summary>
	/// Description of OutlookAppointmentRetriever.
	/// </summary>
	public class OutlookAppointmentRetriever
	{
        private DataGridView dataGrid;

		public OutlookAppointmentRetriever(DataGridView dataGridView)
		{
            dataGrid = dataGridView;
		}
		
		public void retrieveAppointments(DateTime startTime)
		{
		    var oApp = new Microsoft.Office.Interop.Outlook.Application();
			Outlook.Folder calFolder = oApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar)
		        as Outlook.Folder;
			DateTime start = startTime.Date;
		    DateTime end = start.AddDays(1);
		    Outlook.Items rangeAppts = GetAppointmentsInRange(calFolder, start, end);
		    if (rangeAppts != null)
		    {
                dataGrid.Rows.Clear();

		        foreach (Outlook.AppointmentItem appt in rangeAppts)
		        {
                    //MessageBox.Show("Subject: " + appt.Subject + " Start: " + appt.Start.ToString("g"));

                    dataGrid.Rows.Add();
                    int RowIndex = dataGrid.RowCount - 1;
                    DataGridViewRow R = dataGrid.Rows[RowIndex];

                    R.Cells["Date"].Value = appt.Start.ToString("dd.MM.yyyy");
                    R.Cells["StartTime"].Value = appt.Start.ToString("HH:mm");
                    R.Cells["EndTime"].Value = appt.End.ToString("HH:mm");
                    R.Cells["DiffSecs"].Value = (appt.End - appt.Start).TotalSeconds;
                    R.Cells["Comment"].Value = appt.Subject;
                }

                dataGrid.Refresh();
		    }
		    
		}
		
		private Outlook.Items GetAppointmentsInRange(Outlook.Folder folder, DateTime startTime, DateTime endTime)
		{
		    string filter = "[Start] >= '"
		        + startTime.ToString("g")
		        + "' AND [End] <= '"
		        + endTime.ToString("g") + "'";
		    //MessageBox.Show(filter);
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
		    catch { return null; }
		}

	}
	
}
