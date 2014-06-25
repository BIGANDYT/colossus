using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class SampleSet<T>
    {
        public IDictionary<T, double> Items { get; private set; }
        private readonly Random _random;
        public T[] Values { get; private set; }
        private double[] _weights;
        private double _totalWeight;
        

        public SampleSet(IDictionary<T, double> items)
        {
            Items = items;
            Values = items.Select(x => x.Key).ToArray();

            _random = Randomness.Random;                        
            _weights = items.Select(x => _totalWeight += x.Value).ToArray();
        }


        public T Sample(Random random = null)
        {
            var n = (random ?? _random).NextDouble() * _totalWeight;
            for (var i = 0; i < _weights.Length; i++)
            {
                if (n < _weights[i]) return Values[i];
            }

            throw new Exception("What?!");
        }

        public static SampleSet<T> FromArray(object[,] values)
        {
            var dict = new Dictionary<T, double>();
            for (var i = 0; i < values.GetLength(0); i++)
            {
                dict.Add((T) values[i, 0], (double) values[i, 1]);
            }

            return new SampleSet<T>(dict);
        }
    }
}