﻿using System.Collections.Generic;
using System.Linq;
using Colossus.RandomVariables;

namespace Colossus
{
    public class ExperienceOverride
    {
        public Dictionary<ExperienceFactor, int> Factors { get; set; }

        public List<IRandomVariable> Variables { get; set; }

        public Dictionary<Goal, double> GoalBoosts{ get; set; }

        public ExperienceOverride()
        {
            Factors = new Dictionary<ExperienceFactor, int>();
            Variables = new List<IRandomVariable>();
            GoalBoosts = new Dictionary<Goal, double>();
        }

        public bool Matches(Dictionary<ExperienceFactor, int> levels)
        {
            return levels.Contains(Factors);
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
                _override.Factors.Add(factor, level);
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

    
}
