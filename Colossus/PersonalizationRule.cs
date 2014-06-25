using Colossus.Filters;

namespace Colossus
{
    public class PersonalizationRule : ExperienceFactor
    {
        public IVisitFilter Filter { get; set; }        

        public override bool IsRelevant(Visit visit)
        {
            return Filter.Include(visit);
        }
    }
}