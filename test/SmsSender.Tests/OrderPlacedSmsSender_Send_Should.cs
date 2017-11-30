using Infrastructure.DateTimeUtilities;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using Moq;
using RestSharp;
using SmsSender.Data;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmsSender.Tests
{
#pragma warning disable AvoidAsyncVoid // Avoid async void
    public class OrderPlacedSmsSender_Send_Should
    {
        private Customer expectedCustomer;
        private int expectedPhoneLineId;
        private DateTimeOffset expectedDateTimeOffset;

        private Mock<ISmsSenderDataStore> smsSenderDataStoreMock;
        private Mock<IWebServiceGetter> webServiceGetterMock;
        private Mock<IDeserializer> deserializerMock;
        private Mock<IDateTimeOffsetCreator> dateTimeOffsetCreatorMock;

        public OrderPlacedSmsSender_Send_Should()
        {
            expectedCustomer = new Customer()
            {
                Id = 101
            };
            expectedPhoneLineId = 202;
            expectedDateTimeOffset = new DateTimeOffset(new DateTime(2001, 5, 4));

            smsSenderDataStoreMock = new Mock<ISmsSenderDataStore>();
            webServiceGetterMock = new Mock<IWebServiceGetter>();
            webServiceGetterMock.Setup(x => x.Get($"/customers/phonelines/{expectedPhoneLineId}"))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse() { StatusCode = System.Net.HttpStatusCode.OK, Content = "Content" }));
            deserializerMock = new Mock<IDeserializer>();
            deserializerMock.Setup(x => x.Deserialize<Customer>("Content"))
                .Returns(expectedCustomer);
            dateTimeOffsetCreatorMock = new Mock<IDateTimeOffsetCreator>();
            dateTimeOffsetCreatorMock.Setup(x => x.Now)
                .Returns(expectedDateTimeOffset);
        }

        [Fact]
        public async void SendSms()
        {
            var sut = new OrderPlacedSmsSender(smsSenderDataStoreMock.Object, webServiceGetterMock.Object, deserializerMock.Object, dateTimeOffsetCreatorMock.Object);

            await sut.Send(expectedPhoneLineId);

            smsSenderDataStoreMock.Verify(x => x.Send(expectedCustomer, OrderPlacedSmsSender.TextMessage, expectedDateTimeOffset), Times.Once);
        }

        [Fact]
        public async void ReturnTrue()
        {
            var sut = new OrderPlacedSmsSender(smsSenderDataStoreMock.Object, webServiceGetterMock.Object, deserializerMock.Object, dateTimeOffsetCreatorMock.Object);

            var actual = await sut.Send(expectedPhoneLineId);

            Assert.True(actual);
        }

        [Fact]
        public async void ReturnFalseWhenCannotGetPhoneLine()
        {
            webServiceGetterMock.Setup(x => x.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<IRestResponse>(new RestResponse { StatusCode = System.Net.HttpStatusCode.InternalServerError }));

            var sut = new OrderPlacedSmsSender(smsSenderDataStoreMock.Object, webServiceGetterMock.Object, deserializerMock.Object, dateTimeOffsetCreatorMock.Object);

            var actual = await sut.Send(expectedPhoneLineId);

            Assert.False(actual);
            smsSenderDataStoreMock.Verify(x => x.Send(It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>()), Times.Never);
        }
    }
#pragma warning restore AvoidAsyncVoid // Avoid async void
}
