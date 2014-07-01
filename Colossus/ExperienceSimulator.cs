using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{    
    public class VisitSimulator
    {
        private readonly Random _random;
        private readonly SampleSet<VisitGroup> _groups;
        public IVisitContextFactory ContextFactory { get; set; }


        public VisitSimulator(VisitGroup group, IVisitContextFactory contextFactory)
            : this(new DistributedSampleSet<VisitGroup>(new []{group}), contextFactory)
        {
            
        }

        public VisitSimulator(SampleSet<VisitGroup> groups, 
            IVisitContextFactory contextFactory,
            Random random = null)
        {            
            _random = random ?? Randomness.Random;
            _groups = groups;

            ContextFactory = contextFactory;            
        }

        public IEnumerable<Visit> Next(int count)
        {
            return Enumerable.Range(0, count).Select(i => Next());
        }

        public Visit Next()
        {
            var group = _groups.Sample();
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
