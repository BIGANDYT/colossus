using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Diagnostics.PerformanceCounters;
using Sitecore.Analytics.Testing;
using Sitecore.Layouts;
using Sitecore.Pipelines.RenderLayout;
using Sitecore.Shell.Applications.MarketingAutomation.Extensions;

namespace Colossus.Integration
{
    public class Interceptor : RenderLayoutProcessor
    {
        public static List<ITagDataProcessor> TagProcessors { get; private set; }        

        static Interceptor()
        {
            TagProcessors = new List<ITagDataProcessor>();

            TagProcessors.Add(new WhoIsProcessor());
            TagProcessors.Add(new TimeTravel());
        }


        public override void Process(RenderLayoutArgs args)
        {

            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return;
            } 

            //Ensure that it's only the simulator console that can access this information
            //Other people shouldn't know what we're testing.            
            var secret = Sitecore.Configuration.Settings.GetSetting("ColossusSecret");

            if (!string.IsNullOrEmpty(secret) && httpContext.Request.Headers["X-Colossus-Secret"] != secret)
            {
                return;
            }


            var visitTags = ParseHeaderValue(httpContext.Request.Headers["X-Colossus-Visit"]);
            var requestData = ParseHeaderValue(httpContext.Request.Headers["X-Colossus-Request"]);
            if (visitTags != null || requestData != null)
            {                
                foreach (var proc in TagProcessors)
                {
                    proc.Process(visitTags ?? new JObject(), requestData ?? new JObject());
                }
                httpContext.Response.Headers.Add("X-Colossus-Processing", "OK");
            }



            var page = Context.Page;
            if (page != null && page.Renderings != null)
            {
                var renderings = new List<Dictionary<string, string>>();

                foreach (RenderingReference r in page.Renderings)
                {
                    var item = r.RenderingItem;
                    if (item != null && item.InnerItem != null)
                    {
                        renderings.Add(item.InnerItem.Fields.ToDictionary(f => f.Name, f => f.Value));
                    }
                }

                httpContext.Response.Headers.Add("X-Colossus-Renderings", renderings.ToJson());
            }

            if (Tracker.Current != null && Tracker.Current.CurrentPage != null)
            {
                var tc = Tracker.Current.CurrentPage.GetTestCombination();
                if (tc != null)
                {                    
                    httpContext.Response.Headers.Add("X-Colossus-TestSet", tc.Testset.ToJson());
                    httpContext.Response.Headers.Add("X-Colossus-Experience",
                        Enumerable.Range(0, tc.Testset.Variables.Count).Select(i => tc[i]).ToArray().ToJson());
                }
            }

            //Tracker.Current.StartTracking();
            //var p = Tracker.Current.Interaction.GetOrCreateCurrentPage();
            //p.Duration = 1337;
            
            //HttpContext.Current.Response.Write(AnalyticsCount.CollectionRobotRequests.Value);            

        }

        JObject ParseHeaderValue(string value)
        {
            return value == null ? null : JsonConvert.DeserializeObject(value) as JObject;
        }
    }   
}
