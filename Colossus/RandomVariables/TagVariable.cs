using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public abstract class TagVariable : RandomVariable<string>
    {
        protected TagVariable(string key, IRandomDistribution random = null)
            : base(key, random)
        {
            Key = key;
        }
    }
}
