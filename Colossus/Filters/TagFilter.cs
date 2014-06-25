namespace Colossus.Filters
{
    public abstract class TagFilter : IVisitFilter
    {
        public string Key { get; set; }

        protected abstract bool Evaluate(object value);

        protected TagFilter(string key)
        {
            Key = key;
        }

        public bool Include(Visit visit)
        {
            object value;
            return visit.Tags.TryGetValue(Key, out value) && Evaluate(value);            
        }

        public override string ToString()
        {
            return string.Format("Tag filter for {0}", Key);
        }
    
    }
}