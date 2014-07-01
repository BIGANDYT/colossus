using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public static class Sets
    {
        public static DiscreteSampleSet<TValue>.SetBuilder<TValue> Weight<TValue>()
        {
            return new DiscreteSampleSet<TValue>.SetBuilder<TValue>();
        }

        public static DiscreteSampleSet<TValue>.SetBuilder<TValue> Weight<TValue>(TValue value, double weight)
        {            
            return Weight<TValue>().Weight(value, weight);
        }

        public static DistributedSampleSet<TValue> Distribution<TValue>(IRandomDistribution dist,
            params TValue[] values)
        {
            return new DistributedSampleSet<TValue>(values, dist);
        } 

        public static SampleSet<TValue> Uniform<TValue>(params TValue[] values)
        {
            return new DistributedSampleSet<TValue>(values);
        }

        public static SampleSet<TValue> Exponential<TValue>(TValue[] values, double topPerecent, int index)
        {
            return new DistributedSampleSet<TValue>(values, RandomExponential.TopPerecentage(topPerecent, index, values.Length));
        }

        public static SampleSet<TValue> Pareto<TValue>(TValue[] values, double topPerecent, int index)
        {
            return new DistributedSampleSet<TValue>(values, RandomPareto.TopPerecentage(topPerecent, index, maxValue: values.Length));
        }
    }

    public abstract class SampleSet<TValue>
    {
        public TValue[] Values { get; protected set; }

        public abstract TValue Sample();
    }

    public class DistributedSampleSet<TValue> : SampleSet<TValue>
    {        
        public IRandomDistribution Distribution { get; set; }

        public DistributedSampleSet(TValue[] values, IRandomDistribution distribution = null)
        {
            Values = values;
            Distribution = distribution ?? new RandomLinear(0, values.Length);
        }

        public override TValue Sample()
        {
            var index = (int) Math.Floor(Distribution.Next());
            return Values[index];
        }
    }

    public class DiscreteSampleSet<T> : SampleSet<T>
    {
        public IEnumerable<KeyValuePair<T, double>> Items { get; private set; }
        private readonly Random _random;
        
        private double[] _weights;
        private double _totalWeight;
        

        public DiscreteSampleSet(IEnumerable<KeyValuePair<T, double>> items, Random random = null)
        {
            Items = items.ToArray();

            Values = Items.Select(x => x.Key).ToArray();

            _random = random ?? Randomness.Random;                        
            _weights = items.Select(x => _totalWeight += x.Value).ToArray();
        }


        public override T Sample()
        {
            var n = _random.NextDouble() * _totalWeight;
            for (var i = 0; i < _weights.Length; i++)
            {
                if (n < _weights[i]) return Values[i];
            }

            throw new Exception("What?!");
        }

        public class SetBuilder<TValue>
        {
            private List<KeyValuePair<TValue, double>> _weights;
            public SetBuilder()
            {
                _weights = new List<KeyValuePair<TValue, double>>();
            }

            public SetBuilder<TValue> Weight(TValue value, double weight)
            {
                _weights.Add(new KeyValuePair<TValue, double>(value, weight));
                return this;
            }

            public DiscreteSampleSet<TValue> Build()
            {
                return new DiscreteSampleSet<TValue>(_weights);
            } 

            public static implicit operator SampleSet<TValue>(SetBuilder<TValue> builder)
            {
                return builder.Build();
            }
        }

    }
}