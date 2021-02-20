using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServiceBusTopic
{
    public class Payload
    {
        [JsonProperty("ids")] private readonly IList<int> _ids;
        [JsonIgnore] private readonly Type _type;
        [JsonIgnore] public string ToJson => JsonConvert.SerializeObject(this);
        public string Type => _type.ToString().ToLower();
        
        public Payload(IList<int> ids, Type type)
        {
            _ids = ids;
            _type = type;
        }

    }
}