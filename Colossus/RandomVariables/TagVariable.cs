using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public abstract class TagVariable : RandomVariable
    {
        public string Key { get; set; }

        protected TagVariable(string key)
        {
            Key = key;
        }
    }
}
