namespace Colossus.Filters
{
    public class NotFilter : IVisitFilter
    {
        public IVisitFilter Filter { get; set; }

        public NotFilter(IVisitFilter filter)
        {
            Filter = filter;
        }

        public bool Include(Visit visit)
        {
            return !Filter.Include(visit);
        }

        public override bool Equals(object obj)
        {
            var other = obj as NotFilter;
            return other != null && other.Filter == Filter;
        }

        public override int GetHashCode()
        {
            return ~Filter.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Not {0}", Filter);
        }
    }
}