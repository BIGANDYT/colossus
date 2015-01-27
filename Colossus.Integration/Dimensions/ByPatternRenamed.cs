using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics.Model;
using Sitecore.ExperienceInsights.Aggregation.Data.Model.Dimensions;

namespace Colossus.Integration.Dimensions
{
    public class ByPatternRenamed : ByPattern
    {
        public ByPatternRenamed(Guid dimensionId) : base(dimensionId)
        {
        }
        protected override string GetDimensionKey(ProfileData profile)
        {
            return string.Format("{1} ({0})", profile.ProfileName, profile.PatternLabel);
            //return string.Format("{0}{1}{2}", (object)profile.ProfileName, (object)Constants.HierarchySeparator, (object)profile.PatternLabel);
        }

    }
}
