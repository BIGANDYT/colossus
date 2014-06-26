using System;
using System.Collections.Generic;

namespace Colossus
{
    public class UrlTriggeredGoal : Goal
    {
        public string ConversionUrl { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public Func<WebVisitContext, string> PostDataShaper { get; set; }

        public Func<WebVisitContext, IEnumerable<KeyValuePair<string, string>>> UrlArguments { get; set; }


        public UrlTriggeredGoal(string name, int value, string conversionUrl)
            : base(name, value)
        {
            ConversionUrl = conversionUrl;            
        }


        public override GoalState GetState(VisitContext visit)
        {
            return GoalState.Available;            
            //return currentUrl.Equals(_conversionUrl) ? GoalState.Triggered : GoalState.Unavailable;
        }

        public override void Convert(VisitContext visit)
        {
            var wc = visit as WebVisitContext;            
            if (wc != null)
            {
                var url = ConversionUrl;
                if (UrlArguments != null)
                {
                    var args = UrlArguments(wc);
                    var q = url.IndexOf('?');
                    url += (q != -1 && q != url.Length - 1 ? "&" : "") + args;
                }

                var client = wc.WebClient;
                if (PostDataShaper != null)
                {
                    client.Headers.Add("Content-Type", ContentType);
                    try
                    {
                        client.UploadString(url, Method, PostDataShaper(wc));
                    }
                    finally
                    {
                        client.Headers.Remove("Content-Type");
                    }
                }
                else
                {
                    client.DownloadString(url);
                }
            }
        }        
    }
}