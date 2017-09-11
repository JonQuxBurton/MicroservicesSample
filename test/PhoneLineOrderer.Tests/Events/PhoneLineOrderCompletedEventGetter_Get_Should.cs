using Infrastructure.Events;
using Moq;
using PhoneLineOrderer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var eventStoreMock = new Mock<IEventStore>();
            eventStoreMock.Setup(x => x.GetEvents<PhoneLineOrderCompletedEvent>(PhoneLineOrderCompletedEventGetter.PhoneLineOrdersCompletedStream, 0, 100))
                .Returns(expected);

            var sut = new PhoneLineOrderCompletedEventGetter(eventStoreMock.Object);

            var actual = sut.GetEvents(0, 100);

            Assert.Equal(expected, actual);
        }
    }
}
