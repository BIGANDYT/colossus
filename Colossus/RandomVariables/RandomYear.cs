using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class RandomYear : ContinuousVariableBase<Type>
    {
        protected override void Action(Visit visit, double value)
        {            
            visit.StartDate.Year = value;
        }


        public RandomYear(IRandomDistribution random)
            : base(typeof(RandomYear), random)
        {

        }


    }
}
