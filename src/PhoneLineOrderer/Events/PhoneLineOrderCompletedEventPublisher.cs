using Infrastructure.Events;

namespace PhoneLineOrderer.Events
{
    public class PhoneLineOrderCompletedEventPublisher : IPhoneLineOrderCompletedEventPublisher
    {
        public const string PhoneLineOrdersCompletedStream = "phone-line-orders-completed-stream";

        private IEventDataCreator eventDataCreator;
        private IEventStore eventStore;

        public PhoneLineOrderCompletedEventPublisher(IEventDataCreator eventDataCreator,
            IEventStore eventStore)
        {
            this.eventDataCreator = eventDataCreator;
            this.eventStore = eventStore;
        }

        public void Publish(PhoneLineOrderCompletedEvent phoneLineOrderCompletedEvent)
        {
            var eventData = this.eventDataCreator.Create(nameof(phoneLineOrderCompletedEvent), phoneLineOrderCompletedEvent);

            this.eventStore.Raise(PhoneLineOrdersCompletedStream, eventData);
        }
    }
}
