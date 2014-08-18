using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Colossus
{
    public class ExperienceCrawler : IVisitContextFactory
    {
        public string StartUrl { get; set; }
        public Dictionary<string, Test> Tests { get; set; }
        public Clock Clock { get; set; }

        public ExperienceCrawler(string startUrl)
        {
            StartUrl = startUrl;
            Tests = new Dictionary<string, Test>();
        }        
        

        public VisitContext CreateContext(Visit visit)
        {
            var ctx = new WebVisitContext(this, visit);

            var wc = ctx.WebClient;
            //TODO: Fake contact / interaction properties in the root kit by means of headers.
            
            wc.DownloadString(StartUrl);
            
            

            return ctx;
        }


        IEnumerable<Test> IVisitContextFactory.Tests
        {
            get { return Tests.Values; }
        }
        
    }
}