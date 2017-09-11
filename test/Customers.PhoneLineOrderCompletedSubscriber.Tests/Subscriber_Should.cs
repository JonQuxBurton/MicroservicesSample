using Customers.Data;
using Infrastructure.Events;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Customers.PhoneLineOrderCompletedSubscriber.Tests
{
    public class Subscriber_Should
    {
        [Fact]
        public void SaveEvent()
        {
            var expectedPhoneLineOrderCompleted = new {
                phoneLineId = 202,
                status = "Complete",
                phoneNumber = "01140001111"
            };

            var expectedEvent = new Event(101, new DateTimeOffset(), "name", expectedPhoneLineOrderCompleted);
            var events = new List<Event>() { expectedEvent };
            var serializedEvents = JsonConvert.SerializeObject(events.AsEnumerable());

            var customerDataStoreMock = new Mock<ICustomerDataStore>();
            var restClientMock = new Mock<IEventGetter>();
            restClientMock.Setup(
                x => x.Get("PhoneLineOrdersCompleted", 0, 100))
                    .Returns(new RestResponse()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = serializedEvents
                    });

            var sut = new Subscriber(restClientMock.Object, customerDataStoreMock.Object);

            sut.Poll(null, new EventArgs());

            customerDataStoreMock.Verify(
                x => x.CompletePhoneLine(expectedPhoneLineOrderCompleted.phoneLineId, 
                    It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
