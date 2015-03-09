using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;

namespace Colossus.Integration
{
    public class WhoIsProcessor : ITagDataProcessor
    {
        public void Process(JObject visitTags, JObject requestData)
        {
            if (Tracker.Current == null) return;

            var language = visitTags.Value<string>("Language");
            if (!string.IsNullOrEmpty(language))
            {
                Tracker.Current.Interaction.Language = language;
            }


            //var city = visitTags.Value<JObject>("City");
            //if (city != null)
            //{
            //    var whois = new WhoIsInformation
            //    {
            //        BusinessName = "",
            //        City = (string)city["Name"] ?? "",
            //        Country = (string)city["Country"] ?? "",
            //        Region = (string)city["Adm1"] ?? "",
            //        Latitude = (double)city["Latitude"],
            //        Longitude = (double)city["Longitude"]
            //    };

            //    Tracker.Current.Interaction.SetGeoData(whois);
            //    Tracker.Current.Interaction.UpdateLocationReference();

            //}
            //else if (visitTags["IP"] == null)
            //{
            var whois = GetWhoIs(visitTags.Value<string>("Country") ?? "");
            //{
            //    BusinessName = "",
            //    Country = visitTags.Value<string>("Country") ?? ""
            //};

            //whois.City = GetCity(whois.Country);

            Tracker.Current.Interaction.SetGeoData(whois);
            Tracker.Current.Interaction.UpdateLocationReference();
            //}
            //else
            //{
            //    var ip = visitTags.Value<string>("IP");
            //    if (ip != null)
            //    {
            //        Tracker.Current.Interaction.Ip = ip.Split('.').Select(byte.Parse).ToArray();
            //    }
            //}
        }

        /// <summary>
        /// Get a random city by country
        /// </summary>
        /// <param name="country">The country name</param>
        /// <returns>A city name</returns>
        private static WhoIsInformation GetWhoIs(string country)
        {
            var id = new Random(Guid.NewGuid().GetHashCode());
            var randomId = id.NextDouble();
            var whois = new WhoIsInformation();

            switch (country)
            {
                case "United Kingdom":
                    if (randomId <= 0.2)
                    {
                        whois.Country = country;
                        whois.City = "London";
                        whois.Region = "London";
                    }
                    else if (randomId > 0.2 && randomId <= 0.4)
                    {
                        whois.Country = country;
                        whois.City = "Liverpool";
                        whois.Region = "North West England";
                    }
                    else if (randomId > 0.4 && randomId <= 0.6)
                    {
                        whois.Country = country;
                        whois.City = "Bristol";
                        whois.Region = "South West";
                    }
                    else if (randomId > 0.6 && randomId <= 0.8)
                    {
                        whois.Country = country;
                        whois.City = "Manchester";
                        whois.Region = "North West England";
                    }
                    else
                    {
                        whois.Country = country;
                        whois.City = "Leeds";
                        whois.Region = "Yorkshire and the Humber";
                    }
                    break;
                //case "Netherlands":
                //    if (randomId <= 0.2)
                //    {
                //        city = "Amsterdam";
                //    }
                //    else if (randomId > 0.2 && randomId <= 0.4)
                //    {
                //        city = "Groningen";
                //    }
                //    else if (randomId > 0.4 && randomId <= 0.6)
                //    {
                //        city = "Leeuwarden";
                //    }
                //    else if (randomId > 0.6 && randomId <= 0.8)
                //    {
                //        city = "Rotterdam";
                //    }
                //    else
                //    {
                //        city = "Den Haag";
                //    }
                //    break;
                case "Germany":
                    if (randomId <= 0.2)
                    {
                        whois.Country = country;
                        whois.City = "Stuttgart";
                        whois.Region = "Baden-Württemberg";
                    }
                    else if (randomId > 0.2 && randomId <= 0.4)
                    {
                        whois.Country = country;
                        whois.City = "Berlin";
                        whois.Region = "Berlin";
                    }
                    else if (randomId > 0.4 && randomId <= 0.6)
                    {
                        whois.Country = country;
                        whois.City = "Düsseldorf";
                        whois.Region = "North Rhine-Westphalia";
                    }
                    else if (randomId > 0.6 && randomId <= 0.8)
                    {
                        whois.Country = country;
                        whois.City = "Koblenz";
                        whois.Region = "Rhineland-Palatinate";
                    }
                    else
                    {
                        whois.Country = country;
                        whois.City = "Dresden";
                        whois.Region = "Saxony";
                    }
                    break;
                case "Denmark":
                    if (randomId <= 0.2)
                    {
                        whois.Country = country;
                        whois.City = "Copenhagen";
                        whois.Region = "Capital of Denmark";
                    }
                    else if (randomId > 0.2 && randomId <= 0.4)
                    {
                        whois.Country = country;
                        whois.City = "Aarhus";
                        whois.Region = "Central Denmark";
                    }
                    else if (randomId > 0.4 && randomId <= 0.6)
                    {
                        whois.Country = country;
                        whois.City = "Odense";
                        whois.Region = "Southern Denmark";
                    }
                    else if (randomId > 0.6 && randomId <= 0.8)
                    {
                        whois.Country = country;
                        whois.City = "Aalborg";
                        whois.Region = "North Denmark";
                    }
                    else
                    {
                        whois.Country = country;
                        whois.City = "Frederiksberg";
                        whois.Region = "Capital of Denmark";
                    }
                    break;
                case "France":
                    if (randomId <= 0.2)
                    {
                        whois.Country = country;
                        whois.City = "Paris";
                        whois.Region = "Île-de-France";
                    }
                    else if (randomId > 0.2 && randomId <= 0.4)
                    {
                        whois.Country = country;
                        whois.City = "Dijon";
                        whois.Region = "Burgundy";
                    }
                    else if (randomId > 0.4 && randomId <= 0.6)
                    {
                        whois.Country = country;
                        whois.City = "Strasbourg";
                        whois.Region = "Alsace";
                    }
                    else if (randomId > 0.6 && randomId <= 0.8)
                    {
                        whois.Country = country;
                        whois.City = "Bordeaux";
                        whois.Region = "Aquitaine";
                    }
                    else
                    {
                        whois.Country = country;
                        whois.City = "Montpellier";
                        whois.Region = "Languedoc-Roussillon";
                    }
                    break;
                case "Japan":
                    if (randomId <= 0.2)
                    {
                        whois.Country = country;
                        whois.City = "Tokyo";
                        whois.Region = "Kantō";
                    }
                    else if (randomId > 0.2 && randomId <= 0.4)
                    {
                        whois.Country = country;
                        whois.City = "Sendai";
                        whois.Region = "Tōhoku";
                    }
                    else if (randomId > 0.4 && randomId <= 0.6)
                    {
                        whois.Country = country;
                        whois.City = "Sapporo";
                        whois.Region = "Hokkaidō";
                    }
                    else if (randomId > 0.6 && randomId <= 0.8)
                    {
                        whois.Country = country;
                        whois.City = "Osaka";
                        whois.Region = "Kansai";
                    }
                    else
                    {
                        whois.Country = country;
                        whois.City = "Matsuyama";
                        whois.Region = "Shikoku";
                    }
                    break;
            }

            return whois;
        }
    }
}
