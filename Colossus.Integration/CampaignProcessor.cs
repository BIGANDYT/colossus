using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;

namespace Colossus.Integration
{
   public class CampaignProcessor : ITagDataProcessor
    {
       public void Process(JObject visitTags, JObject requestData)
       {
           if (Tracker.Current != null && Tracker.Current.Interaction.PageCount == 1)
           {
               var cid = (string)visitTags["Campaign"];
               if (!string.IsNullOrEmpty(cid))
               {
                   Tracker.Current.Interaction.CampaignId = Guid.Parse(cid);
               }
           }
       }
    }
}
