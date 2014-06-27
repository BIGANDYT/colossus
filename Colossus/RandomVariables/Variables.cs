using System.Collections.Generic;
using System.Text;

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


        public static PeakBuilder Hour()
        {
            return new PeakBuilder((o,s)=>new RandomHour(o,s));
        }

        public static PeakBuilder DayOfWeek()
        {
            return new PeakBuilder((o, s) => new RandomDayOfWeek(o, s));
        }

        public static PeakBuilder DayOfYear()
        {
            return new PeakBuilder((o, s) => new RandomDayOfYear(o, s));
        }

        public static PeakBuilder Year(int min, int max)
        {
            return new PeakBuilder((o, s) => new RandomYear(min, max, o, s));
        }
    }
}