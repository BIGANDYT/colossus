using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.Analytics.Model;
using Sitecore.ExperienceInsights.Aggregation.Data.Model.Dimensions;
using Sitecore.Shell.Applications.ContentEditor;

namespace Colossus.Integration
{
    public class TestDimension : PageEventDimensionBase
    {
        public TestDimension(Guid dimensionId) : base(dimensionId)
        {
        }

        //public override IEnumerable<DimensionData> GetData(IVisitAggregationContext context)
        //{
        //    foreach (var ev in context.Visit.Pages.SelectMany(p => p.PageEvents))
        //    {
        //        yield return new DimensionData
        //        {
        //            DimensionKey = ""+ev.PageEventDefinitionId,
        //            MetricsValue = CalculateCommonMetrics(context)
        //        };
        //    }
            
                      
        //}


        //public override IEnumerable<DimensionData> GetData(IVisitAggregationContext context)
        //{
        //    var counts = GetDimensionKeys(context);
        //    foreach (var ev in context.Visit.Pages.SelectMany(p => p.PageEvents))
        //    {
        //        if (ev.IsGoal)
        //        {
        //            throw new Exception(counts.Count+"");
        //        }
        //    }
        //    //var keyCount = GetDimensionKeys(context);

        //    //if (keyCount.Count > 0)
        //    //{
        //    //    throw new Exception("Foo2");   
        //    //}

        //    return base.GetData(context);
        //}

        protected override bool Filter(PageEventData pageEventData)
        {
            return pageEventData.IsGoal;
        }

        protected override string ExtractDimensionKey(PageEventData pageEvent)
        {
            return "" + pageEvent.PageEventDefinitionId;
        }
    }
}
