namespace Colossus.Filters
{
    public class CatchAllFilter : IVisitFilter
    {        
        public bool Include(Visit visit)
        {
            return true;
        }

        public override string ToString()
        {
            return "(Others)";
        }

        
        public override bool Equals(object obj)
        {
            return obj is CatchAllFilter;
        }

        public override int GetHashCode()
        {
            return 17;
        }
    }
}