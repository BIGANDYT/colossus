using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Colossus;


namespace Colossus
{
    public static class Statistics
    {
        private const int MaxIterations = 10000;
        private const double Eps = 1e-8; //We don't care about numbers smaller than this        

        public static double ChiSquare(double chiSquare, int k)
        {
            return Math.Exp(LogChiSquare(chiSquare, k));
        }

        public static double LogChiSquare(double chiSquare, int k)
        {
            return LogIGamma(k/2d, chiSquare/2) - LogGamma2(k);
        }


        public static double Power(double w, double n, double sigLevel, int k)
        {
            var crit = FindCriticalChiSquare(k, sigLevel);
            var lambda = w*w*n;

            return Power(crit, lambda, k);
        }

        private static double Power(double crit, double lambda, int k)
        {
            if (lambda == 0) return 0;

            var sum = 0d;
            var logPs = 0d;

            for (var m = 0; m < MaxIterations; m++)
            {
                var step = Math.Exp(logPs + LogChiSquare(crit, k + 2*m));
                sum += step;

                if (step < Eps || double.IsInfinity(sum))
                {
                    break;
                }
                logPs += Math.Log((lambda/2)/(m + 1));
            }


            return 1 - (double.IsInfinity(sum) ? 0 : Math.Exp(-lambda/2d)*sum);
        }


        public static double Forecast(double w, int k, double sigLevel, double power)
        {
            if (power == 1 || sigLevel == 0) return double.PositiveInfinity;
            var crit = FindCriticalChiSquare(k, sigLevel);

            double result = 0;
            for (var i = 0; i < 3; i++)
            {
                //Note: High limits takes quite a time to calculate. Thus, we start out low and increase if needed. If the solution is either 0 or limit we know that bracket did not contain the root
                var limit = Math.Pow(10, 4 + i);
                result = BrentsMethodSolve(n => Power(crit, n*w*w, k) - power, 0, limit);
                if (result > 0 && result < limit)
                {
                    break;
                }
            }

            return result;
        }




        public static double LogIGamma(double s, double x)
        {
            //Math.Pow(x, s) * Math.Exp(-x)
            //Log( Math.Pow(x, s) * Math.Exp(-x) ) =
            //s * Log(x) - x


            var step = 1/s;
            var sum = 0d;
            for (var m = 0; m < MaxIterations; m++)
            {
                sum += step;

                step *= x/(s + m + 1);
                if (step < Eps) break;
            }

            return s*Math.Log(x) - x + Math.Log(sum);
        }

        public static double LogGamma2(int k)
        {
            if (k%2 == 0) //Even
            {
                return LogFac(k/2 - 1);
            }
            else //Odd
            {
                var n = (k - 1)/2;
                var nom = LogFac(2*n);
                return nom - n*Math.Log(4) - LogFac(n) + 0.5*Math.Log(Math.PI);
            }
        }


        public static double LogFac(int n)
        {
            var r = Math.Log(1);
            for (var i = 2; i <= n; i++)
            {
                r += Math.Log(i);
            }

            return r;
        }

        public static double HalfDivisionSolve(Func<double, double> f, double a, double b)
        {
            for (var i = 0; i < MaxIterations; i++)
            {
                var mid = (a + b)/2;
                var v = f(mid);
                if (Math.Abs(v) < Eps)
                {
                    Console.Out.WriteLine("{0} hd its", i);
                    return mid;
                }
                if (v > 0)
                {
                    a = mid;
                }
                else
                {
                    b = mid;
                }
            }

            throw new Exception("Yikes!");
        }

        public static double FindCriticalChiSquare(int k, double sigLevel = 0.05d)
        {
            return BrentsMethodSolve(chisq => 1 - ChiSquare(chisq, k) - sigLevel, 0, Math.Max(100, k*10));
        }

        //Stole this one from Wikipedia
        public static double BrentsMethodSolve(Func<double, double> function, double lowerLimit, double upperLimit,
            double errorTol = Eps)
        {
            double a = lowerLimit;
            double b = upperLimit;
            double c = 0;
            double d = double.MaxValue;

            double fa = function(a);
            double fb = function(b);

            double fc = 0;
            double s = 0;
            double fs = 0;

            // if f(a) f(b) >= 0 then error-exit
            if (fa*fb >= 0)
            {
                if (fa < fb)
                    return a;
                else
                    return b;
            }

            // if |f(a)| < |f(b)| then swap (a,b) end if
            if (Math.Abs(fa) < Math.Abs(fb))
            {
                double tmp = a;
                a = b;
                b = tmp;
                tmp = fa;
                fa = fb;
                fb = tmp;
            }

            c = a;
            fc = fa;
            bool mflag = true;
            int i = 0;

            while (!(fb == 0) && (Math.Abs(a - b) > errorTol))
            {
                if ((fa != fc) && (fb != fc))
                    // Inverse quadratic interpolation
                    s = a*fb*fc/(fa - fb)/(fa - fc) + b*fa*fc/(fb - fa)/(fb - fc) + c*fa*fb/(fc - fa)/(fc - fb);
                else
                    // Secant Rule
                    s = b - fb*(b - a)/(fb - fa);

                double tmp2 = (3*a + b)/4;
                if ((!(((s > tmp2) && (s < b)) || ((s < tmp2) && (s > b)))) ||
                    (mflag && (Math.Abs(s - b) >= (Math.Abs(b - c)/2))) ||
                    (!mflag && (Math.Abs(s - b) >= (Math.Abs(c - d)/2))))
                {
                    s = (a + b)/2;
                    mflag = true;
                }
                else
                {
                    if ((mflag && (Math.Abs(b - c) < errorTol)) || (!mflag && (Math.Abs(c - d) < errorTol)))
                    {
                        s = (a + b)/2;
                        mflag = true;
                    }
                    else
                        mflag = false;
                }
                fs = function(s);
                d = c;
                c = b;
                fc = fb;
                if (fa*fs < 0)
                {
                    b = s;
                    fb = fs;
                }
                else
                {
                    a = s;
                    fa = fs;
                }

                // if |f(a)| < |f(b)| then swap (a,b) end if
                if (Math.Abs(fa) < Math.Abs(fb))
                {
                    double tmp = a;
                    a = b;
                    b = tmp;
                    tmp = fa;
                    fa = fb;
                    fb = tmp;
                }
                i++;
                if (i > MaxIterations)
                    throw new Exception(String.Format("Error is {0}", fb));
            }


            return b;
        }


        public static Rank<TValue>[] Ranks<TValue>(this IEnumerable<TValue> values, Func<TValue, double> getter)
        {
            var ranks =
                values.Select(item => new Rank<TValue> {Item = item, Index = getter(item)})
                    .OrderBy(item => item.Index)
                    .ToArray();

            var rank = 1;
            for (var i = 0; i < ranks.Length; i++)
            {
                var rankSum = (double) rank++;
                var i0 = i;

                while (i + 1 < ranks.Length && ranks[i + 1].Index == ranks[i0].Index)
                {
                    rankSum += rank++;
                    ++i;
                }

                for (var j = i0; j <= i; j++)
                {
                    ranks[j].Index = rankSum/(i - i0 + 1);
                }
            }

            return ranks;
        }

        public class Rank<TValue>
        {
            public TValue Item { get; set; }
            public double Index { get; set; }
        }



        public static double[,] CrossTab<TValue>(this IEnumerable<TValue> values,
            Func<TValue, int> group1, Func<TValue, int> group2, int n1, int n2)
        {
            var groups = values.Select(v => Tuple.Create(group1(v), group2(v))).ToArray();

            var result = new double[n1 + 1, n2 + 1];

            foreach (var g in groups)
            {
                ++result[g.Item1, g.Item2];
            }

            for (var i = 0; i < n1; i++)
            {
                for (var j = 0; j < n2; j++)
                {
                    result[n1, j] += result[i, j];
                    result[n1, n2] += result[i, j];
                }
            }

            for (var i = 0; i < n1; i++)
            {
                for (var j = 0; j < n2; j++)
                {
                    result[i, n2] += result[i, j];
                }
            }

            return result;
        }

        public static double Chisq(this double[,] values)
        {
            var n1 = values.GetLength(0) - 1;
            var n2 = values.GetLength(1) - 1;

            var n = values[n1, n2];

            var e = values.Clone() as double[,];
            for (var i = 0; i < n1; i++)
            {
                for (var j = 0; j < n2; j++)
                {
                    e[i, j] = e[i, n2]*e[n1, j]/n;
                }
            }

            var lambda = 0d;
            var effect = 0d;
            for (var i = 0; i < n1; i++)
            {
                for (var j = 0; j < n2; j++)
                {
                    var nom = values[i, j] - e[i, j];
                    lambda += nom*nom/e[i, j];
                    effect += nom*nom/(n*e[i, j]);
                }
            }

            //effect = Math.Sqrt(effect);
            //Console.Out.WriteLine(effect);

            return 1 - ChiSquare(lambda, (n1 - 1)*(n2 - 1));
        }



        public static double[,] CrossTabValues<TKey>(this Dictionary<TKey, List<Visit>> visits, bool nonZero = true)
        {
            var n1 = 0;
            var groups = new Dictionary<TKey, int>();
            foreach (var g in visits)
            {
                groups[g.Key] = n1++;
            }


            var nonZeros = visits.SelectMany(v => v.Value).Where(v => v.Value > 0);
            var mean = nonZeros.Count() == 0 ? 0 : nonZeros.Average(v => v.Value);


            return visits.SelectMany(kv => kv.Value.Select(v => Tuple.Create(groups[kv.Key], v))).CrossTab(
                v => v.Item1, v => v.Item2.Value == 0 ? 0 : v.Item2.Value < mean ? 1 : 2, n1, 3);
        }

        public static double Chisq(this IEnumerable<Visit> visits, ExperienceFactor factor, bool nonZero = true)
        {
            double[,] tab;
            if (nonZero)
            {
                var nonZeros = visits.Where(v => v.Value > 0);
                if (!nonZeros.Any())
                {
                    return 1;
                }
                var mean = nonZeros.Average(v => v.Value);

                tab = visits.CrossTab(v => v.ObservedLevels[factor], v => v.Value == 0 ? 0 : v.Value < mean ? 1 : 2,
                    factor.Levels.Length, 3);

                foreach (var count in tab)
                {
                    if (count < 5)
                    {
                        return visits.Chisq(factor, false);
                    }
                }
            }
            else
            {
                var mean = visits.Average(v => v.Value);
                tab = visits.CrossTab(v => v.ObservedLevels[factor], v => v.Value < mean ? 0 : 1, factor.Levels.Length,
                    2);
                //foreach (var count in tab)
                //{
                //    if (count < 5)
                //    {
                //        return visits.Chisq(factor, false);
                //    }
                //}
            }

            return tab.Chisq();
        }

        public static void Debug(this double[,] counts)
        {
            var r = new StringBuilder();
            r.Append("matrix(c(");
            for (var i = 0; i < counts.GetLength(0); i++)
            {
                for (var j = 0; j < counts.GetLength(1); j++)
                {
                    Console.Out.Write("{0,8:N0}", counts[i, j]);
                    if (i < counts.GetLength(0) - 1 && j < counts.GetLength(1) - 1)
                    {
                        if (i > 0 || j > 0) r.Append(",");
                        r.Append(counts[i, j]);
                    }
                }
                Console.Out.WriteLine();
            }
            r.Append("), byrow=T, nrow=").Append(counts.GetLength(0) - 1).Append(")");
            Console.Out.WriteLine(r.ToString());
        }
    }
}
