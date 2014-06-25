namespace Colossus.Filters
{
    public class NumericTagFilter : TagFilter
    {
        public double? Lower { get; set; }
        public double? Upper { get; set; }
        public bool LessThan { get; set; }

        public NumericTagFilter(string key, double? lower, double? upper)
            : base(key)
        {
            Lower = lower;
            Upper = upper;            
        }


        protected override bool Evaluate(object value)
        {
            if (value is double)
            {
                var v = (double) value;
                if (Lower.HasValue && Lower > v) return false;
                if (Upper.HasValue && Lower != Upper && v >= Upper) return false;

                return true;
            }
            
            return false;
        }

        public override bool Equals(object obj)
        {
            var other = obj as NumericTagFilter;
            return other != null && other.Key.Equals(Key) && other.Lower.Equals(Lower) && other.Upper.Equals(Upper);
        }

        public override int GetHashCode()
        {
            var h = Key.GetHashCode();
            h = h << 5 + h ^ Lower.GetHashCode();
            h = h << 5 + h ^ Upper.GetHashCode();
            return h;
        }

        public override string ToString()
        {            
            if (Lower == Upper)
            {
                return string.Format("{0} = {1}", Key, Lower);
            }

            if (Lower.HasValue && Upper.HasValue)
            {
                return string.Format("{0} in [{1};{2}[", Key, Lower, Upper);
            }
            
            if( Lower.HasValue )
            {
                return string.Format("{0} >= {1}", Key, Lower);
            }

            if (Upper.HasValue)
            {
                return string.Format("{0} < {1}", Key, Upper);
            }

            return string.Format("{0} exists", Key);
        }
    }
}