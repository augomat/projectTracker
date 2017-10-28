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
        //private string apiKey = "NzQwN182ZjI4ZDc4ZmRiMWU0ODRiYjVkY2M5MDdjOWViY2I3ZA==";
        //private string apiSecret = "MGVlMmJjMzhjNDFmNDRhZmJjMjkyNzUzZjJkODk1MjY=";
        private string apiKey { get { return Properties.Settings.Default.timeularAPIkey; } }
        private string apiSecret { get { return Properties.Settings.Default.timeularAPIsecret; } }

        private Presenter Presenter;
        private HttpClient httpClient;

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

        private class Credentials
        {
            public string apiKey { get; set; }
            public string apiSecret { get; set; }
        }

        private class Token
        {
            public string token { get; set; }
        }

        public ProjectChangeNotifierTimeular(ProjectChangeHandler handler, Presenter presenter) : base(handler)
        {
            Presenter = presenter;
        }

        public override void start()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(URL);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            getToken();
            
            while (true)
            {
                try
                {
                    var response = httpClient.GetAsync("tracking").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsAsync<RootObject>().Result;
                        var currentTracking = data.currentTracking;

                        if (currentTracking == null)
                        {
                            System.Threading.Thread.Sleep(5000);
                            continue;
                        }
                           
                        if (currentTracking.activity.name != Handler.currentProject
                            && Handler.currentProject != "[unknown]") //a bit hacky: don't fire if screen is e.g. locked
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
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        getToken();
                    }
                }
                catch { } 
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void getToken()
        {
            var credentials = new Credentials();
            credentials.apiKey = apiKey;
            credentials.apiSecret = apiSecret;

            var response = httpClient.PostAsJsonAsync<Credentials>("developer/sign-in", credentials).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsAsync<Token>().Result;
                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + data.token);
                Presenter.setNotifierState("Timeular", true);
            }
            else
                Presenter.setNotifierState("Timeular", false);
        }
    }
}
