using Infrastructure.Events;
using System.Collections.Generic;

namespace Customers.Events
{
    public interface IPhoneLineOrdersPlacedEventGetter
    {
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
    }
}
