using Customers.Data;
using Customers.Entities;
using Nancy;
using Nancy.ModelBinding;

namespace Customers
{
    public class CustomersModule : NancyModule
    {
        public CustomersModule(ICustomerDataStore customerStore) : base ("/customers")
        {
            Post("", x => {
                var customer = this.Bind<Customer>();

                customerStore.Add(customer.Name);

                return HttpStatusCode.Created;
            });
        }
    }
}
