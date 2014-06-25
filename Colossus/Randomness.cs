using System;
using System.Threading;

namespace Colossus
{
    /// <summary>
    /// Always use a seeded random number generator to make results reproducible
    /// </summary>
    static class Randomness
    {
        private static int _increment;
        public static int Seed = 1337;

        [ThreadStatic] private static Random _random;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random(Seed + _increment);
                    Interlocked.Increment(ref _increment);
                }

                return _random;
            }
        }
    }
}
