using Customers.Config;
using Customers.Events;
using Infrastructure.Events;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Customers.Tests.Events
{
    public class PhoneLineOrdersPlacedEventGetter_Should
    {
        [Fact]
        public void GetEvents()
        {
            var expected = new List<Event>() {
                new Event(101, new DateTimeOffset(new DateTime(2001, 5, 4)), "name", "content")
            }.AsEnumerable();

            var appSettings = new AppSettings
            {
                PhoneLineOrdersPlacedStream = "PhoneLineOrdersPlacedStream"
            };
            var options = Options.Create(appSettings);


            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(x => x.GetEvents<PhoneLineOrderPlaced>(appSettings.PhoneLineOrdersPlacedStream, 0, 100))
                .Returns(expected);

            var sut = new PhoneLineOrdersPlacedEventGetter(eventStoreMock.Object, options);

            var actual = sut.GetEvents(0, 100);

            Assert.Equal(expected, actual);
        }
    }
}
