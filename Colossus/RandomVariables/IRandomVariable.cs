using System;

namespace Colossus.RandomVariables
{
    public interface IRandomVariable<out TKey, out TValue> : IRandomVariable
    {
        new TKey Key { get; }

        new IRandomValue<TValue> Sample(SampleContext context = null, Random random = null);
    }

    public interface IRandomVariable
    {
        object Key { get; }

        IRandomValue Sample(SampleContext context = null, Random random = null);
    }
}