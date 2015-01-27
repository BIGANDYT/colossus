using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Colossus.RandomVariables;

namespace Colossus.Pages
{
    public abstract class SiteAction
    {
        public SiteAction Parent { get; set; }

        public IRandomDistribution Duration { get; set; }

        public List<IRandomVariable> Variables { get; set; }
       
        T Get<T>(Func<SiteAction, T> getter)
        {
            var action = this;
            while (action != null)
            {
                var value = getter(action);
                if (!Equals(value, default(T))) return value;
                action = action.Parent;
            }
            return default(T);
        }

        
        public virtual void Execute(VisitContext context)
        {
            var variables = Get(a => a.Variables);
            if (variables != null )
            {
                context.Visit.UpdateState(variables);
            }

            var duration = Get(a => a.Duration);
            if (duration != null)
            {
                context.Clock.Update(TimeSpan.FromSeconds(duration.Next()));
            }
        }
    }

    public class PageAction : SiteAction
    {
        public string Url { get; set; }

        public Dictionary<string, Func<string>> QueryStringParameters { get; set; }

        

        public PageAction()
        {
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
                wc.DownloadString(url);
            }

            context.ConvertGoals();
        }
    }
}
