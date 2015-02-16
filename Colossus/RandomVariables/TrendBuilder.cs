using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.RandomVariables
{
    public class TrendBuilder<TOwner>
    {
        struct LinearStep
        {
            public double Start, End, StartLevel, EndLevel, Area;
        }

        private List<LinearStep>  _steps = new List<LinearStep>();
        private double _totalArea = 0;
        private readonly Func<IRandomVariable, TOwner> _owner;
        private double _currentStart, _currentLevel;
        private readonly Func<IRandomDistribution, IRandomVariable> _factory;

        public TrendBuilder(Func<IRandomVariable, TOwner> owner, double start, double level, Func<IRandomDistribution, IRandomVariable> factory)
        {
            _owner = owner;
            _currentStart = start;
            _currentLevel = level;
            _factory = factory;
        }

        public TrendBuilder<TOwner> LineTo(double time, double? level = null)
        {
            if( time < _currentStart) throw new ArgumentException("Steps must be added in time order", "time");

            var step = new LinearStep
            {
                Start = _currentStart,
                StartLevel = _currentLevel,
                End = (_currentStart = time),
                EndLevel = (_currentLevel = level ?? _currentLevel)
            };
            
            step.Area = (step.End - step.Start) * (step.StartLevel + 0.5 * (step.EndLevel - step.StartLevel));
            if (step.Area > 0)
            {
                _totalArea += step.Area;
                _steps.Add(step);
            }
            return this;
        }

        public TrendBuilder<TOwner> JumpTo(double time, double level)
        {
            if (time < _currentStart) throw new ArgumentException("Steps must be added in time order", "time");

            _currentStart = time;
            _currentLevel = level;

            return this;
        }

        public TOwner Close()
        {
            var randoms =
                _steps.Where(s => s.Area > 0).Select(s => new KeyValuePair<IEnumerable<IRandomVariable>, double>(new[]
                {
                    _factory(new RandomLinear(s.Start, s.End, s.StartLevel, s.EndLevel))
                }, s.Area/_totalArea)).ToArray();           
            
            return randoms.Length > 0 ? _owner(new RandomFork(new DiscreteSampleSet<IEnumerable<IRandomVariable>>(randoms))) : _owner(new NoOpVariable());
        }
    }
}
