using System;
using System.Collections.Generic;
using System.Linq;
using Colossus.RandomVariables;

namespace Colossus
{
    public class Visit
    {
        public Dictionary<Test, Experience> Experiences { get; private set; }

        public Dictionary<ExperienceFactor, int> ObservedLevels { get; private set; }

        public DateTime Start { get; set; }

        public VisitGroup Group { get; set; }

        public Dictionary<string, object> Tags { get; set; }

        public HashSet<Goal> Goals { get; set; }

        public int Value { get; set; }


        private Dictionary<object, IRandomValue> _variables = new Dictionary<object, IRandomValue>();
        private Dictionary<object, IRandomValue> _testVariables = new Dictionary<object, IRandomValue>();
        

        public Dictionary<ExperienceFactor, int> Adjust(Dictionary<ExperienceFactor, int> factors)
        {
            return factors.ToDictionary(f => f.Key, f => f.Key.IsRelevant(this) ? f.Value : f.Key.Original);
        }

        public Visit()
        {
            Experiences = new Dictionary<Test, Experience>();
            ObservedLevels = new Dictionary<ExperienceFactor, int>();
            Goals = new HashSet<Goal>();
            Tags = new Dictionary<string, object>();
            Start = DateTime.Now;
        }

        public virtual void UpdateState(SampleContext context = null)
        {
            foreach (var var in Group.All().SelectMany(g => g.Variables)
                .Sample(_variables, context).Values)
            {
                var.Update(this);
            }
        }

        /// <summary>
        /// Update state according to experience and the group's ExperienceOverrides
        /// </summary>
        /// <param name="experience"></param>
        public virtual void UpdateState(Experience experience)
        {
            //The test has already been recorded
            if (Experiences.ContainsKey(experience.Test)) return;

            Experiences.Add(experience.Test, experience);

            
            ObservedLevels.Merge(Adjust(experience.Levels));

            //Update test variables given group's overrides for the variables in the experience
            var ctx = Group.GetOverrides(ObservedLevels);
            ctx.Sample(_testVariables);

            if (ctx.GoalBoosts.Count > 0)
            {
                //Conversion rates are changed. Recalculate conversions when state is updated
                _variables.Keys.OfType<Goal>().ToArray().ForEach(g=>_variables.Remove(g));
            }

            _variables.Merge(_testVariables, overwrite: true);            

            UpdateState(ctx);
        }

        public Visit Clone()
        {
            var clone = MemberwiseClone() as Visit;
            clone.Goals = Goals.ToSet();
            clone._variables = _variables.Clone();
            clone._testVariables = _testVariables.Clone();

            return clone;
        }
    }
}
