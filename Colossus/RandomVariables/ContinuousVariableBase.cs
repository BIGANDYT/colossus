using System;
using System.Collections.Generic;
using System.Linq;


namespace Colossus.RandomVariables
{
    public abstract class ContinuousVariableBase<TKey> : RandomVariable<TKey>
    {
        public List<Func<double, IEnumerable<IRandomVariable>>> Correlations { get; private set; }

        protected ContinuousVariableBase(TKey key, IRandomDistribution random) : base(key, random)
        {
            Correlations = new List<Func<double, IEnumerable<IRandomVariable>>>();
        }

        protected IEnumerable<IRandomVariable> GetCorrelations(double value)
        {
            if (Correlations != null)
            {
                return Correlations.SelectMany(c => c(value));                
            }

            return Enumerable.Empty<IRandomVariable>();
        }

        public override IRandomValue Sample(SampleContext context = null)
        {
            var value = Random.Next();
            return new RandomValue<double>(this, value, v => Action(v, value), GetCorrelations(value));
        }

        protected abstract void Action(Visit visit, double value);        
        

        

        public ContinuousVariableBase<TKey> Correlate(double? min = null, double? max = null, params IRandomVariable[] variables)
        {
            Correlations.Add(v => (!min.HasValue || v >= min) && (!max.HasValue || v < max) ? variables : new IRandomVariable[0]);

            return this;
        }

        public ContinuousVariableBase<TKey> Correlate(Func<double, bool> selector, params IRandomVariable[] variables)
        {
            Correlations.Add(v=>selector(v) ? variables : new IRandomVariable[0]);

            return this;
        }

        public ContinuousVariableBase<TKey> Correlate(Func<double, IEnumerable<IRandomVariable>> selector)
        {
            Correlations.Add(selector);

            return this;
        }
    }
}
