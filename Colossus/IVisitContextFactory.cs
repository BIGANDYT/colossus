using System.Collections.Generic;

namespace Colossus
{
    public interface IVisitContextFactory
    {
        IEnumerable<Test> Tests { get; }

        /// <summary>
        /// Call this method with the personalization rules that are relevant to the visitor to get the variations to show/include.        
        /// For example, if a personalization rule shows a sausage to German visitors then the rule should be included here only for Germans.                
        /// </summary>                
        VisitContext CreateContext(Visit visit);
    }
}
