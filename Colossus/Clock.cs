using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus
{
    //TODO: Improve with randomly distributed durations, variations based on visit types etc.
    public class Clock
    {
        public VisitContext Context { get; set; }

        public Clock(VisitContext context)
        {
            Context = context;
        }

        public void Update(TimeSpan duration)
        {
            var start = Context.LastVisit ?? Context.Visit.StartDate;
            var end = start + duration;

            Context.RequestData["StartDate"] = start;
            Context.RequestData["EndDate"] = end;

            Context.LastVisit = end;
        }
    }
}
