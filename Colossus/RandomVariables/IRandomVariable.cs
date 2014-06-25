using System;

namespace Colossus.RandomVariables
{
    public interface IRandomVariable<out TKey, out TValue> : IRandomVariable
    {
        TKey Key { get; }

        IRandomValue<TValue> Sample(Random random = null);
    }

    public interface IRandomVariable
    {
        object Key { get; }

        IRandomValue Sample(Random random = null);
    }
}