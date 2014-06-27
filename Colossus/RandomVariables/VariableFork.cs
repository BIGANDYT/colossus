using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    //A "do nothing" variable that simply weights other variables
    public class VariableFork : RandomVariable
    {
        private SampleSet<IEnumerable<IRandomVariable>> _set;
        
        public VariableFork(Dictionary<IEnumerable<IRandomVariable>, double> weights)
        {            
            _set = new SampleSet<IEnumerable<IRandomVariable>>(weights);
        }

        public override IRandomValue Sample(SampleContext context = null)
        {
            return new RandomValue<Guid>(this, Guid.NewGuid(), (visit) => { }, _set.Sample());
        }
    }
}
