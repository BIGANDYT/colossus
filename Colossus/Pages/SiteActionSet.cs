using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.Pages
{
    public class SiteActionSet : SiteAction
    {
        public SampleSet<SiteAction> Actions { get; set; }

        public SampleSet<SiteActionSet> Successor { get; set; }

        public override void Execute(VisitContext context)
        {
            var action = Actions.Sample();
            if (action != null)
            {
                action.Parent = this;
                action.Execute(context);

                if (Successor != null)
                {
                    var succ = Successor.Sample();
                    if (succ != null)
                    {
                        succ.Parent = this;
                        succ.Execute(context);
                    }
                }
            }            
        }
    }
}
