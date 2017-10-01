using Customers.Data;
using Customers.Entities;
using Moq;
using Nancy.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Customers.Tests
{
    public class CustomersModule_Get_Should
    {
        private Browser browser;
        private ConfigurableBootstrapper bootstrapper;
        private Mock<ICustomerDataStore> customerDataStoreMock;

        public CustomersModule_Get_Should()
        {
            customerDataStoreMock = new Mock<ICustomerDataStore>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(customerDataStoreMock.Object);
                with.Module<CustomersModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void ReturnCustomer()
        {
            var phoneLineId = 101;
            var expectedCustomer = new Customer()
            {
                Id = 201,
                MobilePhoneNumber = "07900111222",
                Name = "Apollo Ltd"
            };
            
            this.customerDataStoreMock.Setup(x => x.GetByPhoneLineId(phoneLineId)).Returns(new[] { expectedCustomer });

            var response = browser.Get($"customers/phonelines/{phoneLineId}", with =>
            {
                with.HttpRequest();
            });
            
            Assert.Equal(Nancy.HttpStatusCode.OK, response.Result.StatusCode);

            var responseBody = response.Result.Body;
            var actualCustomer = JsonConvert.DeserializeObject<Customer>(responseBody.AsString());

            Assert.NotNull(actualCustomer);
            Assert.Equal(expectedCustomer.Id, actualCustomer.Id);
            Assert.Equal(expectedCustomer.Name, actualCustomer.Name);
            Assert.Equal(expectedCustomer.MobilePhoneNumber, actualCustomer.MobilePhoneNumber);
        }
    }
}
