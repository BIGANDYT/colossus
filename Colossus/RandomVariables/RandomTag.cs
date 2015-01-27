using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public class RandomTag<TValue> : TagVariable
    {
        public List<Func<TValue, IEnumerable<IRandomVariable>>> Correlations { get; private set; }


        public Func<TValue> Getter { get; set; }

        public RandomTag(string key, SampleSet<TValue> values)
            : this(key, values.Sample)
        {
            
        }

        public RandomTag(string key, Func<TValue> values)
            : base(key)
        {
            Getter = values;
            Correlations = new List<Func<TValue, IEnumerable<IRandomVariable>>>();
        }

        protected IEnumerable<IRandomVariable> GetCorrelations(TValue value)
        {
            if (Correlations != null)
            {
                return Correlations.SelectMany(c => c(value));
            }

            return Enumerable.Empty<IRandomVariable>();
        }
        

        public override IRandomValue Sample(SampleContext context = null)
        {            
            var value = Getter();
            if (value == null) return null;

            return new RandomValue<TValue>(this, value, v=>v.Tags[Key] = value, GetCorrelations(value));
        }

        public RandomTag<TValue> Correlate(TValue selector, params IRandomVariable[] variables)
        {
            Correlations.Add(v => v.Equals(selector) ? variables : new IRandomVariable[0]);

            return this;
        }

        public RandomTag<TValue> Correlate(Func<TValue, bool> selector, params IRandomVariable[] variables)
        {
            Correlations.Add(v => selector(v) ? variables : new IRandomVariable[0]);

            return this;
        }

        public RandomTag<TValue> Correlate(Func<TValue, IEnumerable<IRandomVariable>> selector)
        {
            Correlations.Add(selector);

            return this;
        }
    }
}