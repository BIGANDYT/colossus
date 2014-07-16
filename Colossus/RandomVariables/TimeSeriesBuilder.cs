using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Colossus.RandomVariables
{
    public class TimeSeriesBuilder : RandomVariable
    {
        private Dictionary<IEnumerable<IRandomVariable>, double> _parts;

        public Func<IRandomDistribution, IRandomVariable> Factory { get; set; }

        public double Min { get; private set; }
        public double Max { get; private set; }

        public TimeSeriesBuilder(Func<IRandomDistribution, IRandomVariable> factory, double min, double max)
        {
            _parts = new Dictionary<IEnumerable<IRandomVariable>, double>();
            Min = min;
            Max = max;
            Factory = factory;
        }


        /// <summary>
        /// Uniform distribution
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public TimeSeriesBuilder Uniform(double weight = 1d)
        {
            _parts.Add(new[] { Factory(new RandomLinear(Min, Max)) }, weight);
            return this;
        }

        /// <summary>
        /// Use this method to do nothing with the specified weight. This allows mixing values with trends from higher level variables. E.g. an hour variable will override a year variable, but with this method the year variable can decide the hour a specified fraction of the times. )
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public TimeSeriesBuilder Mute(double weight = 1d)
        {
            _parts.Add(new[] { new NoOpVariable() }, weight);
            return this;
        }

        /// <summary>
        /// Draw trend lines
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="startLevel"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public TrendBuilder<TimeSeriesBuilder> DrawTrend(double? offset = null, double startLevel = 1, double weight = 1)
        {
            return new TrendBuilder<TimeSeriesBuilder>(var =>
            {
                _parts.Add(new[] { var }, weight);
                return this;
            }, offset ?? Min, startLevel, Factory);
        }

        public TimeSeriesBuilder LinearTrend(double startLevel = 1d, double endLevel = 2d)
        {
            return DrawTrend(Min, startLevel).LineTo(Max, endLevel).Close();
        }

        /// <summary>
        /// Add a peak from the skewed normal distribution
        /// </summary>
        /// <param name="location"></param>
        /// <param name="scale"></param>
        /// <param name="shape"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public TimeSeriesBuilder AddPeak(double location, double scale, double shape = 0, double weight = 1)
        {
            CheckBuild();

            var range = (Max - Min);
            var random = new TruncatedRandom(new RandomSkewNormal(0, scale, shape), -range / 2d, range / 2d, location + range);

            _parts.Add(new[] { Factory(random) }, weight);
            return this;
        }

        private IRandomVariable _variable;

        public TimeSeriesBuilder Build()
        {
            if (_variable == null)
            {
                if (_parts.Count == 0)
                {
                    _variable = Factory(new RandomLinear(Min, Max));
                }
                else
                {
                    _variable = new RandomFork(new DiscreteSampleSet<IEnumerable<IRandomVariable>>(_parts));
                }
            }
            return this;
        }

        void CheckBuild()
        {
            if (_variable != null) throw new InvalidOperationException("Parts can no longer be added to this builder since it has been used");
        }




        public override IRandomValue Sample(SampleContext context = null)
        {
            Build();
            return _variable.Sample(context);
        }
    }

}
