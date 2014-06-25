using System.Linq;

namespace Colossus.Filters
{
    public class OrFilter : IVisitFilter
    {
        public IVisitFilter[] Filters { get; set; }

        public OrFilter(params IVisitFilter[] filters)
        {
            Filters = filters;
        }

        public bool Include(Visit visit)
        {
            return Filters.Any(f => f.Include(visit));
        }

        public override bool Equals(object obj)
        {
            var other = obj as OrFilter;
            return other != null && other.Filters.SequenceEqual(Filters);
        }

        public override int GetHashCode()
        {
            return Filters.Aggregate(1, (current, o) => current << 5 + current ^ o.GetHashCode());
        }

        public override string ToString()
        {
            return string.Join(" or ", Filters.Select(f => f.ToString()));
        }
    }
}