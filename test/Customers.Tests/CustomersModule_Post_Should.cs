using Customers.Data;
using Customers.Entities;
using Moq;
using Nancy.Testing;
using Xunit;

namespace Customers.Tests
{
    public class CustomersModule_Post_Should
    {
        private Mock<ICustomerDataStore> customerStoreMock;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public CustomersModule_Post_Should()
        {
            customerStoreMock = new Mock<ICustomerDataStore>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(customerStoreMock.Object);
                with.Module<CustomersModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void SaveToDataStore()
        {
            var expectedCustomer = new Customer()
            {
                Name = "Mallard",
                MobilePhoneNumber = "07900111222"
            };

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedCustomer);

            var response = browser.Post($"/customers", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            customerStoreMock.Verify(x => x.Add(expectedCustomer.Name, expectedCustomer.MobilePhoneNumber), Times.Once);
        }

        [Fact]
        public void ReturnCreated()
        {
            var expectedCustomer = new Customer()
            {
                Name = "Mallard"                
            };

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedCustomer);

            var response = browser.Post($"/customers", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            var actualResult = response.Result;

            Assert.Equal(Nancy.HttpStatusCode.Created, actualResult.StatusCode);
        }
    }
}
