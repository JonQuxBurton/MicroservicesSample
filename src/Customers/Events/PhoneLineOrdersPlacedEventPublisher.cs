using Customers.Config;
using Infrastructure.Events;
using Microsoft.Extensions.Options;

namespace Customers.Events
{
    public class PhoneLineOrdersPlacedEventPublisher : IPhoneLineOrdersPlacedEventPublisher
    {
        private readonly string phoneLineOrdersPlacedStream;
        private IEventDataCreator eventDataCreator;
        private IEventStore eventStore;

        public PhoneLineOrdersPlacedEventPublisher(IEventDataCreator eventDataCreator,
            IEventStore eventStore,
            IOptions<AppSettings> appSettings)
        {
            this.eventDataCreator = eventDataCreator;
            this.eventStore = eventStore;
            this.phoneLineOrdersPlacedStream = appSettings?.Value.PhoneLineOrdersPlacedStream;
        }

        public void Publish(PhoneLineOrderPlaced phoneLineOrderPlaced)
        {
            var eventData = this.eventDataCreator.Create(nameof(phoneLineOrderPlaced), phoneLineOrderPlaced);

            this.eventStore.Raise(phoneLineOrdersPlacedStream, eventData);
        }
    }
}
