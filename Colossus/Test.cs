using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class Test
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<Experience> Experiences { get; set; }

        public Experience GetExperience(Dictionary<ExperienceFactor, int> factors, bool addIfMissing = false)
        {
            foreach (var exp in Experiences)
            {
                if (exp.Levels.Contains(factors, true))
                {
                    return exp;
                }
            }

            if (addIfMissing)
            {
                Experiences.Add(new Experience
                {
                    Number = Experiences.Count,
                    Levels = factors.Clone()                    
                });

                return Experiences[Experiences.Count - 1];
            }

            throw new KeyNotFoundException("No such experience");
        }

        public static Test FromFactors(params ExperienceFactor[] factors)
        {
            var test = new Test();
            test.Experiences = new RoundRobinCounter(factors.Select(f => f.Levels.Length).ToArray(), false).All
                .Select((combination, i) =>
                    new Experience
                    {
                        Test = test,
                        Number = i + 1,
                        Levels = combination.ToDictionary((level, j) => factors[j], (level, j) => level)
                    }).ToList();

            return test;
        }
    }
}