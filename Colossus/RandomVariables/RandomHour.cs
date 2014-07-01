using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomHour : ContinousVariable<Type>
    {
        public RandomHour(IRandomDistribution random) : base(typeof(RandomHour), random)
        {
        }

        protected override void Action(Visit visit, double value)
        {
            visit.StartDate.Hour = value;
        }
    }
}
