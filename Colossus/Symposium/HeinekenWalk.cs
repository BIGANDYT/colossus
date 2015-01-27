using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.Pages;

namespace Colossus.Symposium
{
    public class HeinekenWalk : VisitAction
    {
        private readonly double _bounceRate;
        private Random _random;

        private ISampleSet<VisitAction> _midPages;

        private double _conversionRate;

        public HeinekenWalk(double legendWeight = 1, double bounceRate = 0.1)
        {
            _bounceRate = bounceRate;
            _random = Randomness.Random;
            _conversionRate = legendWeight > 1 ? 0.8 : 0.55d;

            _midPages = Sets.Weight(SymposiumData.Pages["The Date"].Action, 1)
                .Weight(SymposiumData.Pages["The Kitchen"].Action, 1)
                .Weight(SymposiumData.Pages["The Legend"].Action, legendWeight).Build();
        }

        
        public override void Execute(VisitContext context)
        {
            var i = 0;
            foreach (var action in GetActions(context))
            {
                //Console.Out.WriteLine(i++ + ":");
                action.Parent = this;
                action.Execute(context);
            }
            
        }


        IEnumerable<VisitAction> GetActions(VisitContext context)
        {
            yield return SymposiumData.Pages["Experience"].Action;
            if (_random.NextDouble() < _bounceRate) yield break;

            yield return SymposiumData.Pages["Adverts"].Action;

            var midPages = _random.Next(0, 4);
            for (var i = 0; i < midPages; i++)
            {
                yield return _midPages.Sample();
            }

            if (_random.NextDouble() < _conversionRate)
            {
                yield return SymposiumData.Pages["Register"].Action;
                if (_random.NextDouble() < 0.8)
                {
                    yield return SymposiumData.Pages["Ticket Purchase"].Action;
                }
            }

        }
    }
}
