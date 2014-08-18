using System;
using System.Collections.Generic;
using System.Text;

namespace Colossus.RandomVariables
{
    public static class Variables
    {
        public static RandomTag<TValue> Random<TValue>(string key, SampleSet<TValue> set)
        {
            return new RandomTag<TValue>(key, set);
        }

        public static RandomTag<TValue> Random<TValue>(string key, IDictionary<TValue, double> weights)
        {
            return new RandomTag<TValue>(key, new DiscreteSampleSet<TValue>(weights));
        }

        public static RandomTag<TValue> Random<TValue>(string key, object[,] weights)
        {
            var d = new Dictionary<TValue, double>();
            for (int i = 0, n = weights.GetLength(0); i < n; i++)
            {
                d.Add((TValue) weights[i, 0], (double) weights[i, 1]);
            }
            return new RandomTag<TValue>(key, new DiscreteSampleSet<TValue>(d));
        }

        public static ContinuousVariable Continuous(string key, IRandomDistribution dist)
        {
            return new ContinuousVariable(key, dist);
        }

        public static FixedTag<TValue> Fixed<TValue>(string key, TValue value)
        {
            return new FixedTag<TValue>(key, value);
        }


        public static GoalVariable Goal(Goal goal, double probability)
        {
            return new GoalVariable(goal, probability);
        }
        

        public static TimeSeriesBuilder Hour()
        {
            return new TimeSeriesBuilder(r=>new RandomHour(r), 0, 24);
        }

        public static TimeSeriesBuilder DayOfWeek()
        {
            return new TimeSeriesBuilder(r => new RandomDayOfWeek(r), 0, 7);
        }

        public static TimeSeriesBuilder DayOfYear()
        {
            return new TimeSeriesBuilder(r => new RandomDayOfYear(r), 0, 366);
        }

        public static TimeSeriesBuilder Year(double min, double max)
        {
            return new TimeSeriesBuilder(r => new RandomYear(r), min, max);
        }

        public static RandomFork MultiSet(Action<DiscreteSampleSet<IEnumerable<IRandomVariable>>.SetBuilder> sets)
        {

            var builder = new DiscreteSampleSet<IEnumerable<IRandomVariable>>.SetBuilder();

            sets(builder);

            return new RandomFork(builder.Build());
        }
    }
}