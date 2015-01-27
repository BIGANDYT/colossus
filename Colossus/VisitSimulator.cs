using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{    
    public class VisitSimulator
    {        
        private readonly Func<VisitGroup> _groups;
        public IVisitContextFactory ContextFactory { get; set; }
        
        public VisitSimulator(VisitGroup group, IVisitContextFactory contextFactory)
            : this(()=>group, contextFactory)
        {
            
        }

        public VisitSimulator(Func<VisitGroup> groups, 
            IVisitContextFactory contextFactory)
        {                        
            _groups = groups;
            ContextFactory = contextFactory;            
        }

        public IEnumerable<Visit> Next(int count)
        {
            return Enumerable.Range(0, count).Select(i => Next());
        }

        public Visit Next()
        {
            var group = _groups();
            var ctx = ContextFactory.CreateContext(group.SpawnVisit());

            ctx.Commit();

            return ctx.Visit;
        }


        public Visit Resample(Visit visit, bool keepTags = true)
        {
            return keepTags ? visit.Clone() : visit.Group.SpawnVisit();            
        }

                

    }
}
