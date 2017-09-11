
using Infrastructure.Events;

namespace Customers.Events
{
    public class PhoneLineOrdersPlacedEventPublisher : IPhoneLineOrdersPlacedEventPublisher
    {
        public const string PhoneLineOrdersPlacedStream = "phone-line-orders-placed-stream";

        private IEventDataCreator eventDataCreator;
        private IEventStore eventStore;

        public PhoneLineOrdersPlacedEventPublisher(IEventDataCreator eventDataCreator,
            IEventStore eventStore)
        {
            this.eventDataCreator = eventDataCreator;
            this.eventStore = eventStore;
        }

        public void Publish(PhoneLineOrderPlaced phoneLineOrderPlaced)
        {
            var eventData = this.eventDataCreator.Create(nameof(phoneLineOrderPlaced), phoneLineOrderPlaced);

            this.eventStore.Raise(PhoneLineOrdersPlacedStream, eventData);
        }
    }
}
