using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;

namespace Colossus.Integration
{
    public class ProfileProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current != null && Tracker.Current.Interaction.PageCount == 1)
            {
                var profile = visitTags["Profile"];
                if (profile != null)
                {
                    var scores = new Dictionary<string, float>();
                    foreach (JProperty kv in profile)
                    {
                        scores.Add(kv.Name, (float) kv.Value);
                    }

                    Tracker.Current.Interaction.Profiles["Persona"].Score(scores);
                }
            }
        }
    }
}
