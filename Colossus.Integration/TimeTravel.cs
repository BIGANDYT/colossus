using System;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;

namespace Colossus.Integration
{
    public class TimeTravel : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current == null) return;

            if (visitTags["StartDate"] != null)
            {
                Tracker.Current.Interaction.StartDateTime = visitTags["StartDate"].Value<DateTime>();
            }
            
            if (requestData["StartDate"] != null)
            {                
                var page = Tracker.Current.CurrentPage;
                if (page != null)
                {
                    page.DateTime = requestData.Value<DateTime>("StartDate");
                    page.Duration = (int) (requestData.Value<DateTime>("EndDate") - requestData.Value<DateTime>("StartDate")).TotalMilliseconds;
                }
                Tracker.Current.Interaction.EndDateTime = requestData.Value<DateTime>("EndDate");
            }
        }
    }
}
