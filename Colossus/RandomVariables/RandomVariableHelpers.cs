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


        public static WeightedTag<TValue> When<TValue>(this WeightedTag<TValue> var, TValue category, params IRandomVariable[] ps)
        {
            var.Correlations.Add(category, ps);
            return var;
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

            var pa = gp.Probability;
            var qa = 1 - pa;
            var pb = marginal;
            var qb = 1 - marginal;

            var sqrt = Math.Sqrt(pa * qa * pb * qb);
            var r_max = (Math.Min(pa, pb) - pa * pb) / sqrt; //Maximum correlation given marginal probabilities
            var r_min = -(Math.Min(qa, pb) - qa * pb) / sqrt; //Minimum correlation given marginal probabilities

            correlation = r_min + (correlation + 1) / 2d * r_max; //Rescale correlation to min/max

            var p_true = correlation * sqrt + pa * pb; // P(A ^ B) = r * sqrt(p_a*q_a*p_b*q_b) + p_a*p_b
            var p_false = -correlation * sqrt + qa * pb; // Same as above, except it's in the other diagonal so it's -r * (etc.)

            var t = new GoalVariable(goal, p_true / pa);//P(B | A) = P(A v B)/P(A)
            gp.TrueCorrelations.Add(t);             

            var f = new GoalVariable(goal, p_false / qa);//P(B | ^A) = P(^A v B)/P(^A)
            gp.FalseCorrelations.Add(f);


            if (chain != null)
            {
                chain(t, true);
                chain(f, false);
            }

            if (new[] { p_true, pa - p_true, p_false, qa - p_false }.Any(x => x < 0 || x > 1))
            {
                Console.Error.WriteLine("Invalid correlation/marginal probabilitiy specified");
            }            

            return gp;
        }

        
        //public static IEnumerable<GoalVariable> Boost(this IEnumerable<GoalVariable> probabilities,
        //    IDictionary<Goal, double> boosts, bool clone = true)
        //{
        //    if (clone)
        //    {

        //        probabilities = probabilities.Select(p => p.Clone()).ToList();
        //    }

        //    foreach (var p in probabilities.All())
        //    {
        //        double boost;
        //        if (boosts.TryGetValue(p.Key, out boost))
        //        {
        //            p.Probability *= boost;
        //        }
        //    }

        //    return probabilities;
        //}



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
                    values.Add(var.Key, value = var.Sample(context));
                }

                if (value != null)
                {
                    Sample(value.Correlations, values, context);                    
                }
            }
            return values;
        }        
    }
}