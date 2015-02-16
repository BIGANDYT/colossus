using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus
{
    //public interface IPageSequenceGenerator
    //{
    //    IEnumerable<VisitPage> Generate(Visit visit);
    //}

    //public class SimplePageSequenceGenerator : IPageSequenceGenerator
    //{
    //    public SampleSet<string> Urls { get; set; }
    //    public IRandomDistribution PageCount { get; set; }
    //    public IRandomDistribution Duration { get; set; }

    //    public SimplePageSequenceGenerator(SampleSet<string> urls, IRandomDistribution pageCount, IRandomDistribution duration)
    //    {
    //        Urls = urls;
    //        PageCount = pageCount;
    //        Duration = duration;
    //    }

    //    public IEnumerable<VisitPage> Generate(Visit visit)
    //    {
    //        var n = PageCount.Next();

    //        for (var i = 0; i < n; i++)
    //        {
    //            yield return new VisitPage
    //            {
    //                Url = Urls.Sample(),
    //                Duration = TimeSpan.FromSeconds(Duration.Next())
    //            };
    //        }
    //    }

    //    public static IPageSequenceGenerator Fixed(string url, double duration = 1d)
    //    {
    //        return new SimplePageSequenceGenerator(Sets.Single(url), RandomLinear.Fixed(1), RandomLinear.Fixed(duration));
    //    }
    //}
}
