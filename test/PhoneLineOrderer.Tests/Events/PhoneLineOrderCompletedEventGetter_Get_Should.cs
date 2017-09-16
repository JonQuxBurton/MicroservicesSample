using Infrastructure.Events;
using Microsoft.Extensions.Options;
using Moq;
using PhoneLineOrderer.Config;
using PhoneLineOrderer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhoneLineOrderer.Tests.Events
{
    public class PhoneLineOrderCompletedEventGetter_Get_Should
    {
        [Fact]
        public void GetEvents()
        {
            var expected = new List<Event>() {
                new Event(101, new DateTimeOffset(new DateTime(2001, 5, 4)), "name", "content")
            }.AsEnumerable();

            var appSettings = new AppSettings
            {
                PhoneLineOrdersCompletedStream = "PhoneLineOrdersCompletedStream"
            };
            var options = Options.Create(appSettings);

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(x => x.GetEvents<PhoneLineOrderCompletedEvent>(appSettings.PhoneLineOrdersCompletedStream, 0, 100))
                .Returns(expected);

            var sut = new PhoneLineOrderCompletedEventGetter(eventStoreMock.Object, options);

            var actual = sut.GetEvents(0, 100);

            Assert.Equal(expected, actual);
        }
    }
}
