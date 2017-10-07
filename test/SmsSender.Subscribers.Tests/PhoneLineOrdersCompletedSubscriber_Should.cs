using Infrastructure.Events;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SmsSender.Subscribers.Tests
{
    public class PhoneLineOrdersCompletedSubscriber_Should
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

            var orderCompletedSmsSenderMock = new Mock<IOrderCompletedSmsSender>();

            var eventGetterMock = new Mock<IEventGetter>();
            eventGetterMock.Setup(
                x => x.Get("PhoneLineOrdersCompleted", 0, 100))
                    .Returns(new RestResponse()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = serializedEvents
                    });

            var sut = new PhoneLineOrdersCompletedSubscriber(eventGetterMock.Object, orderCompletedSmsSenderMock.Object);

            sut.Poll(null, new EventArgs());

            orderCompletedSmsSenderMock.Verify(x => x.Send(expectedPhoneLineOrder.phoneLineId), Times.Once);
        }
    }
}
