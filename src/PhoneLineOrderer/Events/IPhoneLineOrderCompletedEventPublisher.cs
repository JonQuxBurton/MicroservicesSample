namespace PhoneLineOrderer.Events
{
    public interface IPhoneLineOrderCompletedEventPublisher
    {
        void Publish(PhoneLineOrderCompletedEvent phoneLineOrderPlaced);
    }
}
