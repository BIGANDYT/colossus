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
        public TimeSpan Duration { get; set; }

        public Clock()
        {
            Duration = TimeSpan.FromSeconds(1);
        }

        public void Update(VisitContext ctx)
        {
            var start = ctx.LastVisit ?? ctx.Visit.Start;
            var end = start + Duration;

            ctx.RequestData["StartDate"] = start;
            ctx.RequestData["EndDate"] = end;

            ctx.LastVisit = end;
        }
    }
}
