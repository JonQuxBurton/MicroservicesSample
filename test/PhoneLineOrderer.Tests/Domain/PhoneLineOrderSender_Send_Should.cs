using Infrastructure.DateTimeUtilities;
using Infrastructure.Rest;
using Microsoft.Extensions.Options;
using Moq;
using PhoneLineOrderer.Config;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using PhoneLineOrderer.Entities;
using RestSharp;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PhoneLineOrderer.Tests.Domain
{
    public class PhoneLineOrderSender_Send_Should
    {
        [Fact]
        public async Task SendOrderToWholesalerAsync()
        {
            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted }));

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var options = Options.Create(new AppSettings() {
                FakeBtWebServiceUrl = "FakeBtWebServiceUrl"
            });

            var sut = new PhoneLineOrderSender(new Mock<IPhoneLineOrdererDataStore>().Object, options, restPosterFactoryMock.Object, new Mock<IDateTimeOffsetCreator>().Object);

            await sut.Send(new Resources.PhoneLineOrder());
            
            restPosterMock.Verify(x => x.Post("PhoneLineOrders", It.IsAny<object>()));
        }

        [Fact]
        public async Task SaveToDataStoreAsync()
        {
            var expectedPhoneLineOrder = new Resources.PhoneLineOrder
            {
                PhoneLineId = 101,
                HouseNumber = 202,
                Postcode = "S1 1AA",
                Reference = Guid.NewGuid()
            };
            var expectedCreatedAt = new DateTimeOffset(new DateTime(2001, 5, 4));

            var dateTimeOffsetCreatorMock = new Mock<IDateTimeOffsetCreator>();
            dateTimeOffsetCreatorMock.Setup(x => x.Now)
                .Returns(expectedCreatedAt);

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted }));

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var options = Options.Create(new AppSettings()
            {
                FakeBtWebServiceUrl = "FakeBtWebServiceUrl"
            });

            var phoneLineOrdersDataStore = new Mock<IPhoneLineOrdererDataStore>();
            
            var sut = new PhoneLineOrderSender(phoneLineOrdersDataStore.Object, options, restPosterFactoryMock.Object, dateTimeOffsetCreatorMock.Object);

            await sut.Send(expectedPhoneLineOrder);

            phoneLineOrdersDataStore.Verify(x => x.Add(
                It.Is<PhoneLineOrder>(
                    y => y.PhoneLineId == expectedPhoneLineOrder.PhoneLineId &&
                        y.Status == "New" &&
                        y.HouseNumber == expectedPhoneLineOrder.HouseNumber &&
                        y.Postcode == expectedPhoneLineOrder.Postcode &&
                        y.ExternalReference == expectedPhoneLineOrder.Reference &&
                        y.CreatedAt == expectedCreatedAt
                    )), Times.Once);
        }

        [Fact]
        public async Task ReturnTrueWhenSendIsSuccessfulAsync()
        {
            var options = Options.Create(new AppSettings()
            {
                FakeBtWebServiceUrl = "FakeBtWebServiceUrl"
            });

            var restPosterMock = new Mock<IRestPoster>();
            restPosterMock.Setup(x => x.Post("PhoneLineOrders", It.IsAny<object>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.Accepted }));

            var restPosterFactoryMock = new Mock<IRestPosterFactory>();
            restPosterFactoryMock.Setup(x => x.Create("FakeBtWebServiceUrl"))
                .Returns(restPosterMock.Object);

            var sut = new PhoneLineOrderSender(new Mock<IPhoneLineOrdererDataStore>().Object, options, restPosterFactoryMock.Object, new Mock<IDateTimeOffsetCreator>().Object);

            var actual = await sut.Send(new Resources.PhoneLineOrder());

            Assert.True(actual);
        }
    }
}
