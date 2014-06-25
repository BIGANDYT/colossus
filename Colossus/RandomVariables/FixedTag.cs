using System;

namespace Colossus.RandomVariables
{
    public class FixedTag<TValue> : RandomVariable<string, TValue>
    {        
        public TValue Value { get; set; }

        public FixedTag(string key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public override IRandomValue<TValue> Sample(Random random = null)
        {
            return new RandomValue<TValue>(this, Value, v=>v.Tags[Key] = Value);
        }
    }
}