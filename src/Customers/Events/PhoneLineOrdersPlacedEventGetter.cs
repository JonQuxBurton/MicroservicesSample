using Customers.Config;
using Infrastructure.Events;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Customers.Events
{
    public class PhoneLineOrdersPlacedEventGetter : IPhoneLineOrdersPlacedEventGetter
    {
        private readonly string phoneLineOrdersPlacedStream;
        private IEventStore eventStore;

        public PhoneLineOrdersPlacedEventGetter(IEventStore eventStore, IOptions<AppSettings> appSettings)
        {
            this.eventStore = eventStore;
            this.phoneLineOrdersPlacedStream = appSettings?.Value.PhoneLineOrdersPlacedStream;
        }

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return this.eventStore.GetEvents<PhoneLineOrderPlaced>(phoneLineOrdersPlacedStream, firstEventSequenceNumber, lastEventSequenceNumber);
        }
    }
}
