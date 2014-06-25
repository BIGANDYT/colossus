using System;
using System.Collections.Generic;
using System.Linq;

namespace Colossus
{    
    public class VisitSimulator
    {
        private readonly Random _random;
        private readonly SampleSet<VisitGroup> _groupSet;
        private readonly IExperienceDistributor _distributor;

        

        public VisitSimulator(Dictionary<VisitGroup, double> groupWeights, 
            IExperienceDistributor distributor, 
            Random random = null)
        {            
            _random = random ?? Randomness.Random;
            _groupSet = new SampleSet<VisitGroup>(groupWeights);

            _distributor = distributor;            
        }

        public IEnumerable<Visit> Next(int count)
        {
            return Enumerable.Range(0, count).Select(i => Next());
        }

        public Visit Next()
        {
            var group = _groupSet.Sample(_random);


            var ctx = _distributor.GetNext(group.SpawnVisit());
            ctx.VisitContext.Visit.UpdateState(ctx.Experience);
            
            ctx.VisitContext.Commit();

            return ctx.VisitContext.Visit;
        }


        public Visit Resample(Visit visit, bool keepTags = true)
        {
            return keepTags ? visit.Clone() : visit.Group.SpawnVisit();            
        }

                

    }
}
