using Infrastructure.Events;
using System.Collections.Generic;

namespace PhoneLineOrderer.Events
{
    public interface IPhoneLineOrderCompletedEventGetter
    {
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
    }
}
