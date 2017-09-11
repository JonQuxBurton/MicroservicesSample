using Newtonsoft.Json;

namespace Infrastructure.Serialization
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
