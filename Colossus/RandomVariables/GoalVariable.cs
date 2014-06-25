using System;
using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public class GoalVariable : RandomVariable<Goal, bool>
    {
        public List<Tuple<ExperienceFactor, int>> Given { get; set; }

        public List<IRandomVariable> TrueCorrelations { get; set; }

        public List<IRandomVariable> FalseCorrelations { get; set; }

        public double Probability { get; set; }

        public GoalVariable(Goal goal, double probability)
        {
            Key = goal;
            Probability = probability;
            TrueCorrelations = new List<IRandomVariable>();
            FalseCorrelations = new List<IRandomVariable>();
        }


        public override IRandomValue<bool> Sample(SampleContext context = null, Random random = null)
        {
            double boost;
            boost = context != null && context.GoalBoosts.TryGetValue(Key, out boost) ? boost : 1d;            
           
            var value = (random ?? Randomness.Random).NextDouble() < boost*Probability;
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
