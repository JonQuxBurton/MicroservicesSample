using Infrastructure.Rest;
using Moq;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using PhoneLineOrderer.Entities;
using RestSharp;
using System;
using Xunit;

namespace PhoneLineOrderer.Tests.Domain
{
    public class PhoneLineOrderSender_Send_Should
    {
        [Fact]
        public void SendOrderToWholesaler()
        {
            var configGetterMock = new Mock<IConfigGetter>();
            configGetterMock.Setup(x => x.FakeBtWebServiceUrl)
                .Returns("FakeBtWebServiceUrl");

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted });

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var sut = new PhoneLineOrderSender(new Mock<IPhoneLineOrdererDataStore>().Object, configGetterMock.Object, restPosterFactoryMock.Object);

            sut.Send(new Resources.PhoneLineOrder());
            
            restPosterMock.Verify(x => x.Post("PhoneLineOrders", It.IsAny<object>()));
        }

        [Fact]
        public void SaveToDataStore()
        {
            var expectedPhoneLineOrder = new Resources.PhoneLineOrder
            {
                PhoneLineId = 101,
                HouseNumber = 202,
                Postcode = "S1 1AA",
                Reference = Guid.NewGuid()
            };

            var configGetterMock = new Mock<IConfigGetter>();
            configGetterMock.Setup(x => x.FakeBtWebServiceUrl)
                .Returns("FakeBtWebServiceUrl");

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted });

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var phoneLineOrdersDataStore = new Mock<IPhoneLineOrdererDataStore>();
            
            var sut = new PhoneLineOrderSender(phoneLineOrdersDataStore.Object, configGetterMock.Object, restPosterFactoryMock.Object);

            sut.Send(expectedPhoneLineOrder);

            phoneLineOrdersDataStore.Verify(x => x.Add(
                It.Is<PhoneLineOrder>(
                    y => y.PhoneLineId == expectedPhoneLineOrder.PhoneLineId &&
                        y.Status == "New" &&
                        y.HouseNumber == expectedPhoneLineOrder.HouseNumber &&
                        y.Postcode == expectedPhoneLineOrder.Postcode &&
                        y.ExternalReference == expectedPhoneLineOrder.Reference
                    )), Times.Once);
        }

        [Fact]
        public void ReturnTrueWhenSendIsSuccessful()
        {
            var configGetterMock = new Mock<IConfigGetter>();
            configGetterMock.Setup(x => x.FakeBtWebServiceUrl)
                .Returns("FakeBtWebServiceUrl");

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted });

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var sut = new PhoneLineOrderSender(new Mock<IPhoneLineOrdererDataStore>().Object, configGetterMock.Object, restPosterFactoryMock.Object);

            var actual = sut.Send(new Resources.PhoneLineOrder());

            Assert.True(actual);
        }
    }
}
