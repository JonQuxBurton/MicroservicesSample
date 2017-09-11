using Infrastructure.Events;
using System.Collections.Generic;

namespace PhoneLineOrderer.Events
{
    public class PhoneLineOrderCompletedEventGetter : IPhoneLineOrderCompletedEventGetter
    {
        public const string PhoneLineOrdersCompletedStream = "phone-line-orders-completed-stream";

        private IEventStore eventStore;

        public PhoneLineOrderCompletedEventGetter(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return this.eventStore.GetEvents<PhoneLineOrderCompletedEvent>(PhoneLineOrdersCompletedStream, firstEventSequenceNumber, lastEventSequenceNumber);
        }
    }
}
