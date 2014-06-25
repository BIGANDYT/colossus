using System.Linq;

namespace Colossus.Filters
{
    public class AndFilter : IVisitFilter
    {
        public IVisitFilter[] Filters { get; set; }

        public AndFilter(params IVisitFilter[] filters)
        {
            Filters = filters;
        }

        public bool Include(Visit visit)
        {
            return Filters.All(f => f.Include(visit));
        }

        public override bool Equals(object obj)
        {
            var other = obj as AndFilter;
            return other != null && other.Filters.SequenceEqual(Filters);
        }

        public override int GetHashCode()
        {
            return Filters.Aggregate(0, (current, o) => current << 5 + current ^ o.GetHashCode());
        }

        public override string ToString()
        {
            return string.Join(" and ", Filters.Select(f=>f.ToString()));
        }
    }
}