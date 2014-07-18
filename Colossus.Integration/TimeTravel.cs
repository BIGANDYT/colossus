using System;
using Sitecore.Analytics;

namespace Colossus.Integration
{
    public class TimeTravel : ITagDataProcessor
    {
        public void Process(dynamic visitTags, dynamic requestData)
        {
            if (Tracker.Current == null) return;

            if (requestData.StartDate != null)
            {
                Tracker.Current.Interaction.StartDateTime = requestData.StartDate;
                Tracker.Current.Interaction.EndDateTime = requestData.EndDate;

                var page = Tracker.Current.CurrentPage;
                if (page != null)
                {
                    page.DateTime = requestData.StartDate;
                    page.Duration = (int) ((DateTime)requestData.EndDate - (DateTime)requestData.StartDate).TotalSeconds;
                }
            }
        }
    }
}
