using FakeBt.Config;
using FakeBt.Data;
using Infrastructure.Rest;
using Microsoft.Extensions.Options;
using Moq;
using RestSharp;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FakeBt.OrderUpdater.Tests
{
    public class PhoneLineOrderUpdater_Update_Should
    {
        [Fact]
        public void CompleteOrder()
        {
            var expectedPendingOrder = new Resources.BtOrderInbound {
                Id = 101
            };

            var btOrdersDataStoreMock = new Mock<IBtOrdersDataStore>();
            btOrdersDataStoreMock.Setup(x => x.GetNew()).Returns(new[] { expectedPendingOrder });

            var appSettings = new AppSettings
            {
                PhoneLineOrdererWebServiceUrl = "PhoneLineOrdererWebServiceUrl"
            };
            var options = Options.Create(appSettings);

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrderCompleted", It.IsAny<object>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.OK }));

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create(appSettings.PhoneLineOrdererWebServiceUrl))
                .Returns(restPosterMock.Object);
            var loggerFactoryMock = new Mock<ILoggerFactory>();

            var sut = new PhoneLineOrderUpdater(btOrdersDataStoreMock.Object, options, restPosterFactoryMock.Object, loggerFactoryMock.Object);

            sut.Update(new object(), new EventArgs());

            btOrdersDataStoreMock.Verify(
                x => x.Complete(
                    It.Is<Resources.BtOrderInbound>(
                        y => y.Id ==  expectedPendingOrder.Id)
                    ), Times.Once);
        }

        [Fact]
        public void NotCompleteOrderWhenCallToPhoneLineOrderCompletedFails()
        {
            var dummyPendingOrder = new Resources.BtOrderInbound
            {
                Id = 101
            };

            var btOrdersDataStoreMock = new Mock<IBtOrdersDataStore>();
            btOrdersDataStoreMock.Setup(x => x.GetNew()).Returns(new[] { dummyPendingOrder });

            var appSettings = new AppSettings
            {
                PhoneLineOrdererWebServiceUrl = "PhoneLineOrdererWebServiceUrl"
            };
            var options = Options.Create(appSettings);

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrderCompleted", It.IsAny<object>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted }));

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create(appSettings.PhoneLineOrdererWebServiceUrl))
                .Returns(restPosterMock.Object);
            var loggerFactory = new LoggerFactory();

            var sut = new PhoneLineOrderUpdater(btOrdersDataStoreMock.Object, options, restPosterFactoryMock.Object, loggerFactory);

            sut.Update(new object(), new EventArgs());

            btOrdersDataStoreMock.Verify(
                x => x.Complete(
                    It.IsAny<Resources.BtOrderInbound>()
                    ), Times.Never);
        }
    }
}
