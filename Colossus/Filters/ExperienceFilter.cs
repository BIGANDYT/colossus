namespace Colossus.Filters
{
    public class ExperienceFilter : IVisitFilter
    {
        public ExperienceFactor Factor { get; set; }
        public int Level { get; set; }

        public ExperienceFilter(ExperienceFactor factor, int level)
        {
            Factor = factor;
            Level = level;
        }

        public bool Include(Visit visit)
        {
            int level;
            return visit.ObservedLevels.TryGetValue(Factor, out level) && level == Level;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExperienceFilter;
            return other != null && other.Factor.Equals(Factor) && other.Level == Level;
        }

        public override int GetHashCode()
        {
            var h = Factor.GetHashCode();
            h = h << 5 + h ^ Level.GetHashCode();
            return h;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Factor.Name, Factor.Levels[Level]);
        }
    }
}