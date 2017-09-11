namespace Customers.Events
{
    public interface IPhoneLineOrdersPlacedEventPublisher
    {
        void Publish(PhoneLineOrderPlaced phoneLineOrderPlaced);
    }
}