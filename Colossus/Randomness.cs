using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Threading;

namespace Colossus
{
    /// <summary>
    /// Always use a seeded random number generator to make results reproducible
    /// </summary>
    public static class Randomness
    {
        private static int _increment;
        public static int Seed = 1338;

        [ThreadStatic]
        private static Random _random;
        

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random(Seed + Interlocked.Increment(ref _increment));                    
                }

                return _random;
            }
        }

        public static void Reset(int? seed = null)
        {
            _random = new Random(seed ?? Seed);
        }
    }

    public interface IRandomDistribution
    {
        double Next();
    }

    public class RandomNormal : IRandomDistribution
    {
        private Random _random;
        public double Location { get; set; }
        public double Scale { get; set; }

        public RandomNormal(double location, double scale)
        {
            _random = Randomness.Random;
            Location = location;
            Scale = scale;
        }

        public double Next()
        {
            var u1 = _random.NextDouble();
            var u2 = _random.NextDouble();

            return Location + Scale * Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }
    }


    public class RandomLinear : IRandomDistribution
    {
        private Random _random;
        public double Min { get; set; }
        public double Max { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public double Increase { get; set; }

        public RandomLinear(double min = 0, double max = 1, double start = 1, double end = 1)
        {
            _random = Randomness.Random;
            Min = min;
            Max = max;
            Start = start;
            End = end;
        }


        double Cdf(double t)
        {
            return 0.5 * (End * t * t - Start * t * t) + Start * t;
        }

        double Quantile(double u)
        {
            return (Start - Math.Sqrt(2 * End * u + Start * Start - 2 * Start * u)) / (Start - End);
        }

        public double Next()
        {
            if (Min == Max) return Min;

            if (Start == End)
            {
                return Min + (Max - Min) * _random.NextDouble();
            }

            var u = _random.NextDouble() * (Cdf(1) - Cdf(0));
            return Min + (Max - Min) * Quantile(u);
        }

        public static RandomLinear Fixed(double value)
        {
            return new RandomLinear(value, value);
        }
    }

    public class RandomExponential : IRandomDistribution
    {
        private Random _random;
        public double Lambda { get; set; }
        public double? MaxValue { get; set; }

        public RandomExponential(double lambda, double? maxValue = null)
        {
            _random = Randomness.Random;
            Lambda = lambda;
            MaxValue = maxValue;
        }

        public static double Cdf(double x, double lambda)
        {
            return 1 - Math.Exp(-lambda * x);
        }

        public static double Quantile(double u, double lambda)
        {
            return -Math.Log(1 - u) / lambda;
        }

        public double Next()
        {
            return Quantile((MaxValue.HasValue ? Cdf(MaxValue.Value, Lambda) : 1) * _random.NextDouble(), Lambda);
        }

        public static RandomExponential TopPerecentage(double topPct, double belowValue, double? maxValue = null)
        {
            var lambda = -Math.Log(1 - topPct) / belowValue;
            if (maxValue.HasValue)
            {
                //Can't find closed form expression for a. Nor can http://www.wolframalpha.com/ 
                lambda =
                    Statistics.BrentsMethodSolve(
                        l => Cdf(belowValue, l) / Cdf(maxValue.Value, l) - topPct, 1e-5, lambda);                                               
            }
            return new RandomExponential(lambda, maxValue);
        }
    }

    public class RandomSkewNormal : IRandomDistribution
    {
        private RandomNormal _normal;
        public double Location { get; set; }
        public double Scale { get; set; }
        public double Shape { get; set; }

        public RandomSkewNormal(double location, double scale, double shape)
        {
            _normal = new RandomNormal(0, 1);
            Location = location;
            Scale = scale;
            Shape = shape;
        }

        public double Next()
        {
            var r = Shape / Math.Sqrt(1 + Shape * Shape);
            var u0 = _normal.Next();
            var v = _normal.Next();
            var u1 = r * u0 + Math.Sqrt(1 - r * r) * v;
            return (u0 >= 0 ? u1 : -u1) * Scale + Location;
        }
    }
    

    public class TruncatedRandom : IRandomDistribution
    {
        public IRandomDistribution Inner { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Offset { get; set; }

        public TruncatedRandom(IRandomDistribution inner, double min = 0, double max = 0, double offset = 0)
        {
            Inner = inner;
            Min = min;
            Max = max;
            Offset = offset;
        }

        public double Next()
        {            
            double value;
            do
            {
                value = Inner.Next();
            } while (value < Min || value >= Max);          

            return Min + (value - Min + Offset)%(Max - Min);            
        }
    }

    public class RandomTransformation : IRandomDistribution
    {
        public IRandomDistribution Distribution { get; set; }
        public Func<double, double> Transformation { get; set; }

        public RandomTransformation(IRandomDistribution distribution, Func<double, double> transformation)
        {
            Distribution = distribution;
            Transformation = transformation;
        }

        public double Next()
        {
            return Transformation(Distribution.Next());
        }
    }    
}
