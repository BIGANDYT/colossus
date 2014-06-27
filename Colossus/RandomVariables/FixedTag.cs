using System;

namespace Colossus.RandomVariables
{
    public class FixedTag<TValue> : RandomVariable
    {        
        public TValue Value { get; set; }

        public FixedTag(string key, TValue value) : base(key)
        {            
            Value = value;
        }

        public override IRandomValue Sample(SampleContext context = null)
        {
            return new RandomValue<TValue>(this, Value, v=>v.Tags[(string)Key] = Value);
        }
    }
}