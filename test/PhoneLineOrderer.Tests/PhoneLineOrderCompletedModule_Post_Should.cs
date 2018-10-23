using Moq;
using Nancy.Testing;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Entities;
using PhoneLineOrderer.Events;
using System;
using Microsoft.Extensions.Logging;
using Xunit;

namespace PhoneLineOrderer.Tests
{
    public class PhoneLineOrderCompletedModule_Post_Should
    {
        private Mock<ILoggerFactory> loggerFactoryMock;
        private Mock<IPhoneLineOrdererDataStore> phoneLineOrdersDataStoreMock;
        private Mock<IPhoneLineOrderCompletedEventPublisher> phoneLineOrderCompletedEventPublisherMock;
        
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrderCompletedModule_Post_Should()
        {
            loggerFactoryMock = new Mock<ILoggerFactory>();
            phoneLineOrdersDataStoreMock = new Mock<IPhoneLineOrdererDataStore>();
            phoneLineOrderCompletedEventPublisherMock = new Mock<IPhoneLineOrderCompletedEventPublisher>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(phoneLineOrdersDataStoreMock.Object);
                with.Dependency(phoneLineOrderCompletedEventPublisherMock.Object);
                with.Dependency(loggerFactoryMock.Object);
                with.Module<PhoneLineOrderCompletedModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void SaveToDataStore()
        {
            var expectedPhoneLineOrderCompleted = new Resources.PhoneLineOrderCompleted()
            {
                PhoneNumber = "01140001111",
                Reference = Guid.NewGuid(),
                Status = "Complete"
            };

            phoneLineOrdersDataStoreMock.Setup(x => x.GetByReference(expectedPhoneLineOrderCompleted.Reference)).Returns(new PhoneLineOrder());

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrderCompleted);

            browser.Post($"/PhoneLineOrderCompleted", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            phoneLineOrdersDataStoreMock.Verify(
                x => x.Receive(
                    It.Is<Resources.PhoneLineOrderCompleted>(
                        y => y.PhoneNumber == expectedPhoneLineOrderCompleted.PhoneNumber &&
                                y.Reference == expectedPhoneLineOrderCompleted.Reference &&
                                y.Status == expectedPhoneLineOrderCompleted.Status)
                                ), Times.Once);
        }

        [Fact]
        public void PublishOrderCompletedEvent()
        {
            var expectedPhoneLineOrderCompleted = new Resources.PhoneLineOrderCompleted()
            {
                PhoneNumber = "01140001111",
                Reference = Guid.NewGuid(),
                Status = "Complete"
            };
            var expectedPhoneLineOrder = new PhoneLineOrder {
                PhoneLineId = 202
            };

            phoneLineOrdersDataStoreMock.Setup(
                x => x.GetByReference(expectedPhoneLineOrderCompleted.Reference))
                .Returns(expectedPhoneLineOrder);

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrderCompleted);

            browser.Post($"/PhoneLineOrderCompleted", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            phoneLineOrderCompletedEventPublisherMock.Verify(
                x => x.Publish(
                    It.Is<PhoneLineOrderCompletedEvent>(
                        y => y.PhoneLineId == expectedPhoneLineOrder.PhoneLineId &&
                                y.PhoneNumber == expectedPhoneLineOrderCompleted.PhoneNumber &&
                                y.Status == expectedPhoneLineOrderCompleted.Status)
                                ), Times.Once);
        }


        [Fact]
        public void ReturnOk()
        {
            var expectedPhoneLineOrderCompleted = new Resources.PhoneLineOrderCompleted()
            {
                PhoneNumber = "01140001111",
                Reference = Guid.NewGuid(),
                Status = "Complete"
            };

            phoneLineOrdersDataStoreMock.Setup(x => x.GetByReference(expectedPhoneLineOrderCompleted.Reference)).Returns(new PhoneLineOrder());

            var json = new Nancy.Json.JavaScriptSerializer().Serialize(expectedPhoneLineOrderCompleted);

            var response = browser.Post($"/PhoneLineOrderCompleted", with =>
            {
                with.HttpRequest();
                with.Body(json, "application/json");
            });

            var actualResult = response.Result;
            Assert.Equal(Nancy.HttpStatusCode.OK, actualResult.StatusCode);
        }
    }
}
