using Customers.Data;
using Customers.Entities;
using Nancy;
using Nancy.ModelBinding;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Customers
{
    public class CustomersModule : NancyModule
    {
        private readonly ILogger logger;

        public CustomersModule(ICustomerDataStore customerStore) : base ("/customers")
        {
            Get("/phonelines/{phoneLineId}", x => {
                int phoneLineId = x.phoneLineId;

                return customerStore.GetByPhoneLineId(phoneLineId).FirstOrDefault();

            });
            Post("", x => {
                var customer = this.Bind<Customer>();

                customerStore.Add(customer.Name, customer.MobilePhoneNumber);

                return HttpStatusCode.Created;
            });
        }
    }
}
