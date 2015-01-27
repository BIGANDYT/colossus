using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace Colossus.Pages
{
    public class RandomWalk : VisitAction
    {

        public string MapUrl { get; set; }

        public Func<VisitAction> StartPage { get; set; }

        public IRandomDistribution Length { get; set; }

        private XElement _map;

        public override void Execute(VisitContext context)
        {            
            var random = Randomness.Random;

            var page = StartPage();

            //System.Console.Out.WriteLine("Random walk");

            var wc = context.WebClient;
            if (wc != null)
            {
                if (_map == null)
                {
                    _map = XDocument.Load(MapUrl).Root;
                }
                //wc.Headers.Add("X-Colossus-Map", _map == null ? "true" : "false");
                page.Parent = this;
                page.Execute(context);
                //wc.Headers.Remove("X-Colossus-Map");


                //var map = _map ?? XDocument.Parse(wc.ResponseHeaders["X-Colossus-Map"]).Root;

                var current = _map;
                var length = Math.Round(Length.Next());
                for (var i = 0; i < length; i++)
                {
                    var parent = current.Parent;
                    var children = current.Elements("Item").ToArray();
                    var siblings = current.Parent == null
                        ? new XElement[0]
                        : current.Parent.Elements("Item").Where(e => e != current).ToArray();

                    if (parent == null && children.Length == 0 && siblings.Length == 0) break;


                    var direction = 0;

                resample:
                    var x = random.NextDouble();
                    if (x < 0.1 && parent != null)
                    {
                        current = parent;
                        direction = -1;
                    }
                    //If we just jumped one step up, don't descent again too often
                    else if (x < (direction == -1 ? 0.2 : 0.5) && children.Length > 0)
                    {
                        current = children[random.Next(0, children.Length)];
                        direction = 1;
                    }
                    else if (siblings.Length > 0)
                    {
                        current = siblings[random.Next(0, siblings.Length)];
                        direction = 0;
                    }
                    else
                    {
                        goto resample;
                    }

                    new PageAction((string)current.Attribute("HRef")) { Parent = this }.Execute(context);
                }
            }

            //System.Console.Out.WriteLine("");

            //var i = 0;
            //while (page != null && i++ <= length)
            //{
            //    page.Execute(context);
            //    page = null;

            //    if (context.LastResponse != null)
            //    {
            //        var doc = new HtmlDocument();
            //        doc.LoadHtml(context.LastResponse);
            //        if (doc.DocumentNode != null)
            //        {
            //            var links = doc.DocumentNode.SelectNodes("//a");
            //            if (links != null)
            //            {
            //                var goodLinks = links.Where(l => l.GetAttributeValue("href", "") != "").ToArray();                            

            //                var link = goodLinks[random.Next(0, goodLinks.Length)].GetAttributeValue("href", "");

            //                var urlBuilder = new UriBuilder(BaseUrl);
            //                urlBuilder.Path = link;
            //                page = new PageAction
            //                {
            //                    Parent = this,
            //                    Url = urlBuilder.Uri.ToString()
            //                };
            //            }
            //        }
            //    }
            //}
        }
    }
}
