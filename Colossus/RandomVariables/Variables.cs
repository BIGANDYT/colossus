using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public static class Variables
    {
        public static WeightedTag<TValue> Weighted<TValue>(string key, IDictionary<TValue, double> weights)
        {
            return new WeightedTag<TValue>(key, weights);
        }

        public static FixedTag<TValue> Fixed<TValue>(string key, TValue value)
        {
            return new FixedTag<TValue>(key, value);
        }


        public static GoalVariable Goal(Goal goal, double probability)
        {
            return new GoalVariable(goal, probability);
        }
        
    }
}