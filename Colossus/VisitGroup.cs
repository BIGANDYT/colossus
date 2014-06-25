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

        public IEnumerable<IRandomVariable> GetOverrides(Dictionary<ExperienceFactor, int> levels)
        {
            List<IRandomVariable> variables;
            return this.All().SelectMany(g => g.ExperienceOverrides.Where(e => e.Matches(levels))
                    .OrderByDescending(f => f.Factors.Count)).SelectMany(f=>f.Variables)
                    .ToList();                
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
