namespace Colossus
{
    public class ExperienceContext
    {
        public Experience Experience { get; set; }        

        public VisitContext VisitContext { get; set; }
    }


    public class VisitContext
    {
        public Visit Visit { get; set; }
        public virtual void Commit()
        {
            
        }

    }
}
