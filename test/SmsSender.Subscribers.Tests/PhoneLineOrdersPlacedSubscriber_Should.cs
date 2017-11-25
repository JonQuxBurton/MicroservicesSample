using Infrastructure.Events;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SmsSender.Subscribers
{
    public class PhoneLineOrdersPlacedSubscriberShould
    {
        [Fact]
        public void SendSms()
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

            var orderPlacedSmsSenderMock = new Mock<IOrderPlacedSmsSender>();

            var eventGetterMock = new Mock<IEventGetter>();
            eventGetterMock.Setup(
                x => x.Get("PhoneLineOrdersPlaced", 0, 100))
                    .Returns(Task.FromResult<IRestResponse>(new RestResponse()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = serializedEvents
                    }));

            var sut = new PhoneLineOrdersPlacedSubscriber(eventGetterMock.Object, orderPlacedSmsSenderMock.Object);

            sut.Poll(null, new EventArgs());

            orderPlacedSmsSenderMock.Verify(x => x.Send(expectedPhoneLineOrder.phoneLineId), Times.Once);
        }
    }
}
