using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
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

        Test ParseTest(WebClient wc)
        {
            Test test = null;
            var testset = JsonConvert.DeserializeObject(wc.ResponseHeaders["X-Colossus-TestSet"]) as JObject;
            if (testset != null)
            {
                var id = (string) testset["Id"];
                if (!Tests.TryGetValue(id, out test))
                {
                    var factors = testset["Variables"].Select((v, i) => new ExperienceFactor
                    {
                        Index = i,
                        Name = (string) v["Label"],
                        Levels = v["Values"].Select(val => (string) val["Label"]).ToArray()
                    }).ToArray();

                    test = Test.FromFactors(factors);                    
                    Tests.Add(id, test);
                }
            }

            return test;
        }
        

        public VisitContext CreateContext(Visit visit)
        {
            var ctx = new WebVisitContext(visit, Clock);

            var wc = ctx.WebClient;
            //TODO: Fake contact / interaction properties in the root kit by means of headers.
            
            wc.DownloadString(StartUrl);
            var test = ParseTest(wc);
                       
            if (test!= null)
            {
                var indexes = JsonConvert.DeserializeObject<int[]>(wc.ResponseHeaders["X-Colossus-Experience"]);

                var vars = indexes.ToDictionary((level, i) => test.Factors[i], (level, i) => level);

                ctx.Visit.UpdateState(test.GetExperience(vars));
            }

            return ctx;
        }


        IEnumerable<Test> IVisitContextFactory.Tests
        {
            get { return Tests.Values; }
        }
        
    }

    public class WebVisitContext : VisitContext
    {
        public Clock Clock { get; set; }        

        public VisitWebClient WebClient { get; set; }        

        public WebVisitContext(Visit visit, Clock clock) : base(visit)
        {
            Clock = clock;            
            WebClient = new VisitWebClient(this);
            RequestData = new Dictionary<string, object>();
        }

        public void PrepareRequest(WebRequest request)
        {
            if (Clock != null)
            {
                Clock.Update(this);
            }

            request.Headers.Add("X-Colossus-Visit", JsonConvert.SerializeObject(Visit.Tags));
            request.Headers.Add("X-Colossus-Request", JsonConvert.SerializeObject(RequestData));
        }       
    }
}