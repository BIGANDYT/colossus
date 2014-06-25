using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public interface IRandomValue<out TValue> : IRandomValue
    {
        new TValue Value { get; }
    }

    public interface IRandomValue
    {
        IRandomVariable Generator { get; }
        object Value { get; }

        IEnumerable<IRandomVariable> Correlations { get; }

        void Update(Visit visit);
    }
}