using Moq;
using Nancy.Testing;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using PhoneLineOrderer.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhoneLineOrderer.Tests
{
    public class PhoneLineOrdersModule_Get_Should
    {
        private Mock<IPhoneLineOrdererDataStore> phoneLineOrdersDataStore;
        private Mock<IPhoneLineOrderSender> orderSender;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrdersModule_Get_Should()
        {
            phoneLineOrdersDataStore = new Mock<IPhoneLineOrdererDataStore>();
            orderSender = new Mock<IPhoneLineOrderSender>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(phoneLineOrdersDataStore.Object);
                with.Module<PhoneLineOrdersModule>();

                with.Dependency(orderSender.Object);
                with.Module<PhoneLineOrdersModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void ReturnPhoneLineOrders()
        {
            var expectedPhoneLineId = 101;
            var expected = new List<PhoneLineOrder>()
            {
                new PhoneLineOrder
                {
                    Id = 201,
                    PhoneLineId = expectedPhoneLineId,
                },
                new PhoneLineOrder
                {
                    Id = 202,
                    PhoneLineId = expectedPhoneLineId,
                }
            };

            phoneLineOrdersDataStore.Setup(x => x.GetByPhoneLineId(expectedPhoneLineId)).Returns(expected);

            var response = browser.Get($"/PhoneLineOrders/{expectedPhoneLineId}", with =>
            {
                with.HttpRequest();
            });

            var responseBody = response.Result.Body;

            Assert.Equal(Nancy.HttpStatusCode.OK, response.Result.StatusCode);

            var actual = new Nancy.Json.JavaScriptSerializer().Deserialize<PhoneLineOrder[]>(responseBody.AsString());

            Assert.Equal(expected.Count, actual.Length);
            Assert.Contains(actual.ToList(), x => x.Id == expected.ElementAt(0).Id);
            Assert.Contains(actual.ToList(), x => x.Id == expected.ElementAt(1).Id);
        }
    }
}
