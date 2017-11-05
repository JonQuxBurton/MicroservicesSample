using Customers.Events;
using Infrastructure.Events;
using Moq;
using Nancy.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Customers.Tests
{
    public class PhoneLineOrdersPlacedModule_Get_Should
    {
        private Mock<IPhoneLineOrdersPlacedEventGetter> phoneLineOrdersPlacedEventStore;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrdersPlacedModule_Get_Should()
        {
            phoneLineOrdersPlacedEventStore = new Mock<IPhoneLineOrdersPlacedEventGetter>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(phoneLineOrdersPlacedEventStore.Object);
                with.Module<PhoneLineOrdersPlacedModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void Return_PhoneLineOrderPlaced()
        {
            var expectedEvent = new Event(10, new DateTimeOffset(new DateTime(2001, 5, 4)), "Name", "Content");

            var expectedEvents = new List<Event>
            {
                expectedEvent
            };

            phoneLineOrdersPlacedEventStore.Setup(x => x.GetEvents(0, 100)).Returns(expectedEvents.AsEnumerable());

            var response = browser.Get($"/PhoneLineOrdersPlaced", with =>
            {
                with.HttpRequest();
                with.Query("start", "0");
                with.Query("end", "100");
            });

            Assert.Equal(Nancy.HttpStatusCode.OK, response.Result.StatusCode);
            var responseBody = response.Result.Body;

            var actual = JsonConvert.DeserializeObject<IEnumerable<Event>>(responseBody.AsString());

            Assert.NotNull(actual);
            Assert.Equal(expectedEvents.Count(), actual.Count());
            Assert.Equal(expectedEvents.First().Name, actual.First().Name);
            Assert.Equal(expectedEvents.First().SequenceNumber, actual.First().SequenceNumber);
            Assert.Equal(expectedEvents.First().OccurredAt, actual.First().OccurredAt);
            Assert.Equal(expectedEvents.First().Content, actual.First().Content);
        }

        [Fact]
        public void Return_InternalServerErrorWhenEventGetFails()
        {
            phoneLineOrdersPlacedEventStore.Setup(x => x.GetEvents(0, 100)).Throws<Exception>();

            var response = browser.Get($"/PhoneLineOrdersPlaced", with =>
            {
                with.HttpRequest();
                with.Query("start", "0");
                with.Query("end", "100");
            });

            Assert.Equal(Nancy.HttpStatusCode.InternalServerError, response.Result.StatusCode);
        }
    }
}
