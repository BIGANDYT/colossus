using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomYear : PeakVariable
    {
        public RandomYear(double min, double max, double? offset, double spread = 0) : base(min, max, offset, spread)
        {
            
        }

        protected override void Action(Visit visit, double value)
        {
            visit.Start = visit.Start.AddYears(-visit.Start.Year + (int) Math.Round(value));
        }
    }
}
