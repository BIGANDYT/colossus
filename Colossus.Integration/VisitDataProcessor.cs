using Sitecore.Analytics;
using Sitecore.Analytics.Model;

namespace Colossus.Integration
{
    public class WhoIsProcessor: ITagDataProcessor
    {
        public void Process(dynamic visitTags, dynamic requestData)
        {
            if (Tracker.Current == null) return;

            if (visitTags.IP == null)
            {
                var whois = new WhoIsInformation {Country = visitTags.Country, City = visitTags.City};
                Tracker.Current.Interaction.SetGeoData(whois);
            }            
        }
    }
}
