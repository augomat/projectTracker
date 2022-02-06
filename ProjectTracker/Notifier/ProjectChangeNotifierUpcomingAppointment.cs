using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ProjectTracker.Util;
using System.IO;
using System.Text.RegularExpressions;

namespace ProjectTracker
{
    class ProjectChangeNotifierUpcomingAppointment : AProjectChangeNotifier //publisher
    {
        private Presenter Presenter;

        private static int checkingIntervalInSeconds = 30;

        public ProjectChangeNotifierUpcomingAppointment(ProjectChangeHandler handler, Presenter presenter) : base(handler)
        {
            Presenter = presenter;
        }

        //-----------------------------------------------------

        // Not really a ProjectChangeNotifier, as we don't directly notify for a project change
        // Hackily abusing this as a vehicle to start a thread
        public override void start()
        {            
            while (true)
            {
                var nextAppointments = OutlookAppointmentRetriever.retrieveAppointments(DateTime.Now, DateTime.Now.AddSeconds(checkingIntervalInSeconds));
                foreach(var nextAppointment in nextAppointments)
                {
                    // Check whether the appointment falls exactly within the last time we check before it starts
                    // Not very robust but would need to store if a specific appointment was already handled which is an overkill for now
                    //TODO: Handle start of multiple appointments in the interval
                    if (DateTime.Now >= nextAppointment.Start.AddSeconds(checkingIntervalInSeconds*-1) 
                        && DateTime.Now < nextAppointment.Start)
                    {
                        System.Console.WriteLine(nextAppointment);
                        Presenter.showNotification(
                            "New upcoming appointment", 
                            nextAppointment.Comment,
                            delegate (Object o, EventArgs a)
                            {
                                Presenter.ShowDialogNewProject();
                            });
                    }
                        
                }

                System.Threading.Thread.Sleep(checkingIntervalInSeconds*1000);
            }
        }
    }
}
