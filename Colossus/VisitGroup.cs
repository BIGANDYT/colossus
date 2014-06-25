﻿using System;
using System.Collections.Generic;
using System.Linq;
using Colossus.RandomVariables;

namespace Colossus
{
    public class VisitGroup
    {
        public VisitGroup BaseGroup { get; set; }
        public string Name { get; set; }        

        public List<IRandomVariable> Variables{ get; set; }

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
            
            var overrides = this.All().SelectMany(g => g.ExperienceOverrides.Where(e => e.Matches(levels))
                .OrderByDescending(f => f.Factors.Count)).ToArray();

            foreach (var o in overrides)
            {                
                ctx.GoalBoosts.Merge(o.GoalBoosts);
            }
           
            ctx.Variables = overrides.SelectMany(o => o.Variables).ToList();

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