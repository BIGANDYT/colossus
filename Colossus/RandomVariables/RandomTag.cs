using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public class RandomTag<TValue> : TagVariable
    {
        public SampleSet<TValue> Set { get; set; }

        public RandomTag(string key, SampleSet<TValue> values)
            : base(key)
        {
            Set = values;
            Correlations = new Dictionary<TValue, IRandomVariable[]>();
        }

        public Dictionary<TValue, IRandomVariable[]> Correlations { get; set; }

        public override IRandomValue Sample(SampleContext context = null)
        {            
            var value = Set.Sample();
            if (value == null) return null;

            return new RandomValue<TValue>(this, value, v=>v.Tags[Key] = value, Correlations.GetOrDefault(value) ?? Enumerable.Empty<IRandomVariable>());
        }
    }
}