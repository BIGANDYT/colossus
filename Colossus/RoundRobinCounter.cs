using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class RoundRobinCounter
    {
        public int[] Counts { get; private set; }

        private readonly int[] _indices;
        
        public RoundRobinCounter(int[] counts, bool randomize = true)
        {
            Counts = counts;
            //Initialize indices randomly to break order when multiple counters are used
            _indices = counts.Select(c=>randomize ? Randomness.Random.Next(0, c) : 0).ToArray();
        }

        public IEnumerable<int[]> All
        {
            get
            {
                var rr = new RoundRobinCounter(Counts, false);
                var n = Counts.Aggregate(1, (current, c) => current *= c);

                for (var i = 0; i < n; i++)
                {
                    yield return rr.Next;
                }
            }
        }

        public int[] Next
        {
            get
            {
                var ix = _indices.ToArray();
                Increment();
                return ix;
            }
        }

        void Increment()
        {
            var i = _indices.Length - 1;
            while (i >= 0 && ++_indices[i] == Counts[i])
            {
                _indices[i] = 0;
                --i;
            }
        }
    }
}