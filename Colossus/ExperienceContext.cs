using System;
using System.Collections.Generic;

namespace Colossus
{
    public class VisitContext
    {       
        public Visit Visit { get; set; }

        public Dictionary<string, object> RequestData { get; set; }

        public DateTime? LastVisit { get; set; }

        public VisitContext(Visit visit)
        {
            Visit = visit;
        }

        public virtual void Commit()
        {
            //This can be made more "realistic"
            //Task: Given a set of goals, find a realistic visit path that converts those.
            //Maybe, include duration. And shortest path.


            foreach (var goal in Visit.Goals)
            {
                goal.Convert(this);
            }
        }
    }
}
