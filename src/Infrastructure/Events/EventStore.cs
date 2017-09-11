using EventStore.ClientAPI;
using Infrastructure.Serialization;
using System.Collections.Generic;

namespace Infrastructure.Events
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreConnection connection;
        private readonly IEventStreamReader eventStreamReader;
        private readonly IDeserializer deserializer;

        public EventStore(IEventStoreConnection connection, 
            IEventStreamReader eventStreamReader,
            IDeserializer deserializer
            )
        {
            this.connection = connection;
            this.eventStreamReader = eventStreamReader;
            this.deserializer = deserializer;
        }

        public void Raise(string streamName, EventData eventData)
        {
            connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventData).Wait();
        }

        public IEnumerable<Event> GetEvents<T>(string streamName, long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            var events = eventStreamReader.Read(streamName, (int)firstEventSequenceNumber, (int)(lastEventSequenceNumber - firstEventSequenceNumber), false);

            foreach (var e in events)
            {
                var content = this.deserializer.DeserializeBytes<T>(e.Data);
                var metadata = this.deserializer.DeserializeBytes<EventMetaData>(e.Metadata);

                yield return new Event(e.OriginalEventNumber, metadata.OccurredAt, metadata.EventName, content);
            }
        }
    }
}
