using Infrastructure.Events;
using Microsoft.Extensions.Options;
using PhoneLineOrderer.Config;

namespace PhoneLineOrderer.Events
{
    public class PhoneLineOrderCompletedEventPublisher : IPhoneLineOrderCompletedEventPublisher
    {
        private readonly string phoneLineOrdersCompletedStream;
        private IEventDataCreator eventDataCreator;
        private IEventStore eventStore;

        public PhoneLineOrderCompletedEventPublisher(IEventDataCreator eventDataCreator,
            IEventStore eventStore,
            IOptions<AppSettings> appSettings)
        {
            this.eventDataCreator = eventDataCreator;
            this.eventStore = eventStore;
            this.phoneLineOrdersCompletedStream = appSettings?.Value.PhoneLineOrdersCompletedStream;
        }

        public void Publish(PhoneLineOrderCompletedEvent phoneLineOrderCompletedEvent)
        {
            var eventData = this.eventDataCreator.Create(nameof(phoneLineOrderCompletedEvent), phoneLineOrderCompletedEvent);

            this.eventStore.Raise(phoneLineOrdersCompletedStream, eventData);
        }
    }
}
