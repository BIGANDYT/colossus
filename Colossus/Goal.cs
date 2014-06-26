using System;
using System.Net;

namespace Colossus
{
    public class Goal
    {
        public string Name { get; set; }
        
        public int Value { get; set; }

        public GoalGroup GoalGroup { get; set; }
        
        public Goal(string name, int value)
        {
            Name = name;
            Value = value;

            GoalGroup = new GoalGroup {Id = Guid.NewGuid(), Name = name};
        }

        public virtual GoalState GetState(VisitContext visit)
        {
            return GoalState.Unavailable;
        }

        public virtual void Convert(VisitContext vist)
        {
            
        }
    }
}
