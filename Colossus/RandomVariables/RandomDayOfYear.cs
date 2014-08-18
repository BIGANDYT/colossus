using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomDayOfYear : ContinuousVariableBase<Type>
    {
        public RandomDayOfYear(IRandomDistribution random) : base(typeof(RandomDayOfYear), random)
        {
        }

        protected override void Action(Visit visit, double value)
        {
            visit.StartDate.DayOfYear = value;
        }
    }
}
