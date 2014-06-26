using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class RoundRobinVisitContextFactory : IVisitContextFactory
    {
        public bool DistributeEvenlyForSubGroups { get; private set; }
        


        public Test Test { get; set; }
        private Dictionary<long, RoundRobinCounter> _counters;

        private PersonalizationRule[] _rules;
       
        public RoundRobinVisitContextFactory(Test test, bool distributeEvenlyForSubGroups = true)
        {
            DistributeEvenlyForSubGroups = distributeEvenlyForSubGroups;

            Test = test;
            _counters = new Dictionary<long, RoundRobinCounter>();
            _rules = Test.Experiences.SelectMany(e => e.Levels.Keys.OfType<PersonalizationRule>()).Distinct().ToArray();
        }


        public IEnumerable<Test> Tests
        {
            get { yield return Test; }
        }

        /// <summary>
        /// Call this method with the personalization rules that are relevant to the visitor to get the variations to show/include.        
        /// For example, if a personalization rule shows a sausage to German visitors then the rule should be included here only for Germans.                
        /// </summary>                
        public VisitContext CreateContext(Visit visit)
        {            
            var key =
                DistributeEvenlyForSubGroups
                    ? _rules.Select((r, i) => r.IsRelevant(visit) ? 2L << i : 0)
                        .Aggregate(0L, (current, value) => current |= value)
                    : 0L;


            RoundRobinCounter counter;
            counter = _counters.TryGetValue(key, out counter)
                ? counter
                : _counters[key] = new RoundRobinCounter(new[] {Test.Experiences.Count});
            
            
            visit.UpdateState(Test.Experiences[counter.Next[0]]);
            return new VisitContext(visit);            
        }
    }
}