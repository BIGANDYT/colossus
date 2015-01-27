using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class UrlTriggeredGoal : Goal
    {
        public string ConversionUrl { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }


        public Func<Visit, IEnumerable<KeyValuePair<string, string>>> UrlArguments { get; set; }


        public UrlTriggeredGoal(string name, int value, string conversionUrl)
            : base(name, value)
        {
            ConversionUrl = conversionUrl;
        }


        private string GetUrl(Visit visit)
        {
            var url = ConversionUrl;
            if (UrlArguments != null)
            {
                var args = UrlArguments(visit);
                var q = url.IndexOf('?');
                url += (q != -1 && q != url.Length - 1 ? "&" : "") + args;
            }

            return url;
        }
        
        public override void Convert(VisitContext visitContext)
        {
            var wc = visitContext.WebClient;
            if (wc != null)
            {
                wc.DownloadString(ConversionUrl);                
            }
            //visitContext.Visit.Pages.Add(new VisitPage
            //{
            //    Url = GetUrl(visitContext.Visit),
            //    Duration = TimeSpan.FromSeconds(0.1)
            //});
        }
    }
}