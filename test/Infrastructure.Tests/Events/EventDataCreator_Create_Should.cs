using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Serialization;
using Moq;
using System.Text;
using Xunit;

namespace Infrastructure.Tests.Events
{
    public class EventDataCreator_Create_Should
    {
        [Fact]
        public void CreateEventData()
        {
            var expectedEventName = "PhoneLineOrderPlaced";
            var expectedGuid = System.Guid.NewGuid();

            var content = "content";

            var jsonSerializerMock = new Mock<ISerializer>();
            jsonSerializerMock
                .Setup(x => x.Serialize(content))
                .Returns("serialzedContent");
            jsonSerializerMock
                .Setup(x => x.Serialize(It.IsAny<EventMetaData>()))
                .Returns("serialzedMetaData");

            var guidCreatorMock = new Mock<IGuidCreator>();
            guidCreatorMock.Setup(x => x.Create())
                .Returns(expectedGuid);

            var sut = new EventDataCreator(jsonSerializerMock.Object, guidCreatorMock.Object);

            var actual = sut.Create(expectedEventName, content);

            Assert.Equal(expectedEventName, actual.Type);
            Assert.Equal(expectedGuid, actual.EventId);
            Assert.Equal(Encoding.UTF8.GetBytes("serialzedContent"), actual.Data);
            Assert.Equal(Encoding.UTF8.GetBytes("serialzedMetaData"), actual.Metadata);
        }
    }
}
