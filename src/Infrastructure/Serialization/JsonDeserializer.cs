using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Serialization
{
    public class JsonDeserializer : IDeserializer
    {
        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public T DeserializeBytes<T>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
        }
    }
}
