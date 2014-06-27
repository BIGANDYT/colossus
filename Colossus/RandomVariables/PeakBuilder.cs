using System;
using System.Collections.Generic;

namespace Colossus.RandomVariables
{
    public class PeakBuilder
    {
        public Dictionary<IEnumerable<IRandomVariable>, double> Peaks { get; set; }

        public Func<double?, double, IRandomVariable> Factory { get; set; }

        public PeakBuilder(Func<double?, double, IRandomVariable> factory)
        {
            Peaks = new Dictionary<IEnumerable<IRandomVariable>, double>();
            Factory = factory;           
        }



        public PeakBuilder AddPeak(double? offset, double spread = 0, double percentage = 1)
        {
            Peaks.Add(new []{Factory(offset, spread)}, percentage);
            return this;
        }

        public IRandomVariable Build()
        {
            if (Peaks.Count == 0)
            {
                return Factory(null, 0);
            }

            return new VariableFork(Peaks);
        }        
    }
  
}
