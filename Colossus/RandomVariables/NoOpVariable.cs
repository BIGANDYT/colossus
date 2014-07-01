using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class NoOpVariable : RandomVariable
    {
        public override IRandomValue Sample(SampleContext context = null)
        {
            return null;
        }
    }
}
