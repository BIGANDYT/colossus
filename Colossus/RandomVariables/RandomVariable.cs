using System;

namespace Colossus.RandomVariables
{
    public abstract class RandomVariable<TKey, TValue> : IRandomVariable<TKey, TValue>
    {
        object IRandomVariable.Key
        {
            get { return Key; }
        }

        public abstract IRandomValue<TValue> Sample(SampleContext context = null, Random random = null);


        public TKey Key { get; set; }

        IRandomValue IRandomVariable.Sample(SampleContext context = null, Random random = null)
        {
            return Sample(context, random);
        }
    }
}