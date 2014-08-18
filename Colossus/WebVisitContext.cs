using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colossus
{
    public class WebVisitContext : VisitContext
    {
        public Clock Clock { get; set; }        

        public VisitWebClient WebClient { get; set; }

        public WebVisitContext(IVisitContextFactory factory, Visit visit)
            : base(factory, visit)
        {
            Clock = new Clock();            
            WebClient = new VisitWebClient(this);
            RequestData = new Dictionary<string, object>();
        }

        public override void VisitPage(VisitPage page)
        {
            base.VisitPage(page);
            if (Clock != null)
            {
                Clock.Update(this, page.Duration);
            }

            page.Execute(WebClient);
        }

        public void PrepareRequest(WebRequest request)
        {            
            request.Headers.Add("X-Colossus-Visit", JsonConvert.SerializeObject(Visit.Tags));
            request.Headers.Add("X-Colossus-Request", JsonConvert.SerializeObject(RequestData));

            (request as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";
        }


        public void ParseResponse(WebResponse response)
        {
            var test = ParseTest(response);            
            if (test != null)
            {
                var indexes = JsonConvert.DeserializeObject<int[]>(response.Headers["X-Colossus-Experience"]);

                var vars = indexes.ToDictionary((level, i) => test.Factors[i], (level, i) => level);

                Visit.UpdateState(test.GetExperience(vars));
            }            
        }


        Test ParseTest(WebResponse wc)
        {
            var crawler = Factory as ExperienceCrawler;
            
            Test test = null;

            var testsetHeader = wc.Headers["X-Colossus-TestSet"];
            var testset = string.IsNullOrEmpty(testsetHeader)
                ? null
                : JsonConvert.DeserializeObject(testsetHeader) as JObject;

            
            if (testset != null)
            {
                var id = (string)testset["Id"];
                if (crawler == null || !crawler.Tests.TryGetValue(id, out test))
                {
                    var factors = testset["Variables"].Select((v, i) => new ExperienceFactor
                    {
                        Index = i,
                        Name = (string)v["Label"],
                        Levels = v["Values"].Select(val => (string)val["Label"]).ToArray()
                    }).ToArray();

                    test = Test.FromFactors(factors);

                    if (crawler != null)
                    {
                        crawler.Tests.Add(id, test);
                    }
                }
            }

            return test;
        }
    }
}