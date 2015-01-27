using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Colossus.RandomVariables;

namespace Colossus.Pages
{
    public abstract class VisitAction
    {
        public VisitAction Parent { get; set; }

        public IRandomDistribution Duration { get; set; }

        public List<IRandomVariable> Variables { get; set; }
       
        T Get<T>(Func<VisitAction, T> getter)
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
}
