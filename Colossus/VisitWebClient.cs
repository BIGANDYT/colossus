using System;
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
            var request = base.GetWebRequest(address);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = _container;
            }
            
            Context.PrepareRequest(request);

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);

            if (response.Headers["X-Colossus-Processing"] != "OK")
            {
                throw new Exception("The site didn't return as expected. Is the interceptor installed?");
            }

            Context.ParseResponse(response);
            
            return response;
        }
    }
}
