using System;
using System.Collections.Generic;
using System.Net;

namespace Colossus
{
    public class VisitContext
    {
        public IVisitContextFactory Factory { get; private set; }

        public Visit Visit { get; private set; }

        public Dictionary<string, object> RequestData { get; set; }

        public DateTime? LastVisit { get; set; }

        public string LastResponse { get; set; }

        public HashSet<Goal> ConvertedGoals { get; set; }

        public Clock Clock { get; set; }

        public virtual WebClient WebClient
        {
            get { return null; }
        }
        
        public VisitContext(IVisitContextFactory factory, Visit visit)
        {
            Factory = factory;
            Visit = visit;
            ConvertedGoals = new HashSet<Goal>();
            Clock = new Clock(this);
        }


        public virtual void Commit()
        {           
            if (Visit.Action != null)
            {
                Visit.Action.Execute(this);
            }

            ConvertGoals();
        }

        public void ConvertGoals()
        {
            foreach (var goal in Visit.Goals)
            {
                if (!ConvertedGoals.Contains(goal))
                {
                    goal.Convert(this);
                    ConvertedGoals.Add(goal);
                }
            }
        }
    }
}
