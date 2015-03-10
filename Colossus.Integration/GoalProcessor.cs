using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Data.Items;

namespace Colossus.Integration
{
    public class GoalProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current != null)
            {
                // Goal tag will look as follows: goalid|url
                var tags = (string)visitTags["Goals"];
                if (!string.IsNullOrWhiteSpace(tags))
                {
                    var goals = tags.Split('|');

                    // Surely there's a better way of doing this...?
                    if (Tracker.Current.Interaction.CurrentPage.Url.Path.Equals(goals[1], StringComparison.InvariantCultureIgnoreCase))
                    {
                        var goalItem = Sitecore.Context.Database.GetItem(goals[0]);
                        if (goalItem != null)
                        {
                            var x = Tracker.Current.Interaction.CurrentPage.Register(new PageEventItem(goalItem));
                        }
                    }
                }
            }
        }
    }
}
