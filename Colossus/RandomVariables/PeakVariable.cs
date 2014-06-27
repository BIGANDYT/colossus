using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public abstract class PeakVariable : ContinousVariable
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double? Offset { get; set; }
        public double Spread { get; set; }

        protected PeakVariable(double min, double max, double? offset = null, double spread = 0)
        {
            Min = min;
            Max = max;
            Offset = offset;
            Spread = spread;
        }

        public override IRandomValue Sample(SampleContext context = null)
        {
            for (;;)
            {
                var value = Offset.HasValue
                    ? (Spread > 0 ? Random.Peak(Min, Max, Offset.Value, Spread) : Min + Offset.Value)
                    : Min + Random.NextDouble()*(Max - Min);

                if (value < Min || value >= Max) continue;

                return new RandomValue<double>(this, value, visit => Action(visit, value), GetCorrelations(value));
            }
        }

        protected abstract void Action(Visit visit, double value);
    }
}
