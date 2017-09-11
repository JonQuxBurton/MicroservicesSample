using Infrastructure.Serialization;
using Newtonsoft.Json;
using System.Text;
using Xunit;

namespace Infrastructure.Tests.Serialization
{
    public class JsonSerializer_Deserialize_Should
    {
        [Fact]
        public void DeserializeObject()
        {
            var expectedObject = new Dummy
            {
                Duck = "Mallard"
            };

            var expectedBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedObject));

            var sut = new JsonDeserializer();

            var actual = sut.DeserializeBytes<Dummy>(expectedBytes);

            Assert.Equal(expectedObject.Duck, actual.Duck);
        }
    }

    public class Dummy
    {
        public string Duck { get; set; }
    }
}
