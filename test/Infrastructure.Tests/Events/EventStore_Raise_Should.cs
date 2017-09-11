using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Serialization;
using Moq;
using System;
using Xunit;

namespace Infrastructure.Tests.Events
{
    public class EventStore_Raise_Should
    {
        [Fact]
        public void RaiseEvent()
        {
            var expectedStreamName = "expected-stream";
            var expectedEventData = new EventData(System.Guid.NewGuid(), "ExpectedEvent", true, new byte[0], new byte[0]);

            var eventStoreConnectMock = new Mock<IEventStoreConnection>();

            var deserializerMock = new Mock<IDeserializer>();

            var sut = new Infrastructure.Events.EventStore(eventStoreConnectMock.Object, new Mock<IEventStreamReader>().Object, deserializerMock.Object);

            sut.Raise(expectedStreamName, expectedEventData);

            eventStoreConnectMock
                .Verify(x =>
                    x.AppendToStreamAsync(expectedStreamName, ExpectedVersion.Any,
                        It.Is<EventData[]>(y => y[0].IsJson == true &&
                                            y[0].EventId == expectedEventData.EventId &&
                                            y[0].Type == expectedEventData.Type)),
                                            Times.Once);
        }
    }
}
