using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.Pages;

namespace Colossus.Symposium
{
    public static class SymposiumData
    {
        public static Dictionary<string,SymposiumPage> Pages { get; set; }

        public static Dictionary<string, string> Campaigns { get; set; }

        public static string Url { get; set; }

        static SymposiumData()
        {
            Pages = new Dictionary<string, SymposiumPage>
            {
                {"Home", new SymposiumPage("/", rank: 100)},

                {"Experience", new SymposiumPage("/The Experience", rank: 10)},

                {"Brochure", new SymposiumPage("/The Experience/Brochure")},
                {"Register", new SymposiumPage("/Visitor Info/Register", 20)},
                {"Ticket Purchase", new SymposiumPage("/Tickets/Ticket Purchase", 50)},
                {"Video", new SymposiumPage("/Wall Of Fame/Video", rank: 5)},
                {"Adverts", new SymposiumPage("/The Experience/Adverts")},

                {"The Date", new SymposiumPage("/The Experience/Adverts/The Date", 10)},
                {"The Kitchen", new SymposiumPage("/The Experience/Adverts/The Kitchen", 10)},
                {"The Legend", new SymposiumPage("/The Experience/Adverts/The Legend", 10)},

                {"Champions League", new SymposiumPage("/Wall Of Fame/Video/Champions League", rank: 5)},
                {"Play the Game", new SymposiumPage("/Wall Of Fame/Video/Play the Game")},
                {"Brand Store", new SymposiumPage("/Brand Store")}
            };

            Campaigns = new Dictionary<string, string>
            {
                {"Bing", "{DCF187D0-23B6-44A1-ADB6-F296CB97E9AA}"},
                {"Facebook", "{AA27998F-1BF6-40F0-9198-C3BAC729A4F2}"},
                {"Google", "{64BF46D3-BC9E-4A65-8199-97C0175493A2}"},
                {"Twitter", "{DB528542-405D-40BA-BFE8-236B690BEBD9}"}
            };
        }
    }

    public class SymposiumPage
    {
        public string Url { get; set; }

        public int Value { get; set; }

        public double Rank { get; set; }

        public SymposiumPage(string url, int value = 0, double rank = 0)
        {
            Url = url;
            Value = value;
            Rank = rank;
        }

        public VisitAction Action
        {
            get { return new PageAction( new Uri(new Uri(SymposiumData.Url), Url).ToString()); }
        }
    }
}
