using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Moq;
using Newtonsoft.Json;
using PhoneLineOrderer.Domain;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace PhoneLineOrderer.OrdersPlacedSubscriber.Tests
{
    public class Subscriber_Should
    {
        [Fact]
        public void SendOrderToWholesaler()
        {
            var expectedPhoneLineOrder = new
            {
                houseNumber = 101,
                phoneLineId = 202,
                postcode = "S1 1AA",
                reference = Guid.NewGuid()
            };

            var expectedEvent = new Event(101, new DateTimeOffset(), "name", expectedPhoneLineOrder);
            var events = new List<Event>() { expectedEvent };
            var serializedEvents = JsonConvert.SerializeObject(events.AsEnumerable());

            var phoneLineOrderSenderMock = new Mock<IPhoneLineOrderSender>();
            var eventGetterMock = new Mock<IEventGetter>();
            eventGetterMock.Setup(
                x => x.Get("PhoneLineOrdersPlaced", 0, 100))
                    .Returns(new RestResponse()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = serializedEvents
                    });
            var guidCreatorMock = new Mock<IGuidCreator>();
            guidCreatorMock.Setup(x => x.Create())
                .Returns(expectedPhoneLineOrder.reference);
            var loggerFactoryMock = new Mock<ILoggerFactory>();

            var sut = new Subscriber(eventGetterMock.Object, phoneLineOrderSenderMock.Object, guidCreatorMock.Object, loggerFactoryMock.Object);

            sut.Poll(null, new EventArgs());

            phoneLineOrderSenderMock.Verify(
                x => x.Send(
                    It.Is<Resources.PhoneLineOrder>(
                        y => y.HouseNumber == expectedPhoneLineOrder.houseNumber &&
                        y.PhoneLineId == expectedPhoneLineOrder.phoneLineId &&
                        y.Postcode == expectedPhoneLineOrder.postcode &&
                        y.Reference == expectedPhoneLineOrder.reference
                        )
                    ), Times.Once);
        }
    }
}
