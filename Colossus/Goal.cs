using System.Net;

namespace Colossus
{
    public class Goal
    {
        public string Name { get; set; }
        
        public int Value { get; set; }


        public virtual GoalState GetState(string currentUrl, string pageHtml)
        {
            return GoalState.Unavailable;
        }

        public virtual void Convert(string currentUrl, string pageHtml, WebClient wc)
        {
            
        }
    }
}
