using System;
using System.Collections.Generic;
using System.Linq;


namespace Colossus.RandomVariables
{
    public abstract class ContinousVariable : RandomVariable
    {
        protected ContinousVariable(Random random = null) : base(random)
        {
            Key = GetType();
            Correlations = new List<Func<double, IRandomVariable[]>>();
        }

        protected IEnumerable<IRandomVariable> GetCorrelations(double value)
        {
            if (Correlations != null)
            {
                return Correlations.SelectMany(c => c(value));                
            }

            return Enumerable.Empty<IRandomVariable>();
        }

        public List<Func<double, IRandomVariable[]>> Correlations { get; private set; }

        public ContinousVariable Correlate(double? min = null, double? max = null, params IRandomVariable[] variables)
        {
            Correlations.Add(v => (!min.HasValue || v >= min) && (!max.HasValue || v < max) ? variables : new IRandomVariable[0]);

            return this;
        }
    }
}
