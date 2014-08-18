using System;
using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public class GoalVariable : RandomVariable<Goal>
    {
        public List<Tuple<ExperienceFactor, int>> Given { get; set; }

        public List<IRandomVariable> TrueCorrelations { get; set; }

        public List<IRandomVariable> FalseCorrelations { get; set; }
        
        public double Probability { get; set; }

        public GoalVariable(Goal goal, double probability) : base(goal)
        {            
            Probability = probability;
            TrueCorrelations = new List<IRandomVariable>();
            FalseCorrelations = new List<IRandomVariable>();
        }


        public override IRandomValue Sample(SampleContext context = null)
        {
            double boost;
            
            boost = context != null && context.GoalBoosts.TryGetValue(Key, out boost) ? boost : 1d;            
           
            var value = Random.Next() < boost*Probability;

            if (context != null && context.Visit != null && Key.GetState(context.Visit) == GoalState.Unavailable)
            {
                value = false;
            }

            return new RandomValue<bool>(this, value,
                (visit) =>
                {                    
                    if (!value && visit.Goals.Contains(Key))
                    {
                        visit.Goals.Remove(Key);
                        visit.Value -= Key.Value;
                    }
                    else if (value)
                    {
                        if (!visit.Goals.Contains(Key))
                        {
                            visit.Value += Key.Value;
                        }
                        visit.Goals.Add(Key);                        
                    }
                }, value ? TrueCorrelations : FalseCorrelations);
        }
    }
}
