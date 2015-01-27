using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class SampleContext
    {
        public Visit Visit { get; set; }
        public List<IRandomVariable> Variables { get; set; }
        //public Dictionary<Goal, double> GoalBoosts { get; set; }

        public SampleContext()
        {
            //GoalBoosts = new Dictionary<Goal, double>();
            Variables = new List<IRandomVariable>();
        }


    }
}
