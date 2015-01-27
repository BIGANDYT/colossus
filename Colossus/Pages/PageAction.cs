using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Colossus.Pages
{
    public class PageAction : VisitAction
    {
        public string Url { get; set; }

        public Dictionary<string, Func<string>> QueryStringParameters { get; set; }



        public PageAction(string url)
        {
            Url = url;
            QueryStringParameters = new Dictionary<string, Func<string>>();
        }

        public override void Execute(VisitContext context)
        {
            base.Execute(context);

            var url = new StringBuilder(Url);

            var search = Url.Contains("?");
            foreach (var p in QueryStringParameters)
            {
                url.Append(search ? "&" : "?");
                url.Append(p.Key).Append("=").Append(p.Value());

                search = true;
            }

            Execute(context, url.ToString());
        }

        protected void Execute(VisitContext context, string url)
        {
            var wc = context.WebClient;
            if (wc != null)
            {                
                //System.Console.Out.WriteLine(url);                
                try
                {
                    context.LastResponse = wc.DownloadString(url);
                }
                catch (WebException wex)
                {
                    var r = wex.Response as HttpWebResponse;
                    if (r == null || r.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }

            context.ConvertGoals();
        }
    }
}