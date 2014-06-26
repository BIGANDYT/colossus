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
        public DateTime Start { get; set; }

        public TimeSpan Duration { get; set; }

        public Clock()
        {
            Start = DateTime.Now;
            Duration = TimeSpan.FromMilliseconds(1);
        }

        public void Update(VisitContext ctx)
        {
            var start = Start;
            var end = Start + Duration;

            ctx.RequestData["StartDate"] = start;
            ctx.RequestData["EndDate"] = end;

            Start = end;
        }
    }
}
