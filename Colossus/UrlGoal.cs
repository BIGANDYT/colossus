using System.Net;

namespace Colossus
{
    public class UrlGoal : Goal
    {
        private readonly string _conversionUrl;

        public UrlGoal(string conversionUrl)
        {
            _conversionUrl = conversionUrl;
        }


        public override GoalState GetState(string currentUrl, string pageHtml)
        {
            return currentUrl.Equals(_conversionUrl) ? GoalState.Triggered : GoalState.Unavailable;
        }

        public override void Convert(string currentUrl, string pageHtml, WebClient wc)
        {
            wc.DownloadString(_conversionUrl);
        }
    }
}