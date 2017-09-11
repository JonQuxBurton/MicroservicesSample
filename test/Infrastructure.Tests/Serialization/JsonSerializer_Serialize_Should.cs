using Infrastructure.Serialization;
using Xunit;

namespace Infrastructure.Tests.Serialization
{
    public class JsonSerializer_Serialize_Should
    {
        [Fact]
        public void SerializeObject()
        {
            var sampleObject = new {
                Duck = "Mallard"
            };

            var sut = new JsonSerializer();

            var actual = sut.Serialize(sampleObject);

            Assert.Equal("{\"Duck\":\"Mallard\"}", actual);
        }
    }
}
