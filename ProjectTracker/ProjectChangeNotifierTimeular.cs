using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProjectTracker
{
    class ProjectChangeNotifierTimeular : ProjectChangeNotifier
    {
        private const string URL = "https://api.timeular.com/api/v1/";
        private string currentToken = "eyJhbGciOiJIUzUxMiJ9.eyJ0eXBlIjoidXNlciIsInN1YiI6Ijc0MDcifQ.RJft2lh4LJTMz7xuX5fdg3CbxsYF_dvn76AMyIE4uaPlvUHXV-LJ3bF6Blfndes_ZKx7SJFpTMcLD5bINmWaDQ";

        private Presenter Presenter;

        private class Activity
        {
            public string id { get; set; }
            public string name { get; set; }
            public string color { get; set; }
            public string integration { get; set; }
        }

        private class CurrentTracking
        {
            public Activity activity { get; set; }
            public DateTime startedAt { get; set; }
            public object note { get; set; }
        }

        private class RootObject
        {
            public CurrentTracking currentTracking { get; set; }
        }

        public ProjectChangeNotifierTimeular(ProjectChangeHandler handler, Presenter presenter) : base(handler)
        {
            Presenter = presenter;
        }

        public override void start()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL + "tracking");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + currentToken);

            while (true)
            {
                var response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsAsync<RootObject>().Result;
                    var currentTracking = data.currentTracking;

                    if (currentTracking == null)
                        continue;

                    if (currentTracking.activity.name != Handler.currentProject)
                    {
                        if (!string.IsNullOrEmpty(Handler.currentProject)) //TODO - should that really be decided here?
                        {
                            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                                ProjectChangeEvent.Types.Change,
                                currentTracking.activity.name,
                                "Tracking Change detected",
                                new WorktimeRecord(
                                    new DateTime(Handler.currentProjectSince.Ticks),
                                    DateTime.Now,
                                    Handler.currentProject,
                                    "")
                                )
                            );
                        }
                        else
                        {
                            OnRaiseProjectChangeEvent(new ProjectChangeEvent(
                                ProjectChangeEvent.Types.Init,
                                currentTracking.activity.name,
                                "Tracking Change detected",
                                new WorktimeRecord(
                                    DateTime.Now,
                                    DateTime.Now,
                                    currentTracking.activity.name,
                                    "Application started")
                                )
                            );
                        }
                        Presenter.refreshGridExternal(); //TODO - should that really happen here???
                    }
                        
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
