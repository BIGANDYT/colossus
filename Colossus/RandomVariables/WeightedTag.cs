using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public class WeightedTag<TValue> : TagVariable
    {
        public SampleSet<TValue> Set { get; set; }

        public WeightedTag(string key, IDictionary<TValue, double> weights)
            : base(key)
        {           
            Set = new SampleSet<TValue>(weights);
            Correlations = new Dictionary<TValue, IRandomVariable[]>();
        }

        public Dictionary<TValue, IRandomVariable[]> Correlations { get; set; }

        public override IRandomValue Sample(SampleContext context = null)
        {            
            var value = Set.Sample(Random);
            if (value == null) return null;

            return new RandomValue<TValue>(this, value, v=>v.Tags[Key] = value, Correlations.GetOrDefault(value) ?? Enumerable.Empty<IRandomVariable>());
        }
    }
}