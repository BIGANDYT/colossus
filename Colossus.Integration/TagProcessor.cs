using Newtonsoft.Json.Linq;

namespace Colossus.Integration
{
    public interface ITagDataProcessor
    {
        void Process(JObject visitTags, JObject requestData);
    }

 
}
