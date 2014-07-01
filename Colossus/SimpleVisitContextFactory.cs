using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus
{
    public class SimpleVisitContextFactory : IVisitContextFactory
    {
        public IEnumerable<Test> Tests { get; private set; }
        public VisitContext CreateContext(Visit visit)
        {
            return new VisitContext(visit);
        }
    }
}
