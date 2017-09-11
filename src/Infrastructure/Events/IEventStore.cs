using EventStore.ClientAPI;
using System.Collections.Generic;

namespace Infrastructure.Events
{
    public interface IEventStore
    {
        void Raise(string streamName, EventData eventData);
        IEnumerable<Event> GetEvents<T>(string streamName, long firstEventSequenceNumber, long lastEventSequenceNumber);
    }
}
