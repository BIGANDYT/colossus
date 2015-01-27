using System;
using System.Collections.Generic;
using System.Linq;
using Colossus.Pages;
using Colossus.RandomVariables;

namespace Colossus
{
    public class VisitGroup
    {
        //private IPageSequenceGenerator _pageSequenceGenerator;
        public VisitGroup BaseGroup { get; set; }
        public string Name { get; set; }


        public VisitAction Action { get; set; }

        //public IPageSequenceGenerator PageSequenceGenerator
        //{
        //    get { return _pageSequenceGenerator ?? (BaseGroup != null ? BaseGroup.PageSequenceGenerator : null); }
        //    set { _pageSequenceGenerator = value; }
        //}

        public List<IRandomVariable> Variables { get; set; }

        public List<ExperienceOverride> ExperienceOverrides { get; set; }

        public Visit SpawnVisit()
        {
            var v = new Visit()
            {
                Group = this
            };

            v.UpdateState();

            var group = this;
            while (group != null)
            {
                if (group.Action != null)
                {
                    v.Action = group.Action;
                    break;
                }
                group = group.BaseGroup;
            }
            //if (PageSequenceGenerator != null)
            //{
            //    v.Pages.AddRange(PageSequenceGenerator.Generate(v));
            //}

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
                //ctx.GoalBoosts.Merge(o.GoalBoosts);
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
