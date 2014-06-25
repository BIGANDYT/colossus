using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public class WeightedTag<TValue> : RandomVariable<string, TValue>
    {
        public SampleSet<TValue> Set { get; set; }

        public WeightedTag(string key, IDictionary<TValue, double> weights)
        {
            Key = key;
            Set = new SampleSet<TValue>(weights);

            Correlations = new Dictionary<TValue, IRandomVariable[]>();
        }

        public Dictionary<TValue, IRandomVariable[]> Correlations { get; set; }

        public override IRandomValue<TValue> Sample(Random random = null)
        {
            var value = Set.Sample(random);
            if (value == null) return null;

            return new RandomValue<TValue>(this, value, v=>v.Tags[Key] = value, Correlations.GetOrDefault(value) ?? Enumerable.Empty<IRandomVariable>());
        }
    }
}