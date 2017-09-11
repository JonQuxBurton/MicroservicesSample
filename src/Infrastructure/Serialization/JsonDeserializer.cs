using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Serialization
{
    public class JsonDeserializer : IDeserializer
    {
        public T DeserializeBytes<T>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
        }
    }
}
