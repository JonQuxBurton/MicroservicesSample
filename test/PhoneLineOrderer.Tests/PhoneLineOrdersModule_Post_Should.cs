using Moq;
using Nancy.Testing;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using System;
using Xunit;

namespace PhoneLineOrderer.Tests
{
    public class PhoneLineOrdersModule_Post_Should
    {
        private Mock<IPhoneLineOrderSender> phoneLineOrderSenderMock;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrdersModule_Post_Should()
        {
            phoneLineOrderSenderMock = new Mock<IPhoneLineOrderSender>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(new Mock<IPhoneLineOrdererDataStore>().Object);
                with.Module<PhoneLineOrdersModule>();

                with.Dependency(phoneLineOrderSenderMock.Object);
                with.Module<PhoneLineOrdersModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void SendOrderToWholesaler()
        {
            var expectedPhoneLineOrder = new Resources.PhoneLineOrder()
            {
                PhoneLineId = 101,
                HouseNumber = 42,
                Postcode = "S1 1AA",
                Reference = Guid.NewGuid()
            };

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrder);

            browser.Post($"/PhoneLineOrders", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            phoneLineOrderSenderMock.Verify(x => x.Send(
                It.Is<Resources.PhoneLineOrder>(
                    y => y.HouseNumber == expectedPhoneLineOrder.HouseNumber &&
                    y.Postcode == expectedPhoneLineOrder.Postcode &&
                    y.PhoneLineId == expectedPhoneLineOrder.PhoneLineId &&
                    y.Reference == expectedPhoneLineOrder.Reference
                    )), Times.Once);
        }
        
        [Fact]
        public void ReturnAccepted()
        {
            var dummyPhoneLineOrder = new Resources.PhoneLineOrder();

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(dummyPhoneLineOrder);

            var response = browser.Post($"/PhoneLineOrders", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            var actualResult = response.Result;

            Assert.Equal(Nancy.HttpStatusCode.Accepted, actualResult.StatusCode);
        }
    }
}
