using System.Linq;
using Customers.Data;
using Customers.Entities;
using Nancy;
using Nancy.ModelBinding;

namespace Customers
{
    public class CustomersModule : NancyModule
    {
        public CustomersModule(ICustomerDataStore customerStore) : base("/customers")
        {
            Get("/phonelines/{phoneLineId}", x =>
            {
                int phoneLineId = x.phoneLineId;

                return customerStore.GetByPhoneLineId(phoneLineId).FirstOrDefault();
            });
            Post("", x =>
            {
                var customer = this.Bind<Customer>();

                customerStore.Add(customer.Name, customer.MobilePhoneNumber);

                return HttpStatusCode.Created;
            });
        }
    }
}