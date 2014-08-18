using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus.RandomVariables
{
    public static class RandomVariableHelpers
    {
        
        public static GoalVariable WhenTrue(this GoalVariable gp, params IRandomVariable[] ps)
        {
            gp.TrueCorrelations = ps.ToList();
            return gp;
        }

        public static GoalVariable WhenFalse(this GoalVariable gp, params IRandomVariable[] ps)
        {
            gp.FalseCorrelations = ps.ToList();
            return gp;
        }


        
        /// <summary>
        /// Creates a correlation between the goals such that the marginal probability for the second goal is fixed.
        /// -1 corresponds to g1:false => g2:false and 1 g1:true => g2:true. A value between -1 and 1 is something in between (i.e. structure + randomness)
        /// </summary>
        /// <param name="gp"></param>
        /// <param name="goal"></param>
        /// <param name="marginal"></param>
        /// <param name="correlation"></param>
        /// <returns></returns>
        public static GoalVariable Correlate(this GoalVariable gp, Goal goal, double marginal, double correlation, Action<GoalVariable, bool> chain = null)
        {
            double ptrue, pfalse;
            if (gp.Probability >= 1)
            {
                ptrue = marginal;
                pfalse = 0;
            }
            else if (gp.Probability <= 0 || marginal <= 0)
            {
                ptrue = pfalse = 0;
            }
            else if (marginal >= 1)
            {
                ptrue = pfalse = 1;
            }
            else
            {
                var pa = gp.Probability;
                var qa = 1 - pa;
                var pb = marginal;
                var qb = 1 - marginal;

                var sqrt = Math.Sqrt(pa*qa*pb*qb);
                var r_max = (Math.Min(pa, pb) - pa*pb)/sqrt; //Maximum correlation given marginal probabilities
                var r_min = -(Math.Min(qa, pb) - qa*pb)/sqrt; //Minimum correlation given marginal probabilities

                correlation = r_min + (correlation + 1)/2d*r_max; //Rescale correlation to min/max

                ptrue = correlation*sqrt + pa*pb; // P(A ^ B) = r * sqrt(p_a*q_a*p_b*q_b) + p_a*p_b
                pfalse = -correlation*sqrt + qa*pb;
                    // Same as above, except it's in the other diagonal so it's -r * (etc.)
                
                if (new[] {ptrue, pa - ptrue, pfalse, qa - pfalse}.Any(x => x < 0 || x > 1))
                {
                    Console.Error.WriteLine("Invalid correlation/marginal probabilitiy specified");
                }

                ptrue /= pa; //P(B | A) = P(A v B)/P(A)
                pfalse /= qa; //P(B | ^A) = P(^A v B)/P(^A)          
            }

            var t = new GoalVariable(goal, ptrue);
            gp.TrueCorrelations.Add(t);

            var f = new GoalVariable(goal, pfalse);
            gp.FalseCorrelations.Add(f);

            if (chain != null)
            {
                chain(t, true);
                chain(f, false);
            }

            return gp;
        }     

        public static IEnumerable<VisitGroup> All(this VisitGroup group)
        {
            yield return group;

            if (group.BaseGroup != null)
            {
                foreach (var baseGroup in group.BaseGroup.All())
                {
                    yield return baseGroup;
                }
            }
        }

        public static Dictionary<object, IRandomValue> Sample(this SampleContext context,
            Dictionary<object, IRandomValue> values = null)
        {            
            return context.Variables.Sample(values, context);
        }
        
        public static Dictionary<object, IRandomValue> Sample(this IEnumerable<IRandomVariable> variables, 
            Dictionary<object, IRandomValue> values = null, SampleContext context = null)
        {
            values = values ?? new Dictionary<object, IRandomValue>();
            
            foreach (var var in variables)
            {
                IRandomValue value;
                if (!values.TryGetValue(var.Key, out value))
                {
                    if ((value = var.Sample(context)) != null)
                    {
                        values.Add(var.Key, value);
                    }
                }

                if (value != null)
                {
                    Sample(value.Correlations, values, context);                    
                }
            }
            return values;
        }


        public static DiscreteSampleSet<IEnumerable<TValue>>.SetBuilder Weight<TValue>(
            this DiscreteSampleSet<IEnumerable<TValue>>.SetBuilder builder, TValue value, double weight)
        {
            return builder.Weight(new[] {value}, weight);
        }
    }
}