using FakeBt.Data;
using FakeBt.OrderUpdater.Configuration;
using Infrastructure.Rest;
using Moq;
using RestSharp;
using System;
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

            var configGetterMock = new Mock<IConfigGetter>();
            configGetterMock.Setup(x => x.PhoneLineOrdererUrl)
                .Returns("PhoneLineOrdererUrl");

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrderCompleted", It.IsAny<object>()))
                .Returns(new RestResponse { StatusCode = System.Net.HttpStatusCode.OK });

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("PhoneLineOrdererUrl"))
                .Returns(restPosterMock.Object);

            var sut = new PhoneLineOrderUpdater(btOrdersDataStoreMock.Object, configGetterMock.Object, restPosterFactoryMock.Object);

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

            var configGetterMock = new Mock<IConfigGetter>();
            configGetterMock.Setup(x => x.PhoneLineOrdererUrl)
                .Returns("PhoneLineOrdererUrl");

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrderCompleted", It.IsAny<object>()))
                .Returns(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted });

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("PhoneLineOrdererUrl"))
                .Returns(restPosterMock.Object);

            var sut = new PhoneLineOrderUpdater(btOrdersDataStoreMock.Object, configGetterMock.Object, restPosterFactoryMock.Object);

            sut.Update(new object(), new EventArgs());

            btOrdersDataStoreMock.Verify(
                x => x.Complete(
                    It.IsAny<Resources.BtOrderInbound>()
                    ), Times.Never);
        }
    }
}
