using FakeBt.Data;
using Moq;
using Nancy.Testing;
using System;
using Xunit;

namespace FakeBt.Tests
{
    public class PhoneLineOrdersModule_Post_Should
    {
        private Mock<IBtOrdersDataStore> btOrdersDataStoreMock;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrdersModule_Post_Should()
        {
            btOrdersDataStoreMock = new Mock<IBtOrdersDataStore>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(btOrdersDataStoreMock.Object);
                with.Module<PhoneLineOrdersModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void StoreReceivedOrder()
        {
            var expectedPhoneLineOrder = new Resources.BtOrderInbound()
            {
                HouseNumber = 101,
                Postcode = "S1 1AA",
                Reference = Guid.NewGuid(),
            };

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrder);

            browser.Post($"/PhoneLineOrders", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            this.btOrdersDataStoreMock.Verify(
                x => x.Receive(
                    It.Is<Resources.BtOrderInbound>(
                        y => y.HouseNumber == expectedPhoneLineOrder.HouseNumber &&
                            y.Postcode == expectedPhoneLineOrder.Postcode &&
                            y.Reference == expectedPhoneLineOrder.Reference
                    )), 
                Times.Once);
        }

        [Fact]
        public void ReturnAccepted()
        {
            var dummyPhoneLineOrder = new Resources.BtOrderInbound();

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