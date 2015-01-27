using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colossus.Integration.NervaDemo;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.ExperienceInsights.Aggregation.Data.Model.Dimensions;

namespace Colossus.Integration.Dimensions
{
    public abstract class MockDataDimension :DimensionBase
    {
        public MockDataDimension(Guid dimensionId) : base(dimensionId)
        {
        }

        public override IEnumerable<DimensionData> GetData(IVisitAggregationContext context)
        {
            object json;
            if (context.Visit.CustomValues.TryGetValue("NervaDemo", out json))
            {
                var jsonString = json as string;
                if (!string.IsNullOrEmpty(jsonString))
                {
                    return GetData(context, JsonConvert.DeserializeObject<NervaDemoMockData>(jsonString));
                }
            }

            return new DimensionData[0];
        }

        protected abstract IEnumerable<DimensionData> GetData(IVisitAggregationContext context, NervaDemoMockData data);
    }
}
