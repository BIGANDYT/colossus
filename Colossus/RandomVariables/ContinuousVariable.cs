using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class ContinuousVariable : ContinuousVariableBase<string>
    {
        public ContinuousVariable(string key, IRandomDistribution random) : base(key, random)
        {
        }

        protected override void Action(Visit visit, double value)
        {
            visit.Tags[Key] = value;
        }
    }
}
