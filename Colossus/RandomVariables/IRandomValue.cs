using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public interface IRandomValue<TValue> : IRandomValue
    {
        TValue Value { get; set; }
    }

    public interface IRandomValue
    {
        IRandomVariable Generator { get; }
       
        IEnumerable<IRandomVariable> Correlations { get; set; }

        void Update(Visit visit);
    }
}