using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colossus
{
    public class ExperienceCrawler : IExperienceDistributor
    {
        public Test Test { get; set; }

        public ExperienceFactor[] Factors { get; set; }

        public ExperienceCrawler(Test test)
        {
            Test = test;
        }

        private bool _initialized;
        void Initialize(WebClient wc)
        {
            if (_initialized) return;

            var testset = JsonConvert.DeserializeObject(wc.ResponseHeaders["X-TestSet"]) as JObject;
            Factors = testset["Variables"].Select((v, i) => new ExperienceFactor
            {
                Index = i,
                Name = (string)v["Label"],
                Levels = v["Values"].Select(val => (string)val["Label"]).ToArray()
            }).ToArray();

            Test = Test.FromFactors(Factors);            

            _initialized = true;
        }

        public ExperienceContext GetNext(Visit visit)
        {
            var wc = new CookieAwareWebClient();

            //TODO: Fake contact / interaction properties in the root kit by means of headers.

            wc.Headers["X-VisitTags"] = JsonConvert.SerializeObject(visit.Tags);
            wc.DownloadString(Test.Url);

            Initialize(wc);

            var indexes = JsonConvert.DeserializeObject<int[]>(wc.ResponseHeaders["X-Experience"]);

            //var vars = indexes.Select((level, i) => new FactorLevel(Factors[i], level, true)).ToArray(); 

            var vars = indexes.ToDictionary((level, i) => Factors[i], (level, i) => level);

            return new ExperienceContext {Experience = Test.GetExperience(vars), VisitContext = new CrawlingVisitContext(wc)};
        }

        class CrawlingVisitContext : VisitContext
        {
            private readonly WebClient _webClient;

            public CrawlingVisitContext(WebClient webClient)
            {
                _webClient = webClient;
            }

            public override void Commit()
            {                
                //This can be made more "realistic"
                //Task: Given a set of goals, find a realistic visit path that converts those.
                //Maybe, include duration. And shortest path.
                foreach (var goal in Visit.Goals)
                {
                    _webClient.Headers["X-VisitTags"] = JsonConvert.SerializeObject(Visit.Tags);
                    goal.Convert("", "", _webClient);                    
                }
                base.Commit();
            }
        }
    }
}