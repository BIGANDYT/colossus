using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Colossus
{
    public interface IPageGenerator
    {
        IEnumerable<VisitPage> Generate(Visit visit);
    }

    public class SimplePageGenerator : IPageGenerator
    {
        public SampleSet<string> Urls { get; set; }
        public IRandomDistribution PageCount { get; set; }
        public IRandomDistribution Duration { get; set; }

        public SimplePageGenerator(SampleSet<string> urls, IRandomDistribution pageCount, IRandomDistribution duration)
        {
            Urls = urls;
            PageCount = pageCount;
            Duration = duration;
        }

        public IEnumerable<VisitPage> Generate(Visit visit)
        {
            var n = PageCount.Next();

            for (var i = 0; i < n; i++)
            {
                yield return new VisitPage
                {
                    Url = Urls.Sample(),
                    Duration = TimeSpan.FromSeconds(Duration.Next())
                };
            }
        }
    }
}
