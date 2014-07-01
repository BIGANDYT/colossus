﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            
            //Simple(args);
            //Online(args);
            Offline(args);
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
                output.WriteLine("Made a visit from {2} at {1} with value {0:N0}", v.Value, v.StartDate, v.Tags["Country"]);
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
                Variables.Goal(goals[2], 0.02).WhenTrue(Variables.Random<string>("Gender", new object[,]{{"Male", 0.2}, {"Female", 0.8}})), //<- That is one way to specify weights for values

                Variables.Random<string>("Gender", Sets.Weight("Male", 0.49).Weight("Female", 0.51)), // <- That is another one

                Variables.Random("Country", Sets.Exponential(new[] { "Denmark", "Australia", "Chile", "Sweden" }, 0.8, 3)), // <- 80 % of the visits will com e from the three first countries


                Variables.Year(2012, 2014).LinearTrend().Close(2014, 2), //Double as many visits will hit the site start 2014 as start of 2012                
                Variables.Hour().AddPeak(12, 1).AddPeak(20, 4), //Visits will have a sharp peak at lunch and a soft peak in the evening
                Variables.DayOfWeek().AddPeak(1, 4) //Most visits occur Monday
                );

            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany")
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When("Promo", "Variation Name").Then(Variables.Goal(goals[2], 0.4));


            var crawler = new ExperienceCrawler(testUrl) {Clock = new Clock()};

            var simulator = new VisitSimulator(Sets.Weight(baseGroup, 0.8).Weight(germans, 0.2), crawler);


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
                Variables.Goal(goals[2], 0.02).WhenTrue(Variables.Random<string>("Gender", Sets.Weight("Male", 0.2).Weight("Female", 0.8))),

                Variables.Random<string>("Gender", Sets.Weight("Male", 0.49).Weight("Female", 0.51)),   
            
                Variables.Random("Country", Sets.Exponential(new[]{"Denmark", "Brazil", "Australia", "Chile", "Sweden", "China", "Finland", "Portugal"}, 0.8, 3)),

                Variables.Year(2012, 2014).LinearTrend().Close(2014, 2), //Double as many visits will hit the site start 2014 as start of 2012                
                Variables.Hour().BaseLevel(0.2).AddPeak(12, 1, shape: 3).AddPeak(20, 4, shape: -2), //Visits will have a sharp peak at lunch and a soft peak in the evening
                Variables.DayOfWeek().AddPeak(1, 4) //Most visits occur Monday

                );

            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany"),
                Variables.Hour().AddPeak(18, 3), // All Germans visit the site around 18:00

                Variables.DayOfYear().Blend(.5).AddPeak(182.5, 100*1.5) //Germans prefer the site during summer
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When(null, "B").Then(Variables.Goal(goals[2], 0.4))
                //Germans don't like the combination A/B. The conversion rate for downloads will drop with 20%
                    .When("V1", "A").And("V2", "B").Boost(goals[0], 0.8).End();

            
            var simulator = new VisitSimulator(Sets.Weight(baseGroup, 0.8).Weight(germans, 0.2), new RoundRobinVisitContextFactory(test));


            var visits = simulator.Next(100000).ToArray();
            
            foreach (var country in visits.GroupBy(v => v.Tags["Country"]).OrderBy(g => g.Count()))
            {
                output.WriteLine("\"{0}\":", country.Key);
                Summarize(output, test, country);
                output.WriteLine();           
            }

            output.WriteLine("All visits:");
            Summarize(output, test, visits);

            var min = visits.Min(v => v.StartDate.Date);
            var max = visits.Max(v => v.StartDate.Date);

            
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using (var f = File.CreateText("Visits.txt"))
            {
                f.WriteTsvLine("Date", "Year", "Month", "Day", "DayOfWeek", "Hour", "Country", "Count");                
                foreach (var v in visits)
                {
                    var date = v.StartDate.Date;
                    f.WriteTsvLine(date.Date, date.Year, date.Month, date.Day, (int)date.DayOfWeek, date.Hour, v.Tags["Country"], 1);
                }
            }                      
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
