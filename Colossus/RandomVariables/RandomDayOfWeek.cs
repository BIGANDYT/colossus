using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomDayOfWeek : PeakVariable
    {
        public RandomDayOfWeek(double? offset = null, double spread = 0) 
            : base(0, 7, offset, spread)
        {
        }

        protected override void Action(Visit visit, double value)
        {            
            var day = Math.Floor(value);            
            visit.Start = visit.Start.AddDays(-(int) visit.Start.DayOfWeek + day);
        }
    }
}
