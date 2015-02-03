using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Colossus
{
    public class VisitWebClient : WebClient
    {
        public WebVisitContext Context { get; set; }

        public VisitWebClient(WebVisitContext context)
        {
            Context = context;
        }

        private readonly CookieContainer _container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            //Console.Out.WriteLine(address);
            var request = base.GetWebRequest(address);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = _container;
                webRequest.MaximumResponseHeadersLength = -1;
            }
            
            Context.PrepareRequest(request);
            
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                var response = base.GetWebResponse(request);

                if (response.Headers["X-Colossus-Processing"] != "OK")
                {
                    throw new Exception("The site didn't return as expected. Is the interceptor installed?");
                }

                Context.ParseResponse(response);

                return response;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    var response = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                    File.WriteAllText(@"C:\Temp\ColossusError.htm", request.RequestUri + "\r\n\r\n" + response);
                }
                File.WriteAllText(@"C:\Temp\ColossusError.htm", request.RequestUri + "\r\n\r\n" + wex.Message+ "\r\n\r\n" + wex.InnerException + "\r\n\r\n" + wex.StackTrace);

                throw;
            }                                  
        }
    }
}
