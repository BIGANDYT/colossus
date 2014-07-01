using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    //A "do nothing" variable that simply weights other variables
    public class RandomFork : RandomVariable
    {
        private SampleSet<IEnumerable<IRandomVariable>> _set;

        public RandomFork(SampleSet<IEnumerable<IRandomVariable>> forks)
        {
            _set = forks;
        }

        public override IRandomValue Sample(SampleContext context = null)
        {
            return new RandomValue<Guid>(this, Guid.NewGuid(), (visit) => { }, _set.Sample());
        }
    }
}
