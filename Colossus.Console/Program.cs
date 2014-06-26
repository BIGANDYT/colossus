using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.RandomVariables;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colossus.Console
{
    class Program
    {

        static void Main(string[] args)
        {
            Simple(args);
            //Online(args);
            //Offline(args);
        }


        static void Simple(string[] args)
        {
            var output = System.Console.Out;

            var baseUrl = "http://skynet.local";
            var testUrl = baseUrl + "/en/Solutions/Solution%201.aspx";


            var goals = new[]
            {                
                new UrlTriggeredGoal("Brochure Request", 2, baseUrl + "/en/Partners/Aoede.aspx"),
                new UrlTriggeredGoal("Newsletter Signup", 2, baseUrl + "/en/Partners/Arche.aspx"),
                new UrlTriggeredGoal("Instant Demo", 4, baseUrl + "/en/Partners/Autonoe.aspx")
            };


            var config = new VisitGroup(
                Variables.Goal(goals[0], 0.1),
                Variables.Goal(goals[1], 0.1),
                Variables.Goal(goals[2], 0.05),
                Variables.Fixed("Country", "England")

                ).When("Promo", "Original").Then(
                    Variables.Goal(goals[0], 0.5)
                );

            

            var simpleSim = new VisitSimulator(config, new ExperienceCrawler(testUrl));            

            var visits = new List<Visit>();
            foreach (var v in simpleSim.Next(100))
            {
                output.WriteLine("Made a visit from {1} with value {0:N0}", v.Value, v.Tags["Country"]);
                visits.Add(v);
            }

            output.WriteLine();
            output.WriteLine("Summary:");

            Summarize(output, simpleSim.ContextFactory.Tests.First(), visits);
            
        }
        
        static void Online(string[] args)
        {                       
            var output = System.Console.Out;

            var baseUrl = "http://skynet.local";
            var testUrl = baseUrl + "/en/Solutions/Solution%201.aspx";


            var goals = new[]
            {
                
                new UrlTriggeredGoal("Brochure Request", 2, baseUrl + "/en/Partners/Aoede.aspx"),
                new UrlTriggeredGoal("Newsletter Signup", 2, baseUrl + "/en/Partners/Arche.aspx"),
                new UrlTriggeredGoal("Instant Demo", 4, baseUrl + "/en/Partners/Autonoe.aspx")
            };

            //Define base conversion rates and tags
            var baseGroup = new VisitGroup(
                Variables.Goal(goals[0], 0.1)
                    .Correlate(goals[1], 0.3, 0.5), //When goal 1 happens goal 2 is more likely to happen

                //For some reason the buying visits are more likely to be women
                Variables.Goal(goals[2], 0.02).WhenTrue(Variables.Weighted("Gender", new Dictionary<string, double> { { "Female", 0.8 }, { "Male", 0.2 } })),

                Variables.Weighted("Gender", new Dictionary<string, double> { { "Male", 0.49 }, { "Female", 0.51 } }),
                Variables.Weighted("Country", new Dictionary<string, double> { { "Denmark", 0.7 }, { "Australia", 0.2 }, { "Chile", 0.05 }, { "Sweden", 0.05 } })
                );

            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany")
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When("Promo", "Variation Name").Then(Variables.Goal(goals[2], 0.4));


            var crawler = new ExperienceCrawler(testUrl) {Clock = new Clock()};

            var simulator = new VisitSimulator(new Dictionary<VisitGroup, double>
            {
                {baseGroup, 0.8}, //80 % of the visits will be from the base group
                {germans, 0.2}
            }, crawler);


            var visits = new List<Visit>();

            foreach (var v in simulator.Next(100))
            {                
                output.Write(".");
                visits.Add(v);
            }

            output.WriteLine();

            var test = crawler.Tests.Values.First();
            foreach (var country in visits.GroupBy(v => v.Tags["Country"]).OrderBy(g => g.Key))
            {
                output.WriteLine("\"{0}\":", country.Key);
                Summarize(output, test, country);
                output.WriteLine();
            }

            output.WriteLine("All visits:");
            Summarize(output, test, visits);
        }

        static void Offline(string[] args)
        {

            var output = System.Console.Out;

            
            var factors = new[] {CreateExperienceFactor("V1", 2), CreateExperienceFactor("V2", 5)};

            var test = Test.FromFactors(factors);            
            test.Name = "An MV test";

            var goals = new[]
            {
                new Goal("Brochure A", 4),
                new Goal("Brochure B", 2),
                new Goal("Bought something", 10)
            };


            //Define base conversion rates and tags
            var baseGroup = new VisitGroup(
                Variables.Goal(goals[0], 0.1)
                    .Correlate(goals[1], 0.3, 0.5), //When goal 1 happens goal 2 is more likely to happen

                    //For some reason the buying visits are more likely to be women
                Variables.Goal(goals[2], 0.02).WhenTrue(Variables.Weighted("Gender", new Dictionary<string, double> { { "Female", 0.8 }, { "Male", 0.2 } })),

                Variables.Weighted("Gender", new Dictionary<string, double>{{"Male", 0.49}, {"Female", 0.51}}),
                Variables.Weighted("Country", new Dictionary<string, double>{{"Denmark", 0.7}, {"Australia", 0.2}, {"Chile", 0.05}, {"Sweden", 0.05}})
                );

            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany")
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When(null, "B").Then(Variables.Goal(goals[2], 0.4))
                //Germans don't like the combination A/B. The conversion rate for downloads will drop with 20%
                    .When("V1", "A").And("V2", "B").Boost(goals[0], 0.8).End();

            var simulator = new VisitSimulator(new Dictionary<VisitGroup, double>
            {
                {baseGroup, 0.8}, //80 % of the visits will be from the base group
                {germans, 0.2}
            }, new RoundRobinVisitContextFactory(test));


            var visits = simulator.Next(5000).ToArray();
            
            foreach (var country in visits.GroupBy(v => v.Tags["Country"]).OrderBy(g => g.Key))
            {
                output.WriteLine("\"{0}\":", country.Key);
                Summarize(output, test, country);
                output.WriteLine();           
            }

            output.WriteLine("All visits:");
            Summarize(output, test, visits);            
        }

        static void Summarize(TextWriter output, Test test, IEnumerable<Visit> visits)
        {
            output.WriteLine("{0, -30} {1,8} {2,8}", "", "Count", "VpV");
            output.WriteLine(string.Join("", Enumerable.Repeat("-", 48)));
            foreach (var exp in visits.GroupBy(v => v.Experiences[test]).OrderBy(e => e.Key.Number))
            {
                output.WriteLine("{0,-30} {1,8:N0} {2,8:N2}", exp.Key, exp.Count(), exp.Average(v=>v.Value));
            }
            output.WriteLine(string.Join("", Enumerable.Repeat("-", 48)));
            output.WriteLine("{0, -30} {1,8:N0} {2,8:N2}", "", visits.Count(), visits.Average(v=>v.Value));
        }



        static PersonalizationRule CreatePersonalizationRule(string name)
        {
            return new PersonalizationRule()
            {
                Index = _nextIndex++,
                Name = name,
                Levels = new[] { name + "_No", name + "_Yes" }
            };
        }

        private static int _nextIndex = 0;
        static ExperienceFactor CreateExperienceFactor(string name, int variationCount)
        {
            return new ExperienceFactor
            {
                Index = _nextIndex++,
                Name = name,
                Levels = Enumerable.Range(0, variationCount).Select(i => ""+(char)(65 + i)).ToArray()
            };
        }        

    }
}
