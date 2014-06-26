using System.Collections.Generic;
using System.Linq;
using Colossus.RandomVariables;

namespace Colossus
{
    public class ExperienceOverride
    {        
        public List<ExperienceFactorMatcher> Factors { get; set; }

        public List<IRandomVariable> Variables { get; set; }

        public Dictionary<Goal, double> GoalBoosts{ get; set; }

        public ExperienceOverride()
        {
            Factors = new List<ExperienceFactorMatcher>();
            Variables = new List<IRandomVariable>();
            GoalBoosts = new Dictionary<Goal, double>();
        }

        public bool Matches(Dictionary<ExperienceFactor, int> levels)
        {
            return Factors.All(f => f.Match(levels));
        }

        public class Builder
        {
            private readonly VisitGroup _owner;

            private readonly ExperienceOverride _override = new ExperienceOverride();
            public Builder(VisitGroup owner)
            {
                _owner = owner;
            }

            public Builder And(ExperienceFactor factor, int level)
            {
                _override.Factors.Add(new DirectFactorMatcher(factor, level));
                return this;
            }

            public Builder And(string name, int level)
            {
                _override.Factors.Add(new FactorByNameMatcher(name, level));
                return this;
            }

            public Builder And(string factorName, string levelName)
            {
                _override.Factors.Add(new FactorLevelByNameMatcher(factorName, levelName));
                return this;
            }

            public VisitGroup Then(params IRandomVariable[] variables)
            {
                _override.Variables = variables.ToList();
                _owner.ExperienceOverrides.Add(_override);
                return _owner;
            }

            public Builder Boost(Goal goal, double boost)
            {
                _override.GoalBoosts[goal] = boost;
                return this;
            }

            public VisitGroup End(params IRandomVariable[] variables)
            {
                return Then();
            }
        } 
    }


    public abstract class ExperienceFactorMatcher
    {
        public abstract bool Match(IDictionary<ExperienceFactor, int> levels);
    }

    public class DirectFactorMatcher : ExperienceFactorMatcher
    {
        public ExperienceFactor Factor { get; set; }
        public int Level { get; set; }


        public DirectFactorMatcher(ExperienceFactor factor, int level)
        {
            Factor = factor;
            Level = level;
        }


        public override bool Match(IDictionary<ExperienceFactor, int> levels)
        {
            int level;
            return levels.TryGetValue(Factor, out level) && level == Level;
        }
    }
    public class FactorByNameMatcher : ExperienceFactorMatcher
    {
        public string Name { get; set; }
        public int Level { get; set; }

        public FactorByNameMatcher(string name, int level)
        {
            Name = name;
            Level = level;
        }

        public override bool Match(IDictionary<ExperienceFactor, int> levels)
        {
            return levels.Any(kv => kv.Key.Name.Equals(Name) && kv.Value == Level);
        }
    }

    public class FactorLevelByNameMatcher : ExperienceFactorMatcher
    {
        public string LevelName { get; set; }
        public string FactorName { get; set; }

        public FactorLevelByNameMatcher(string factorName, string levelName)
        {
            LevelName = levelName;
            FactorName = factorName;
        }

        public override bool Match(IDictionary<ExperienceFactor, int> levels)
        {            
            return levels.Any(kv => kv.Key.Levels[kv.Value].Equals(LevelName) && (FactorName == null || kv.Key.Name == FactorName));
        }
    }
}
