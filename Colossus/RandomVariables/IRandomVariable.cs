using System;

namespace Colossus.RandomVariables
{   
    public interface IRandomVariable
    {
        object Key { get;}

        IRandomValue Sample(SampleContext context = null);
    }
}