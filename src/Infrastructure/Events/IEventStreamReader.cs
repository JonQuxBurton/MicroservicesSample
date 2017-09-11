using System.Collections.Generic;

namespace Infrastructure.Events
{
    public interface IEventStreamReader
    {
        IEnumerable<EventsStreamEvent> Read(string streamName, int start, int count, bool resolveLinkTos);
    }
}
