using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class SampleContext
    {
        public Dictionary<Goal, double> GoalBoosts { get; set; }

        public SampleContext()
        {
            GoalBoosts = new Dictionary<Goal, double>();
            Variables = new List<IRandomVariable>();
        }

        public List<IRandomVariable> Variables { get; set; }        
    }
}
