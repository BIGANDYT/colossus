using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Colossus.RandomVariables;

namespace Colossus.Pages
{
    public class VisitActionList : VisitAction
    {
        public IEnumerable<VisitAction> Actions { get; set; }


        public IRandomDistribution Length { get; set; }

        public VisitActionList(IEnumerable<VisitAction> actions)
        {
            Actions = actions;
        }

        public VisitActionList(params VisitAction[] actions)
        {
            Actions = actions;
        }

        public override void Execute(VisitContext context)
        {
            var i = 0;
            var length = Length != null ? Math.Round(Length.Next()) : int.MaxValue;
            foreach (var action in Actions)
            {
                action.Parent = this;
                action.Execute(context);

                if (++i >= length)
                {                                   
                    break;
                }
            }            
        }
    }
}
