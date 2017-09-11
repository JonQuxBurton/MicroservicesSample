using Infrastructure.Events;
using Moq;
using Nancy.Testing;
using Newtonsoft.Json;
using PhoneLineOrderer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhoneLineOrderer.Tests
{
    public class PhoneLineOrdersCompletedModule_Get_Should
    {
        private Mock<IPhoneLineOrderCompletedEventGetter> phoneLineOrderCompletedEventGetterMock;
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public PhoneLineOrdersCompletedModule_Get_Should()
        {
            phoneLineOrderCompletedEventGetterMock = new Mock<IPhoneLineOrderCompletedEventGetter>();

            bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Dependency(phoneLineOrderCompletedEventGetterMock.Object);
                with.Module<PhoneLineOrdersCompletedModule>();
            });

            browser = new Browser(bootstrapper, x => x.Header("Accept", "application/json"));
        }

        [Fact]
        public void Return_PhoneLineOrderCompleted()
        {
            var expectedEvent = new Event(10, new DateTimeOffset(new DateTime(2001, 5, 4)), "Name", "Content");

            var expectedEvents = new List<Event>();
            expectedEvents.Add(expectedEvent);

            phoneLineOrderCompletedEventGetterMock.Setup(x => x.GetEvents(0, 100)).Returns(expectedEvents.AsEnumerable());

            var response = browser.Get("/PhoneLineOrdersCompleted", with =>
            {
                with.HttpRequest();
                with.Query("start", "0");
                with.Query("end", "100");
            });

            var responseBody = response.Result.Body;

            Assert.Equal(Nancy.HttpStatusCode.OK, response.Result.StatusCode);

            var actual = JsonConvert.DeserializeObject<IEnumerable<Event>>(responseBody.AsString());

            Assert.NotNull(actual);
            Assert.Equal(expectedEvents.Count(), actual.Count());
            Assert.Equal(expectedEvents.First().Name, actual.First().Name);
            Assert.Equal(expectedEvents.First().SequenceNumber, actual.First().SequenceNumber);
            Assert.Equal(expectedEvents.First().OccurredAt, actual.First().OccurredAt);
            Assert.Equal(expectedEvents.First().Content, actual.First().Content);
        }
    }
}
