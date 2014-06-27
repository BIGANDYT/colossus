using System;
using System.Threading;

namespace Colossus
{
    /// <summary>
    /// Always use a seeded random number generator to make results reproducible
    /// </summary>
    public static class Randomness
    {
        private static int _increment;
        public static int Seed = 1305454;

        [ThreadStatic] private static Random _random;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    Interlocked.Increment(ref _increment);
                    _random = new Random(Seed + _increment);                    
                }

                return _random;
            }
        }        
    }

    public static class RandomHelpers
    {
        public static double NextNormal(this Random random, double mean, double sd)
        {
            //Box-Muller transform

            var u1 = random.NextDouble();
            var u2 = random.NextDouble();

            return mean + sd * Math.Sqrt(-2*Math.Log(u1))*Math.Cos(2*Math.PI*u2);
        }

        public static double Peak(this Random random, double min, double max, double offset,
            double fiftyPercentWillBeWithinTheOffsetPlusMinusThisValue)
        {
            var sd = fiftyPercentWillBeWithinTheOffsetPlusMinusThisValue*1.5d;
            var mid = (max - min)/2;

            double value;
            do
            {
                value = random.NextNormal(0, sd);
            } while (value < mid && value >= max);            
            return (value + offset)%(mid*2);
        }
    }
}
