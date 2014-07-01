using System.Collections.Generic;
using System.Linq;

namespace Colossus
{
    public class Experience
    {       
        public int Number { get; set; }

        public Test Test { get; set; }               

        public Dictionary<ExperienceFactor, int> Levels { get; set; }

        public override string ToString()
        {
            return string.Format("{0, 2}. ({1})", 
                Number, 
                string.Join(", ", Levels.OrderBy(f => f.Key.Index).ThenBy(f => f.Key.Name).Select(f => f.Format(false))));            
        }
    }
}
