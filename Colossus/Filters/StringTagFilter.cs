namespace Colossus.Filters
{
    public class StringTagFilter : TagFilter
    {
        public string Value { get; set; }

        public StringTagFilter(string key, string value)
            : base(key)
        {
            Value = value;
        }

        protected override bool Evaluate(object value)
        {
            return Value.Equals(value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as StringTagFilter;
            return other != null && other.Key.Equals(Key) && other.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            var h = Key.GetHashCode();
            h = h << 5 + h ^ Value.GetHashCode();
            return h;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Key, Value);
        }
    }
}