using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Tracking;

namespace Colossus.Integration
{
    class OfficecoreProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current != null && Tracker.Current.Interaction.PageCount == 1)
            {
                // Simple function to see whether user should identify. 90% chance to identify
                var id = new Random(Guid.NewGuid().GetHashCode());
                if (id.NextDouble() <= 0.9)
                {
                    // Existing contacts are identified with numbers between 1-1000 (including 1000)
                    var random = new Random(Guid.NewGuid().GetHashCode());
                    Tracker.Current.Session.Identify(random.Next(1, 1000).ToString()); // = Guid.Parse(cid);
                }
            }
        }
    }
}
