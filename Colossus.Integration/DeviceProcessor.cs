using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;

namespace Colossus.Integration
{
    public class DeviceProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current != null)
            {
                var cid = (string)visitTags["Device"];

                if (cid != null)
                {
                    Tracker.Current.Interaction.DeviceId = new Guid(cid);
                }
            }
        }
    }
}
