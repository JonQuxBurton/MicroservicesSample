using Infrastructure.Events;
using System.Collections.Generic;

namespace Customers.Events
{
    public class PhoneLineOrdersPlacedEventGetter : IPhoneLineOrdersPlacedEventGetter
    {
        public const string PhoneLineOrdersPlacedStream = "phone-line-orders-placed-stream";

        private IEventStore eventStore;

        public PhoneLineOrdersPlacedEventGetter(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return this.eventStore.GetEvents<PhoneLineOrderPlaced>(PhoneLineOrdersPlacedStream, firstEventSequenceNumber, lastEventSequenceNumber);
        }
    }
}
