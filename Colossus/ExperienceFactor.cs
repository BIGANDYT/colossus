namespace Colossus
{
    public class ExperienceFactor
    {
        public int Index { get; set; }

        /// <summary>
        /// This is also the "not personalized" variation for personalization rules
        /// </summary>
        public int Original { get; set; }

        public string Name { get; set; }

        public string[] Levels { get; set; }

        public virtual bool IsRelevant(Visit visit)
        {
            return true;
        }
    }
}