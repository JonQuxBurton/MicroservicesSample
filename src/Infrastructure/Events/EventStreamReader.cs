using EventStore.ClientAPI;
using System.Collections.Generic;

namespace Infrastructure.Events
{
    public class EventStreamReader : IEventStreamReader
    {
        private readonly IEventStoreConnection connection;

        public EventStreamReader(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<EventsStreamEvent> Read(string streamName, int start, int count, bool resolveLinkTos)
        {
            var result = this.connection.ReadStreamEventsForwardAsync(
              streamName,
              start,
              count,
              false).Result;

            var events = new List<EventsStreamEvent>();

            foreach (var e in result.Events)
            {
                yield return new EventsStreamEvent()
                {
                    Data = e.Event.Data,
                    Metadata = e.Event.Metadata,
                    OriginalEventNumber = e.OriginalEventNumber
                };
            }
        }
    }
}
