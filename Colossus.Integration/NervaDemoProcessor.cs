using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.Integration.NervaDemo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace Colossus.Integration
{
    public class NervaDemoProcessor : ITagDataProcessor
    {

        static ID _subchannelTemplateId = new ID("{3B4FDE65-16A8-491D-BF15-99CE83CF3506}");
        static ID _channelTemplateId = new ID("{3BA3D006-1648-4CC7-8D19-FFADFDC46D0F}");
        static ID _channelTypeTemplateId = new ID("{F13CE61A-B9F1-4573-8EC8-08CD14410BAF}");

        public void Process(JObject visitTags, JObject requestData)
        {               
            if (Tracker.Current != null && Tracker.Current.Interaction.PageCount == 1)
            {
                Tracker.Current.Contact.System.Classification = 0;
                
                var interaction = Tracker.Current.Interaction;
                interaction.TrafficType = (int?) visitTags["TrafficType"] ?? 20;
                interaction.Keywords = (string)visitTags["Keywords"] ?? "";

                var extraData = new NervaDemoMockData();
                var channelItemId = (Guid?) visitTags["ChannelItemId"];
                if (channelItemId.HasValue)
                {
                    var channelItem = Context.Database.GetItem(new ID(channelItemId.Value));

                    if (channelItem.TemplateID.Equals(_subchannelTemplateId))
                    {
                        extraData.SubChannelId = channelItem.ID.Guid;
                        channelItem = channelItem.Parent;
                    }

                    if (channelItem.TemplateID.Equals(_channelTemplateId))
                    {
                        extraData.ChannelId = channelItem.ID.Guid;
                        interaction.ChannelId = channelItem.ID.Guid;                        
                        channelItem = channelItem.Parent;
                    }

                    if (channelItem.TemplateID.Equals(_channelTypeTemplateId))
                    {
                        extraData.ChannelTypeId = channelItem.ID.Guid;                        
                    }
                }
                
                extraData.DeviceType = visitTags.Value<string>("DeviceType") ?? "Desktop";
                
                Tracker.Current.Interaction.CustomValues.Add("NervaDemo", JsonConvert.SerializeObject(extraData));

                foreach (var ev in interaction.CurrentPage.PageEvents)
                {
                    if (ev.IsGoal)
                    {
                        ev.CustomValues.Add("Goal Category", Guid.Empty);   
                    }                    
                }
            }            
        }
    }
}
