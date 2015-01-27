using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Colossus
{
    public class ExperienceCrawler : IVisitContextFactory
    {        
        public Dictionary<string, Test> Tests { get; set; }        

        public ExperienceCrawler()
        {            
            Tests = new Dictionary<string, Test>();
        }        
        

        public VisitContext CreateContext(Visit visit)
        {
            return new WebVisitContext(this, visit);                        
        }


        IEnumerable<Test> IVisitContextFactory.Tests
        {
            get { return Tests.Values; }
        }
        
    }
}