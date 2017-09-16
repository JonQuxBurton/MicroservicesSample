using Infrastructure.Events;
using Microsoft.Extensions.Options;
using PhoneLineOrderer.Config;
using System.Collections.Generic;

namespace PhoneLineOrderer.Events
{
    public class PhoneLineOrderCompletedEventGetter : IPhoneLineOrderCompletedEventGetter
    {
        private readonly string phoneLineOrdersCompletedStream;

        private IEventStore eventStore;

        public PhoneLineOrderCompletedEventGetter(IEventStore eventStore, IOptions<AppSettings> appSettings)
        {
            this.eventStore = eventStore;
            this.phoneLineOrdersCompletedStream = appSettings?.Value.PhoneLineOrdersCompletedStream;
        }

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return this.eventStore.GetEvents<PhoneLineOrderCompletedEvent>(phoneLineOrdersCompletedStream, firstEventSequenceNumber, lastEventSequenceNumber);
        }
    }
}
