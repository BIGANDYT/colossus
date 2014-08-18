using System;
using System.Collections.Generic;

namespace Colossus
{
    public class VisitContext
    {
        public IVisitContextFactory Factory { get; private set; }

        public Visit Visit { get; private set; }

        public Dictionary<string, object> RequestData { get; set; }

        public DateTime? LastVisit { get; set; }

        public virtual void VisitPage(VisitPage page)
        {
            
        }

        public VisitContext(IVisitContextFactory factory, Visit visit)
        {
            Factory = factory;
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

            if (Visit.Pages != null)
            {
                foreach (var page in Visit.Pages)
                {
                    VisitPage(page);
                }
            }
        }
    }
}
