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
    public class BySubChannel : MockDataDimension
    {
        public BySubChannel(Guid dimensionId) : base(dimensionId)
        {
        }

        protected override IEnumerable<DimensionData> GetData(IVisitAggregationContext context, NervaDemoMockData data)
        {
            if (data.SubChannelId.HasValue)
            {
                yield return new DimensionData
                {
                    DimensionKey = "" + data.SubChannelId,
                    MetricsValue = CalculateCommonMetrics(context)
                };

                yield return new DimensionData
                {
                    DimensionKey = "" + data.SubChannelId,
                    MetricsValue = CalculateCommonMetrics(context),
                    Tag = "" + data.ChannelId,
                };
            }
        }
    }
}
