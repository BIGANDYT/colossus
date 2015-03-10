using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Diagnostics.PerformanceCounters;
using Sitecore.Analytics.Testing;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Links;
using Sitecore.Mvc.Extensions;
using Sitecore.Pipelines.RenderLayout;
using Sitecore.Shell.Applications.MarketingAutomation.Extensions;

namespace Colossus.Integration
{
    public class Interceptor : RenderLayoutProcessor
    {
        public static List<ITagDataProcessor> TagProcessors { get; private set; }        

        public Interceptor()
        {
            TagProcessors = new List<ITagDataProcessor>();
            

            TagProcessors.Add(new DeviceProcessor());
            TagProcessors.Add(new WhoIsProcessor());
            TagProcessors.Add(new TimeTravel());
            TagProcessors.Add(new ProfileProcessor());
            TagProcessors.Add(new CampaignProcessor());
            TagProcessors.Add(new NervaDemoProcessor());
            TagProcessors.Add(new OfficecoreProcessor());
            TagProcessors.Add(new GoalProcessor());
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
            

            if (httpContext.Request.Headers["X-Colossus-Map"] == "true")
            {                               
                httpContext.Response.AddHeader("X-Colossus-Map",  Map(Context.Item, new XDocument()).ToString());
            }

            var mapId = httpContext.Request.QueryString["colossus-map"];
            if (!string.IsNullOrEmpty(mapId))
            {                
                var item = Context.Database.GetItem(ID.Parse(mapId));
                var map = Map(item, new XDocument());
                httpContext.Response.AddHeader("X-Colossus-Map", map.ToString());

                if (Tracker.Current != null)
                {                    
                    Switcher<ITracker, TrackerSwitcher>.Exit();
                }
                httpContext.Response.ContentType = "text/xml";
                httpContext.Response.Write(map.ToString());
                httpContext.Response.End();
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

            //if (Tracker.Current != null)
            //{
            //    foreach (var p in Tracker.Current.Interaction.Profiles.GetProfileNames())
            //    {
            //        if (Tracker.Current.Interaction.Profiles[p].PatternLabel == "Pat the Potential Traveller")
            //        {
            //            throw new Exception("Traveller");
            //        }
            //        HttpContext.Current.Response.Write(Tracker.Current.Interaction.Profiles[p].PatternLabel + "<br />");
            //    }
            //}

            //Tracker.Current.StartTracking();
            //var p = Tracker.Current.Interaction.GetOrCreateCurrentPage();
            //p.Duration = 1337;
            
            //HttpContext.Current.Response.Write(AnalyticsCount.CollectionRobotRequests.Value);            

        }

        static string DecodeHeaderValue(string value)
        {
            return Encoding.UTF7.GetString(Encoding.GetEncoding(1252).GetBytes(value));
        }

        XContainer Map(Item item, XContainer target)
        {
            var options = UrlOptions.DefaultOptions;
            options.AlwaysIncludeServerUrl = true;
            options.LanguageEmbedding = LanguageEmbedding.Always;

            var xitem = new XElement("Item",
                    new XAttribute("Id", item.ID.ToGuid()),
                    new XAttribute("TemplateId", item.TemplateID.ToGuid()),
                    new XAttribute("HRef",
                        LinkManager.GetItemUrl(item, options)));
            target.Add(xitem);

            foreach (Item child in item.Children)
            {                
                Map(child, xitem);                
            }

            return target;
        }

        JObject ParseHeaderValue(string value)
        {
            return value == null ? null : JsonConvert.DeserializeObject(DecodeHeaderValue(value)) as JObject;
        }
    }   
}
