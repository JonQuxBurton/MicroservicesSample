using Nancy;
using Nancy.ModelBinding;
using Customers.Events;
using Customers.Data;

namespace Customers
{
    public class CustomerModule : NancyModule
    {
        public CustomerModule(ICustomerDataStore customerStore, IPhoneLineOrdersPlacedEventPublisher phoneLineOrdersPlacedEventPublisher) : base("/customer")
        {
            base.Post("/{id}/PhoneLines", x =>
            {
                var phoneLineOrder = this.Bind<Resources.PhoneLineOrder>();

                var phoneLineId = customerStore.AddPhoneLine(x.id, phoneLineOrder);

                phoneLineOrdersPlacedEventPublisher.Publish(new PhoneLineOrderPlaced
                {
                    PhoneLineId = phoneLineId,
                    HouseNumber = phoneLineOrder.HouseNumber,
                    Postcode = phoneLineOrder.Postcode
                });

                return HttpStatusCode.Accepted;
            });
        }
    }
}
