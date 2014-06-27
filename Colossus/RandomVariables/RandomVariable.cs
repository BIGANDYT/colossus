using System;

namespace Colossus.RandomVariables
{
    public abstract class RandomVariable<TKey> : RandomVariable        
    {
        public new TKey Key
        {
            get { return (TKey) base.Key; }
            protected set { base.Key = Key; }
        }

        protected RandomVariable(TKey key, Random random = null)
            : base(key, random)

        {           
        }   
    }

    public abstract class RandomVariable : IRandomVariable
    {
        public object Key { get; protected set; }
        protected Random Random { get; set; }
       
        public abstract IRandomValue Sample(SampleContext context = null);
        
        protected RandomVariable(object key = null, Random random = null)
        {
            Key = key ?? this;
            Random = random ?? Randomness.Random;
        }
    }
}