using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics.Model;
using Sitecore.ExperienceInsights.Aggregation.Data.Model.Dimensions;

namespace Colossus.Integration.Dimensions
{
    public class ByGoalCategory : PageEventDimensionBase
    {
        public ByGoalCategory(Guid dimensionId) : base(dimensionId)
        {
        }

        protected override bool Filter(PageEventData pageEventData)
        {
            return pageEventData.IsGoal;
        }

        protected override string ExtractDimensionKey(PageEventData pageEvent)
        {
            return "(None)";
        }
    }
}
