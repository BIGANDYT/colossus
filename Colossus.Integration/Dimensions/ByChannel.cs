using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.Integration.NervaDemo;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.ExperienceInsights.Aggregation.Data.Model.Dimensions;

namespace Colossus.Integration.Dimensions
{
    public class ByChannel : MockDataDimension
    {
        public ByChannel(Guid dimensionId)
            : base(dimensionId)
        {
        }

        protected override IEnumerable<DimensionData> GetData(IVisitAggregationContext context, NervaDemoMockData data)
        {
            if (data.ChannelId.HasValue)
            {
                yield return new DimensionData
                {
                    DimensionKey = "" + data.ChannelId,
                    MetricsValue = CalculateCommonMetrics(context)                    
                };

                yield return new DimensionData
                {
                    DimensionKey = "" + data.ChannelId,
                    MetricsValue = CalculateCommonMetrics(context),
                    Tag = "" + data.ChannelTypeId,
                };
            }
        }
    }
}
