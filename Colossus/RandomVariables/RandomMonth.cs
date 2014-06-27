using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomDayOfYear : PeakVariable
    {
        public RandomDayOfYear(double? offset = null, double spread = 0) : base(0, 365, offset, spread)
        {

        }

        protected override void Action(Visit visit, double value)
        {
            var day = (Math.Floor(value)/365)*(DateTime.IsLeapYear(visit.Start.Year) ? 366 : 365);            
            visit.Start = visit.Start.AddDays(-visit.Start.DayOfYear + day);
        }
    }
}
