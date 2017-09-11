using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Serialization;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Infrastructure.Tests.Events
{
    public class EventStore_GetEvents_Should
    {
        [Fact]
        public void ReturnEvents()
        {
            var expectedContent = "content";
            var expectedMetaData = new EventMetaData {
                EventName = "eventName",
                OccurredAt = new DateTimeOffset(new DateTime(2001, 5, 4))
            };

            var expectedEventsStreamEvent1 = new EventsStreamEvent()
            {
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedContent)),
                Metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(expectedMetaData)),
                OriginalEventNumber = 101
            };
            var expectedEvents = new List<EventsStreamEvent> { expectedEventsStreamEvent1 }
                .AsEnumerable();
            var dummyStreamName = "expected-stream";
            
            var eventStreamReader = new Mock<IEventStreamReader>();
            eventStreamReader.Setup(x => x.Read(dummyStreamName, 0, 100, false))
                .Returns(expectedEvents);

            var sut = new Infrastructure.Events.EventStore(new Mock<IEventStoreConnection>().Object, eventStreamReader.Object, new JsonDeserializer());

            var actualEvents = sut.GetEvents<string>(dummyStreamName, 0, 100);

            Assert.Equal(expectedContent, actualEvents.First().Content);
            Assert.Equal(expectedMetaData.OccurredAt, actualEvents.First().OccurredAt);
            Assert.Equal(expectedMetaData.EventName, actualEvents.First().Name);
        }
    }
}
