using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.Pages
{
    public class VisitActionSet : VisitAction
    {
        public ISampleSet<VisitAction> Actions { get; set; }

        public ISampleSet<VisitAction> Successor { get; set; }

        public IRandomDistribution Length { get; set; }

        public VisitActionSet(ISampleSet<VisitAction> actions)
        {
            Actions = actions;
        }

        public override void Execute(VisitContext context)
        {
            var i = 0;
            var length = Length != null ? Length.Next() : 1;
            for (;;)
            {
                var action = Actions.Sample();
                if (action != null)
                {
                    action.Parent = this;
                    action.Execute(context);                    
                }
                if (++i >= length) break;
            }

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
