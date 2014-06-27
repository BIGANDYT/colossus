using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomHour : PeakVariable
    {
        public RandomHour(double? offset = null, double spread = 0) 
            : base(0, 24, offset, spread)
        {
        }

        protected override void Action(Visit visit, double value)
        {
            visit.Start = visit.Start.AddHours(-visit.Start.Hour + value);
        }
    }
}
