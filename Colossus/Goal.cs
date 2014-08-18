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

        public virtual GoalState GetState(Visit visit)
        {
            return GoalState.Available;
        }

        public virtual void Convert(VisitContext visitContext)
        {
            
        }
    }
}
