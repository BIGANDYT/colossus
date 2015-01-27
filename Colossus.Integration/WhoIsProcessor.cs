using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;

namespace Colossus.Integration
{
    public class WhoIsProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current == null) return;

            var language = visitTags.Value<string>("Language");
            if (!string.IsNullOrEmpty(language))
            {
                Tracker.Current.Interaction.Language = language;
            }


            var city = visitTags.Value<JObject>("City");
            if (city != null)
            {
                var interaction = Tracker.Current.Interaction;
                var whois = new WhoIsInformation
                {
                    BusinessName = "",
                    City = (string)city["Name"] ?? "",
                    Country = (string)city["Country"] ?? "",
                    Region = (string)city["Adm1"] ?? "",
                    Latitude = (double)city["Latitude"],
                    Longitude = (double)city["Longitude"]
                };
                interaction.SetGeoData(whois);
                interaction.UpdateLocationReference();

            }
            else if (visitTags["IP"] == null)
            {
                var whois = new WhoIsInformation
                {
                    BusinessName = "",
                    Country = visitTags.Value<string>("Country") ?? "",
                    City = visitTags.Value<string>("City") ?? ""
                };

                Tracker.Current.Interaction.SetGeoData(whois);
                Tracker.Current.Interaction.UpdateLocationReference();
            }
            else
            {
                var ip = visitTags.Value<string>("IP");
                if (ip != null)
                {
                    Tracker.Current.Interaction.Ip = ip.Split('.').Select(byte.Parse).ToArray();
                }
            }
        }
    }
}
