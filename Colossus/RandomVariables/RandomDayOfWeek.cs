using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomDayOfWeek : ContinuousVariableBase<Type>
    {
        public RandomDayOfWeek(IRandomDistribution random) : base(typeof(RandomDayOfWeek), random)
        {
        }

        protected override void Action(Visit visit, double value)
        {            
            visit.StartDate.DayOfWeek = value;
        }
    }
}
