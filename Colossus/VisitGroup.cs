using System;
using System.Collections.Generic;
using System.Linq;
using Colossus.RandomVariables;

namespace Colossus
{
    public class VisitGroup
    {
        private IPageGenerator _pageGenerator;
        public VisitGroup BaseGroup { get; set; }
        public string Name { get; set; }


        public IPageGenerator PageGenerator
        {
            get { return _pageGenerator ?? BaseGroup.PageGenerator; }
            set { _pageGenerator = value; }
        }

        public List<IRandomVariable> Variables { get; set; }

        public List<ExperienceOverride> ExperienceOverrides { get; set; }

        public Visit SpawnVisit()
        {
            var v = new Visit()
            {
                Group = this
            };


            v.UpdateState();
            
            return v;
        }


        //private Dictionary<Experience, List<IRandomVariable>> _cache = new Dictionary<Experience, List<IRandomVariable>>();

        public SampleContext GetOverrides(Dictionary<ExperienceFactor, int> levels)
        {
            var ctx = new SampleContext();

            ctx.Variables = new List<IRandomVariable>();

            foreach (var o in this.All().SelectMany(g => g.ExperienceOverrides.Where(e => e.Matches(levels))
                .OrderByDescending(f => f.Factors.Count)))
            {
                ctx.Variables.AddRange(o.Variables);
                ctx.GoalBoosts.Merge(o.GoalBoosts);
            }
            

            return ctx;
        }

        //public VisitGroup ClearCache()
        //{
        //    _cache.Clear();
        //    return this;
        //}

        
        public VisitGroup(params IRandomVariable[] variables)
        {
            Variables = variables.ToList();
            ExperienceOverrides = new List<ExperienceOverride>();
        }
    }
}
