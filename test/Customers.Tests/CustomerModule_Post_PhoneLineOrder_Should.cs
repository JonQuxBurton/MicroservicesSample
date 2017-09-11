using Nancy.Testing;
using Xunit;
using Moq;
using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Serialization;
using Customers.Events;
using Customers.Data;
using Customers.Resources;

namespace Customers.Tests
{
    public class CustomerModule_Post_PhoneLineOrder_Should
    {
        private Mock<ICustomerDataStore> customerStoreMock;
        private Mock<IPhoneLineOrdersPlacedEventGetter> eventStoreMock;
        private Mock<IPhoneLineOrdersPlacedEventPublisher> phoneLineOrdersPlacedEventPublisherMock;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public CustomerModule_Post_PhoneLineOrder_Should()
        {
            customerStoreMock = new Mock<ICustomerDataStore>();
            eventStoreMock = new Mock<IPhoneLineOrdersPlacedEventGetter>();
            phoneLineOrdersPlacedEventPublisherMock = new Mock<IPhoneLineOrdersPlacedEventPublisher>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(new Mock<IEventStoreConnection>());
                with.Module<CustomerModule>();

                with.Dependency(new Mock<ISerializer>());
                with.Module<CustomerModule>();

                with.Dependency(new Mock<IEventDataCreator>());
                with.Module<CustomerModule>();

                with.Dependency(new Mock<IEventStore>());
                with.Module<CustomerModule>();

                with.Dependency(customerStoreMock.Object);
                with.Module<CustomerModule>();

                with.Dependency(eventStoreMock.Object);
                with.Module<CustomerModule>();

                with.Dependency(phoneLineOrdersPlacedEventPublisherMock.Object);
                with.Module<CustomerModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void SaveToDataStore()
        {
            var expectedPhoneLineOrder = new PhoneLineOrder()
            {
                HouseNumber = 42,
                Postcode = "S1 1AA"
            };
            var expectedCustomerId = 101;

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrder);
            
            browser.Post($"/customer/{expectedCustomerId}/PhoneLines", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            customerStoreMock.Verify(x => x.AddPhoneLine(expectedCustomerId, It.Is<PhoneLineOrder>(y => y.HouseNumber == expectedPhoneLineOrder.HouseNumber && 
                y.Postcode == expectedPhoneLineOrder.Postcode)), Times.Once);
        }

        [Fact]
        public void PublishEvent()
        {
            var expectedPhoneLineOrder = new PhoneLineOrder()
            {
                HouseNumber = 101,
                Postcode = "S1 1AA"
            };
            var expectedPhoneLineId = 42;
            var dummyCustomerId = 101;

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrder);

            customerStoreMock.Setup(x => x.AddPhoneLine(dummyCustomerId, It.IsAny<PhoneLineOrder>())).Returns(expectedPhoneLineId);

            browser.Post($"/customer/{dummyCustomerId}/PhoneLines", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            phoneLineOrdersPlacedEventPublisherMock.Verify(x => x.Publish(It.Is<PhoneLineOrderPlaced>(y => y.HouseNumber == expectedPhoneLineOrder.HouseNumber && 
                y.Postcode == expectedPhoneLineOrder.Postcode && 
                y.PhoneLineId == expectedPhoneLineId)), Times.Once);
        }

        [Fact]
        public void ReturnAccepted()
        {
            var expectedPhoneLineOrder = new PhoneLineOrder()
            {
                HouseNumber = 42,
                Postcode = "S1 1AA"
            };
            var expectedCustomerId = 101;

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrder);

            var response = browser.Post($"/customer/{expectedCustomerId}/PhoneLines", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            var actualResult = response.Result;

            Assert.Equal(Nancy.HttpStatusCode.Accepted, actualResult.StatusCode);
        }
    }
}
