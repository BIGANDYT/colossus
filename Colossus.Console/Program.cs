using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Colossus.Geo;
using Colossus.Pages;
using Colossus.RandomVariables;
using Colossus.Symposium;
using FiftyOne.Foundation.Mobile.Detection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colossus.Console
{
    class Program
    {

        static void Main(string[] args)
        {
           // Simple(args);
            //Online(args);
            //Offline(args);

            //Skynet();
            // NervaHeineken();
                        NervaDemo();
        }

        static void Skynet()
        {
            var output = System.Console.Out;


            var baseUrl = "http://skynet.local";
            //This is the URL for a page that contains a test.
            var testUrl = baseUrl + "/";


            //These are the conversions the simulated visitors might make.
            //The URLs points to pages that will trigger a goal.
            var goals = new[]
            {                
                new UrlTriggeredGoal("Goal 1", 2, baseUrl + "/Goal 1"),
                new UrlTriggeredGoal("Goal 2", 4, baseUrl + "/Goal 2"),
                //new UrlTriggeredGoal("Instant Demo", 4, baseUrl + "/en/Partners/Autonoe.aspx")                
            };


            var config = new VisitGroup(
                //These are the "normal" conversion rates (i.e. regardless of test experience)
                Variables.Goal(goals[0], 0.1),
                Variables.Goal(goals[1], 0.1)
                //Variables.Goal(goals[2], 0.05)

                ).When("Page Version", "Home - Version 2").Then(
                //When the component called "Promo" has the value "Original", the conversion rate for goal 1 will increase.
                //Experiences containing Promo->Original will be winners
                    Variables.Goal(goals[0], 0.5)
                ).Actions(new PageAction(testUrl));



            output.WriteLine("Making requests for {0}", testUrl);
            output.WriteLine();
            var simpleSim = new VisitSimulator(config, new ExperienceCrawler());
            var visits = new List<Visit>();
            foreach (var v in simpleSim.Next(50))
            {
                output.Write(".");
                visits.Add(v);
            }

            output.WriteLine();
            output.WriteLine();

            var test = simpleSim.ContextFactory.Tests.FirstOrDefault();
            if (test != null)
            {
                foreach (var v in visits)
                {
                    output.WriteLine("Made a visit for experience #{1} with value {0:N0}", v.Value,
                        v.Experiences[test].Number);
                }

                output.WriteLine();
                output.WriteLine("Summary:");

                Summarize(output, simpleSim.ContextFactory.Tests.First(), visits);
            }

            output.WriteLine();
            output.WriteLine("Done.");   
        }


        static void NervaHeineken()
        {
            //var url = "http://nervaheineken.local/";
            var url = "http://pocweb1dk1.pocdomain.scua:1864/";


            SymposiumData.Url = url;
            int simulated = 1;

            var threads = 6;
            int visitsPerThread = 10000;

            var channels = XDocument.Load(url + "?colossus-map={1EB725EC-BD0A-4B45-9166-9C09BADEDB2C}");
            var subChannelIds =
                channels.Root.Elements("Item")
                    .Elements("Item")
                    .Elements("Item")
                    .Select(item => (Guid)item.Attribute("Id")).ToArray();



            //var set = Sets.Exponential(SymposiumData.Pages.Values
            //    .OrderBy(p => p.Value).ThenByDescending(p => p.Rank).ToArray(), .8, 4);

            //var r = Randomness.Random;
            //foreach (var pg in Enumerable.Range(0, 2000).Select(i => set.Sample()).GroupBy(g => g.Url)
            //    .OrderByDescending(g=>g.Count()))
            //{
            //    System.Console.Out.WriteLine("{0}: {1}", pg.Key, pg.Count());
            //}
           
            //return;



            //var pages = XDocument.Load(url + "?colossus-map={2A2E1561-5C1A-4FEE-8E08-434C9CA0F676}").Root
            //    .Elements("Item").Select(p => (string)p.Attribute("HRef")).ToArray();

            //throw new Exception(string.Join(", ", pages));



            var citites =
                Sets.Exponential(
                GetCities().OrderByDescending(c => c.Population * (c.Country == "RU" || c.Country=="IN" ? 0.2 : 1)).ToArray(),
                    0.8, 200);

            Randomness.Seed = new Random().Next(0, 10000);

            var visits = Enumerable.Range(0, threads).AsParallel().WithDegreeOfParallelism(threads).Select(ix =>
            {

                var countries = Sets.Exponential(new[]
                {
                    "Denmark", "France", "Netherlands", "United Kingdom", "Australia", "New Zealand", "Japan", "Anguilla", "Antigua and Barbuda", "Aruba", "Bahamas", "Barbados", "Bonaire", "British Virgin Islands", "Cayman Islands", "Cuba", "Curacao", "Dominica", "Dominican Republic", "Grenada", "Guadeloupe", "Haiti", "Jamaica", "Martinique", "Montserrat", "Puerto Rico", "St Barths", "St Kitts and Nevis", "St Lucia", "St Maarten and St Martin", "St Vincent and the Grenadines", "Trinidad and Tobago", "Turks and Caicos Islands", "US Virgin Islands", "Ukraine"
                }, .8, 7);

                var searchPatterns = new DistributedSampleSet<string>(new[]
                {
                    "Dutch beer in *",                    
                    "Heineken in *",
                    "Heineken"
                });

                var startYear = 2014d;
                var endYear = startYear + new DateTime(2014, 9, 18).DayOfYear / 365d;
                var duration = endYear - startYear;

                var random = Randomness.Random;

                var basePageSet = Sets.Exponential(SymposiumData.Pages.Values
                    .OrderBy(p => p.Value).ThenByDescending(p => p.Rank).Select(p => p.Action).ToArray(), .8, 4);

                //var landingPages =
                //    Sets.Exponential(
                //        new[]
                //        {
                //            "en/Landing Pages/Free Companion Offer",
                //            "en/Landing Pages/Jetstream Contest Signup",
                //            "en/Landing Pages/Like Us",
                //            "en/Landing Pages/Ski Hire",
                //            "en/Landing Pages/Ski Lessons",
                //            "en/Landing Pages/Ski Pass",
                //            "en/Landing Pages/Skis and Boots"
                //        }.Select(pageUrl => new PageAction(url + pageUrl.ToLower()))
                //            .ToArray(), .3, 4);

                var baseGroup =
                    new VisitGroup(

                        //Variables.Random("Country", Sets.Weight("Denmark", 0.8).Weight("UK", 0.2).Build()),
                        Variables.Random("City", citites),
                        Variables.Random("Language",
                            Sets.Exponential(new[] {"en", "es", "fr", "nl", "zh", "de", "da", "sv", "ru"}, 0.85, 4)),
                        //Variables.Random("Profile", () => new Dictionary<string, double>
                        //{
                        //{"cecile", random.NextDouble()},    
                        //{ "chris", random.NextDouble()},
                        //{ "ian", random.NextDouble()},
                        //{ "sandra", random.NextDouble()}
                        //}),
                        Variables.Random("ChannelItemId", Sets.Uniform(subChannelIds)),
                        Variables.Random("DeviceType", Sets.Weight("Desktop", 0.3).Weight("Tablet", 0.2).Weight("Mobile", 0.5).Build()),
                        Variables.Year(startYear, endYear).LinearTrend(0.5, 1),
                        Variables.DayOfWeek().Weight(Sets
                            .Weight(0, .5)
                            .Weight(1, 1)
                            .Weight(2, 1)
                            .Weight(3, 1)
                            .Weight(4, 1)
                            .Weight(5, 1)
                            .Weight(6, .5).Build()
                            ))
                    //.Actions(new PageAction { Url=url})


                        .Actions(
                        new VisitActionSet(basePageSet)
                        {
                            Length = new RandomExponential(1/2d)
                        })
                        //new VisitActionList
                        //{
                        //    Actions = new VisitAction[]
                        //    {
                        //        new VisitActionSet(Sets.Weight<VisitAction>(
                        //                new PageAction(url), 1
                        //            ).Weight(new VisitActionSet(landingPages), .5).Build()),                                
                        //        new RandomWalk
                        //            {
                        //                MapUrl = url + "?colossus-map={D2FAC701-3E22-4421-8270-1D46FF8517F1}",
                        //                //BaseUrl = url,
                        //                StartPage = () => new PageAction(url + "en/Plan%20And%20Book/Destinations"),
                        //                Length = new RandomExponential(1 / 5d)
                        //            },
                        //    },
                        //    Length = Sets.Weight(1, 0.18).Weight(2, .82).Build()
                        //}                        
                    //.Actions(new SiteActionList
                    //{
                    //    Actions = Enumerable.Repeat(new PageAction
                    //        {
                    //            Url = url,
                    //            QueryStringParameters = new Dictionary<string, Func<string>>()
                    //    {
                    //        {"test", ()=>"Lipsum" + Randomness.Random.Next(1, 100)}
                    //    }
                    //        }, 10),
                    //    Length = new RandomExponential(1/3d)
                    //})
                    .DefaultDuration(new TruncatedRandom(new RandomSkewNormal(60, 30, 3), 5, 1000))
                    ;


                var keywordVisitors = new VisitGroup(
                    Variables.Random("Keywords", () =>
                    {
                        return searchPatterns.Sample().Replace("*", countries.Sample());
                    }),
                    Variables.Random("TrafficType", Sets.Weight(10, 0.9).Weight(15, 0.1).Build()))
                    .Override(baseGroup);


                var campaignGroups = new List<VisitGroup>();


                //{7B2ECBA5-59DC-4635-A9AF-2299E776102B} CRM merchandise
                campaignGroups.Add(new VisitGroup(
                    Variables.Year(startYear, endYear).LinearTrend(0.2, 6, 0.95)
                        .AddPeak(startYear + duration * .2, duration * 0.03, 1, 0.05),
                    Variables.Fixed("Campaign", SymposiumData.Campaigns["Bing"])
                    ));

                //{FFF73FDB-9696-447B-B461-7CA7BACB8564} Summer party promo
                
                campaignGroups.Add(new VisitGroup(
                    Variables.Year(startYear, endYear)//.LinearTrend(2, 1),
                    .AddPeak(startYear + duration * .7, duration * 0.02, -2)
                    .AddPeak(startYear + duration * .3, duration * 0.05, 1.5, 0.3),
                    Variables.Random("Language",
                                        Sets.Exponential(new[] { "en", "es", "nl", "da" }, 0.85, 3)),
                        Variables.Fixed("Campaign", SymposiumData.Campaigns["Facebook"])
                    ));

                campaignGroups.Add(new VisitGroup(
                  Variables.Year(startYear, endYear).LinearTrend(2, 1),
                    //.AddPeak(startYear + duration * .7, duration * 0.05, -2)
                    //.AddPeak(startYear + duration * .4, duration * 0.2, 1.5, 0.2),
                  Variables.Fixed("Campaign", SymposiumData.Campaigns["Google"])
                  ));

                campaignGroups.Add(new VisitGroup(
                    Variables.Year(startYear, endYear)//.LinearTrend(2, 1),
                    .AddPeak(startYear + duration * .4, duration * 0.2, 1),
                    Variables.Fixed("Campaign", SymposiumData.Campaigns["Twitter"])
                    ));

                ////Summer seeker promo
                //campaignGroups.Add(new VisitGroup(
                //    Variables.Year(startYear, endYear).LinearTrend(0.2, 4, 0.4)
                //        .AddPeak(startYear + duration * .3, duration * 0.1, 1),
                //    Variables.Fixed("Campaign", "{FFF73FDB-9696-447B-B461-7CA7BACB8564}")
                //    ));

                ////Skiing
                //campaignGroups.Add(new VisitGroup(
                //    Variables.Year(startYear, endYear).LinearTrend(2, 1),
                //    Variables.Fixed("Campaign", "{7B2ECBA5-59DC-4635-A9AF-2299E776102B}")
                //    ));



                ////Home for holidays
                //campaignGroups.Add(new VisitGroup(Variables.Year(startYear, endYear)
                //    .AddPeak(startYear + duration * .2, duration * 0.04, -2),
                //    Variables.Fixed("Campaign", "{8063A2D9-2C82-466D-9E34-6675D880D578}")));

                ////Frequent flyer promotion
                //campaignGroups.Add(new VisitGroup(
                //    Variables.Fixed("Campaign", "{F5061EFE-4E79-47FC-8871-7C641C37256C}"),
                //    Variables.Year(startYear, endYear).AddPeak(startYear + duration * .7, duration * .03, 3)));

                ////Facebook
                //campaignGroups.Add(new VisitGroup(
                //    Variables.Year(startYear, endYear).AddPeak(startYear + duration * .5, duration * .1, 1),
                //    Variables.Fixed("Campaign", "{C0374204-6ABF-4082-BF14-95F578337C4B}")));

                ////campaignGroups.Add(new VisitGroup(Variables.Year(startYear, endYear).AddPeak(startYear + duration * .7, duration * 0.1, 1),
                ////    Variables.Fixed("Campaign", "{FFF73FDB-9696-447B-B461-7CA7BACB8564}")));


                var campaignVisitors = new VisitGroup(
                    Variables.Random("TrafficType", Sets.Weight(10, 0.33).Weight(15, 0.67).Build())).Override(baseGroup)
                    .Actions(new HeinekenWalk());



                foreach (var cg in campaignGroups) cg.Override(campaignVisitors);

                var legendUsers = new VisitGroup(
                    Variables.Year(startYear, endYear).LinearTrend(0.2, 4, 0.95)
                        .AddPeak(startYear + duration * .2, duration * 0.03, 1, 0.05),
                    Variables.Random("Campaign", Sets.Weight(SymposiumData.Campaigns["Bing"], 0.8).Weight(SymposiumData.Campaigns["Google"], 0.2).Build())
                    ).Override(baseGroup).Actions(new HeinekenWalk(10, 0.05)).DefaultDuration(new TruncatedRandom(new RandomSkewNormal(110, 30, 3), 5, 1000));


                var cgSet = Sets.Weight(campaignGroups[0], 1).Weight(campaignGroups[1], 1)
                    .Weight(campaignGroups[2], 0.4).Weight(campaignGroups[3], 0.7).Build();                



                var sam = new VisitGroup(Variables.Year(startYear, endYear)
                    .AddPeak(startYear + 0.5*duration, 0.12*duration)).Override(baseGroup)
                    .Actions(new VisitActionSet(Sets.Weight<VisitAction>(
                        new VisitActionSet(basePageSet), 0.3)
                        .Weight(SymposiumData.Pages["Play the Game"].Action, 0.7)
                        .Build()
                        ) {Length = new RandomExponential(1/4d)})
                    .DefaultDuration(new TruncatedRandom(new RandomSkewNormal(80, 50, 3), 5, 1000));

                var brandStore = new VisitGroup(Variables.Year(startYear, endYear).LinearTrend(0.1, 2)).Override(baseGroup)
                    .Actions(new VisitActionSet(Sets.Weight<VisitAction>(
                        new VisitActionSet(basePageSet), 0.1).Weight(SymposiumData.Pages["Brand Store"].Action, 0.9).Build()
                        ) { Length = new RandomExponential(1 / 4d) })
                        .DefaultDuration(new TruncatedRandom(new RandomSkewNormal(100, 10, -3), 5, 1000));

                //.Pages(SimplePageSequenceGenerator.Fixed(url));

                var groups = Sets
                    .Weight<Func<VisitGroup>>(() => baseGroup, 0.2)
                    .Weight(() => keywordVisitors, 0.1)
                    .Weight(() => sam, 0.1)
                    .Weight(() => brandStore, 0.05)
                    .Weight(cgSet.Sample, 0.3)
                    .Weight(()=>legendUsers, 0.2)
                    .Build();

                var sim = new VisitSimulator(() => groups.Sample()(), new ExperienceCrawler());

                var vs = new List<Visit>();
                foreach (var v in sim.Next(visitsPerThread))
                {

                    if (Interlocked.Increment(ref simulated) % 100 == 0)
                    {
                        System.Console.Out.Write("X");
                    }
                    else
                    {
                        System.Console.Out.Write(".");
                    }
                    vs.Add(v);
                }

                return vs;
            }).SelectMany(v => v).ToArray();


            System.Console.Out.WriteLine();
            System.Console.Out.WriteLine("Done.");

        }

        static void NervaDemo()
        {


            var url = "http://develop.officecore.net/";

            int simulated = 1;

            var threads = 6;
            int visitsPerThread = 500;

            //var channels = XDocument.Load(url + "?colossus-map={1EB725EC-BD0A-4B45-9166-9C09BADEDB2C}");
            //var subChannelIds =
            //    channels.Root.Elements("Item")
            //        .Elements("Item")
            //        .Elements("Item")
            //        .Select(item => (Guid)item.Attribute("Id")).ToArray();


            //var citites =
            //    Sets.Exponential(
            //        GetCities().OrderByDescending(c => c.Population).ToArray(),
            //        0.8, 200);

            Randomness.Seed = new Random().Next(0, 1000);

            var visits = Enumerable.Range(0, threads).AsParallel().WithDegreeOfParallelism(threads).Select(ix =>
            {

                var countries = Sets.Exponential(new[]
                {
                    "Denmark", "France", "Netherlands", "United Kingdom", "Australia", "New Zealand", "Japan", "Anguilla", "Antigua and Barbuda", "Aruba", "Bahamas", "Barbados", "Bonaire", "British Virgin Islands", "Cayman Islands", "Cuba", "Curacao", "Dominica", "Dominican Republic", "Grenada", "Guadeloupe", "Haiti", "Jamaica", "Martinique", "Montserrat", "Puerto Rico", "St Barths", "St Kitts and Nevis", "St Lucia", "St Maarten and St Martin", "St Vincent and the Grenadines", "Trinidad and Tobago", "Turks and Caicos Islands", "US Virgin Islands", "Ukraine"
                }, .8, 7);

                var searchPatterns = new DistributedSampleSet<string>(new[]
                {
                    "office supplies",
                    "holiday",
                    "lamp",
                    "camera",
                    "chair",
                    "desk lamp"
                });

                var startYear = 2014d;
                var endYear = startYear + DateTime.Now.DayOfYear / 365d;
                var duration = endYear - startYear;

                var landingPages =
                    Sets.Exponential(
                        new[]
                        {
                            "/",
                            "Products",
                            "About-Us",
                            "Contact",
                            "office-products/black-goose/c-24/p-105",
                            "News",
                            "office-products/holiday-products/c-24/c-70",
                            "office-products/office-products/c-24/c-69",
                            "News/2013/06/04/17/01/Officecore-launches-Partner-Incentive"
                        }.Select(pageUrl => new PageAction(url + pageUrl.ToLower()))
                            .ToArray(), .3, 4);

                var baseGroup =
                    new VisitGroup(
                        Variables.Random("Country", Sets.Weight("Denmark", 0.2).Weight("UK", 0.2).Weight("Netherlands", 0.2).Weight("Germany", 0.2).Weight("France", 0.2).Build()),
                        //Variables.Random("City", citites),
                        //Variables.Fixed("Profile", new Dictionary<string, double>
                        //{
                        //{"Holiday buyer", 2},    
                        //{ "Office buyer", 0.5d }
                        //}),
                 //       Variables.Random("ChannelItemId", Sets.Uniform(subChannelIds)),
                        Variables.Random("DeviceType", Sets.Weight("Desktop", 0.5).Weight("iPhone", 0.5).Build()),
                        Variables.Year(startYear, endYear).LinearTrend(0.5, 1),
                        Variables.DayOfWeek().Weight(Sets
                            .Weight(0, .5)
                            .Weight(1, 1)
                            .Weight(2, 1)
                            .Weight(3, 1)
                            .Weight(4, 1)
                            .Weight(5, 1)
                            .Weight(6, .5).Build()
                            ))
                    .Actions(new PageAction(url))
                        .Actions(
                        new VisitActionList
                        {
                            Actions = new VisitAction[]
                            {
                                new VisitActionSet(Sets.Weight<VisitAction>(
                                        new PageAction(url), 1
                                    ).Weight(new VisitActionSet(landingPages), .5).Weight(new VisitActionSet(landingPages), .4).Weight(new VisitActionSet(landingPages), .3).Build()),  
                            },
                            Length = Sets.Weight(1, 0.18).Weight(2, .82).Weight(2, .87).Weight(2, .92).Build()
                        }
                        )
                    //.Actions(new SiteActionList
                    //{
                    //    Actions = Enumerable.Repeat(new PageAction
                    //        {
                    //            Url = url,
                    //            QueryStringParameters = new Dictionary<string, Func<string>>()
                    //    {
                    //        {"test", ()=>"Lipsum" + Randomness.Random.Next(1, 100)}
                    //    }
                    //        }, 10),
                    //    Length = new RandomExponential(1/3d)
                    //})
                    .DefaultDuration(new TruncatedRandom(new RandomNormal(60, 30), 5, 1000))
                    ;


                var keywordVisitors = new VisitGroup(
                    Variables.Random("Keywords", searchPatterns.Sample),
                    Variables.Random("TrafficType", Sets.Weight(10, 0.9).Weight(15, 0.1).Build()))
                    .Override(baseGroup);

                var goals = new[]
                {                
                    new UrlTriggeredGoal("Register", 0, url + "/My-Account"),
                 //   new UrlTriggeredGoal("Requested Callback", 100, url + "/office-products/black-goose/c-24/p-105"),
                    new UrlTriggeredGoal("Received call-back", 50, url + "/Standard-Items/Call-back"),
                 //   new UrlTriggeredGoal("Presented with call-back form", 0, "/office-products/black-goose/c-24/p-105"),
                    new UrlTriggeredGoal("Tell a Friend", 0, url + "/News/2013/06/04/17/01/Officecore-launches-Partner-Incentive"),
                    new UrlTriggeredGoal("Downloaded Case Study", 25, url + "/Discover/Whitepapers"), 
                    new UrlTriggeredGoal("Requested Discount Code", 25, url),
                    new UrlTriggeredGoal("LoginWithSocial", 10, url + "/My-Account"),
                };


                var campaignGroups = new List<VisitGroup>();

                //office supplies
                campaignGroups.Add(new VisitGroup(
                    Variables.Year(startYear, endYear).LinearTrend(0.2, 4, 0.4)
                        .AddPeak(startYear + duration * .3, duration * 0.1, 1),
                    Variables.Fixed("Campaign", "{6EB3D7B6-2FCB-4BD3-8458-826B21C2D721}"),
                    Variables.Goal(goals[0], 0.2),
                    Variables.Goal(goals[1], 0.1),
                    Variables.Goal(goals[2], 0.2),
                    Variables.Goal(goals[3], 0.2),
                    Variables.Goal(goals[4], 0.2),
                    Variables.Goal(goals[5], 0.1)//,
                  //  Variables.Goal(goals[6], 0.1),
                  //  Variables.Goal(goals[7], 0.1)
                    ));

                //mortgages
                campaignGroups.Add(new VisitGroup(
                    Variables.Year(startYear, endYear).LinearTrend(2, 1),
                    Variables.Fixed("Campaign", "{C520D564-12FA-4108-82AF-4AF73110BE2C}"),
                    Variables.Goal(goals[0], 0.2),
                    Variables.Goal(goals[1], 0.1),
                    Variables.Goal(goals[2], 0.2),
                    Variables.Goal(goals[3], 0.2),
                    Variables.Goal(goals[4], 0.2),
                    Variables.Goal(goals[5], 0.1)//,
                 //   Variables.Goal(goals[6], 0.1),
                //    Variables.Goal(goals[7], 0.1)
                    ));



                //facebook discount
                campaignGroups.Add(new VisitGroup(Variables.Year(startYear, endYear)
                    .AddPeak(startYear + duration * .2, duration * 0.04, -2),
                    Variables.Fixed("Campaign", "{B6CA3741-8510-443A-8A6F-A00BADC5DE53}"),
                    Variables.Goal(goals[0], 0.2),
                    Variables.Goal(goals[1], 0.1),
                    Variables.Goal(goals[2], 0.2),
                    Variables.Goal(goals[3], 0.2),
                    Variables.Goal(goals[4], 0.2),
                    Variables.Goal(goals[5], 0.1)//,
              //      Variables.Goal(goals[6], 0.1),
             //       Variables.Goal(goals[7], 0.1)
                ));

                var campaignVisitors = new VisitGroup(
                         Variables.Random("TrafficType", Sets.Weight(10, 0.33).Weight(15, 0.67).Build())).Override(baseGroup);





                foreach (var cg in campaignGroups) cg.Override(campaignVisitors);

                var cgSet = Sets.Uniform(campaignGroups.ToArray());

                //.Pages(SimplePageSequenceGenerator.Fixed(url));

                var groups = Sets
                    .Weight<Func<VisitGroup>>(() => baseGroup, 0.2)
                    .Weight(() => keywordVisitors, 0.5)
                    .Weight(cgSet.Sample, 0.3).Build();

                var sim = new VisitSimulator(() => groups.Sample()(), new ExperienceCrawler());

                var vs = new List<Visit>();
                foreach (var v in sim.Next(visitsPerThread))
                {

                    if (Interlocked.Increment(ref simulated) % 100 == 0)
                    {
                        System.Console.Out.Write("X");
                    }
                    else
                    {
                        System.Console.Out.Write(".");
                    }
                    vs.Add(v);
                }

                return vs;
            }).SelectMany(v => v).ToArray();


            System.Console.Out.WriteLine();
            System.Console.Out.WriteLine("Done.");
        }


        static void Simple(string[] args)
        {
            var output = System.Console.Out;


            var baseUrl = "http://develop.officecore.net";
            //This is the URL for a page that contains a test.
          //  var testUrl = baseUrl + "/en/Solutions/Solution%201.aspx";


            //These are the conversions the simulated visitors might make.
            //The URLs points to pages that will trigger a goal.
            var goals = new[]
            {                
              //  new UrlTriggeredGoal("Brochure Request", 2, baseUrl + "/en/Partners/Aoede.aspx"),
                new UrlTriggeredGoal("Requested Callback", 100, baseUrl + "/office-products/black-goose/c-24/p-105"),
                new UrlTriggeredGoal("Received call-back", 50, baseUrl + "/Standard-Items/Call-back")                
            };


            var config = new VisitGroup(
                //These are the "normal" conversion rates (i.e. regardless of test experience)
                Variables.Goal(goals[0], 0.5),
                Variables.Goal(goals[1], 0.1)
               // Variables.Goal(goals[2], 0.05)

                ).When("Promo", "Original").Then(
                //When the component called "Promo" has the value "Original", the conversion rate for goal 1 will increase.
                //Experiences containing Promo->Original will be winners
                    Variables.Goal(goals[0], 0.5)
                );//.Actions(new PageAction(testUrl));



           // output.WriteLine("Making requests for {0}", testUrl);
          //  output.WriteLine();
            var simpleSim = new VisitSimulator(config, new ExperienceCrawler());
            var visits = new List<Visit>();
            foreach (var v in simpleSim.Next(50))
            {
                output.Write(".");
                visits.Add(v);
            }

            output.WriteLine();
            output.WriteLine();

            var test = simpleSim.ContextFactory.Tests.FirstOrDefault();
            if (test != null)
            {
                foreach (var v in visits)
                {
                    output.WriteLine("Made a visit for experience #{1} with value {0:N0}", v.Value,
                        v.Experiences[test].Number);
                }

                output.WriteLine();
                output.WriteLine("Summary:");

                Summarize(output, simpleSim.ContextFactory.Tests.First(), visits);
            }

            output.WriteLine();
            output.WriteLine("Done.");
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
                Variables.Goal(goals[2], 0.02)
                    .WhenTrue(Variables.Random<string>("Gender", new object[,] { { "Male", 0.2 }, { "Female", 0.8 } })),
                //<- That is one way to specify weights for values

                Variables.Random<string>("Gender", Sets.Weight("Male", 0.49).Weight("Female", 0.51)),
                // <- That is another one

                Variables.Random("Country", Sets.Exponential(new[] { "Denmark", "Australia", "Chile", "Sweden" }, 0.8, 3)),
                // <- 80 % of the visits will com e from the three first countries

                Variables.MultiSet(b => b.Weight(Variables.Fixed("Keywords", "Horse"), 0.25)),


                //Variables.Random("Keywords", Sets.Exponential(new[]{null, "Horse"}, 0.8, 3)),

                Variables.Year(2012, 2014).LinearTrend(1, 2),
                //Double as many visits will hit the site start 2014 as start of 2012                                
                Variables.Hour().AddPeak(12, 1).AddPeak(20, 4),
                //Visits will have a sharp peak at lunch and a soft peak in the evening
                Variables.DayOfWeek().AddPeak(1, 4).Build() //Most visits occur Monday
                ).Actions(new PageAction(testUrl));



            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany")
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When("Promo", "Variation Name").Then(Variables.Goal(goals[2], 0.4));


            var crawler = new ExperienceCrawler();

            var simulator = new VisitSimulator(Sets.Weight(baseGroup, 0.8).Weight(germans, 0.2).Build(), crawler);


            var visits = new List<Visit>();

            foreach (var v in simulator.Next(1000))
            {
                output.Write(".");
                visits.Add(v);
            }

            output.WriteLine();

            var test = crawler.Tests.Values.FirstOrDefault();
            if (test != null)
            {
                foreach (var country in visits.GroupBy(v => v.Tags["Country"]).OrderBy(g => g.Key))
                {
                    output.WriteLine("\"{0}\":", country.Key);
                    Summarize(output, test, country);
                    output.WriteLine();
                }

                output.WriteLine("All visits:");
                Summarize(output, test, visits);
            }
        }

        static void Offline(string[] args)
        {

            var output = System.Console.Out;


            var factors = new[] { CreateExperienceFactor("V1", 2), CreateExperienceFactor("V2", 5) };
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



                Variables.Random("Country", Sets.Exponential(
                    new[] { "Denmark", "Brazil", "Australia", "Chile", "Sweden", "China", "Finland", "Portugal" }, 0.8, 3))
                    .Correlate(country => country == "Finland" || country == "Chile",
                        Variables.Random<string>("Gender", new object[,] { { "Male", 0.75 }, { "Female", 0.25 } })),

                Variables.Random<string>("Gender", Sets.Weight("Male", 0.49).Weight("Female", 0.51)),

                Variables.Year(2012, 2014).DrawTrend().LineTo(2014, 2).Close(), //Double as many visits will hit the site start 2014 as start of 2012                
                Variables.Hour().Uniform(1).AddPeak(12, 1, shape: 3).AddPeak(20, 4, shape: -2), //Visits will have a sharp peak at lunch and a soft peak in the evening
                Variables.DayOfWeek().AddPeak(1, 4) //Most visits occur Monday


                );

            var germans = new VisitGroup(
                Variables.Fixed("Country", "Germany"),
                Variables.Hour().AddPeak(18, 3), // All Germans visit the site around 18:00

                Variables.DayOfYear().Mute(.5).AddPeak(182.5, 100 * 1.5) //Germans prefer the site during summer
                ).Override(baseGroup)
                //Germans will buy a lot when Var 1 shows B
                .When(null, "B").Then(Variables.Goal(goals[2], 0.4))
                //Germans don't like the combination A/B. The conversion rate for downloads will drop with 20%
                    .When("V1", "A").And("V2", "B").Then(Variables.Goal(goals[0], .08));


            var simulator = new VisitSimulator(Sets.Weight(baseGroup, 0.8).Weight(germans, 0.2).Build(), new RoundRobinVisitContextFactory(test));


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
                f.WriteTsvLine("Date", "Year", "Month", "Day", "DayOfWeek", "Hour", "Country", "Gender", "Count");
                foreach (var v in visits)
                {
                    var date = v.StartDate.Date;
                    f.WriteTsvLine(date.Date, date.Year, date.Month, date.Day, (int)date.DayOfWeek, date.Hour, v.Tags["Country"], v.Tags["Gender"], 1);
                }
            }
        }

        static void Summarize(TextWriter output, Test test, IEnumerable<Visit> visits)
        {
            output.WriteLine("{0, -30} {1,8} {2,8}", "", "Count", "VpV");
            output.WriteLine(string.Join("", Enumerable.Repeat("-", 48)));
            foreach (var exp in visits.GroupBy(v => v.Experiences[test]).OrderBy(e => e.Key.Number))
            {
                output.WriteLine("{0,-30} {1,8:N0} {2,8:N2}", exp.Key, exp.Count(), exp.Average(v => v.Value));
            }
            output.WriteLine(string.Join("", Enumerable.Repeat("-", 48)));
            output.WriteLine("{0, -30} {1,8:N0} {2,8:N2}", "", visits.Count(), visits.Average(v => v.Value));
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
                Levels = Enumerable.Range(0, variationCount).Select(i => "" + (char)(65 + i)).ToArray()
            };
        }


        static List<City> GetCities()
        {
            var enus = CultureInfo.GetCultureInfo("en-US");
            var cities = new List<City>();
            foreach (var line in File.ReadAllLines("cities15000.txt").Skip(1))
            {
                var parts = line.Split('\t');

                cities.Add(new City
                {
                    Name = parts[1],
                    Country = parts[8],
                    Adm1 = parts[10],
                    Adm2 = parts[11],
                    Adm3 = parts[13],
                    Adm4 = parts[14],
                    Population = int.Parse(parts[14]),
                    TimeZone = parts[17],
                    Latitude = double.Parse(parts[4], enus),
                    Longitude = double.Parse(parts[5], enus)
                });
            }

            return cities;
        }
    }
}
