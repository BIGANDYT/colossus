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
        public void Update(VisitContext ctx, TimeSpan duration)
        {
            var start = ctx.LastVisit ?? ctx.Visit.StartDate;
            var end = start + duration;

            ctx.RequestData["StartDate"] = start;
            ctx.RequestData["EndDate"] = end;

            ctx.LastVisit = end;
        }
    }
}
