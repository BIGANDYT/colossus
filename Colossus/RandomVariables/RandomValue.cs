using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public class RandomValue<TValue> : IRandomValue<TValue>
    {
        public IRandomVariable Generator { get; private set; }
        public TValue Value { get; private set; }
        public IEnumerable<IRandomVariable> Correlations { get; set; }
        

        private Action<Visit> _updater;

        public RandomValue(IRandomVariable generator, TValue value,
            Action<Visit> updater, IEnumerable<IRandomVariable> correlations = null)
        {
            Generator = generator;
            Value = value;
            Correlations = correlations ?? Enumerable.Empty<IRandomVariable>();
            _updater = updater;
        }

        public void Update(Visit visit)
        {
            if (_updater != null)
            {
                _updater(visit);
            }
        }

        TValue IRandomValue<TValue>.Value
        {
            get { return Value; }
        }

        object IRandomValue.Value
        {
            get { return Value; }
        }
        
    }
}