using Customers.Events;
using EventStore.ClientAPI;
using Infrastructure.Events;
using Moq;
using System;
using Xunit;

namespace Customers.Tests.Events
{
    public class PhoneLineOrdersPlacedEventPublisher_Publish_Should
    {
        [Fact]
        public void PublishEvent()
        {
            var expectedEventData = new EventData(Guid.NewGuid(), "PhoneLineOrderPlaced", true, new byte[0], new byte[0]);
            var dummyPhoneLineOrderPlaced = new PhoneLineOrderPlaced();

            var eventDataCreatorMock = new Mock<IEventDataCreator>();
            var eventStoreMock = new Mock<IEventStore>();
            eventDataCreatorMock.Setup(x => x.Create("phoneLineOrderPlaced", dummyPhoneLineOrderPlaced)).Returns(expectedEventData);

            var sut = new PhoneLineOrdersPlacedEventPublisher(eventDataCreatorMock.Object, eventStoreMock.Object);

            sut.Publish(dummyPhoneLineOrderPlaced);

            eventStoreMock.Verify(x =>
                x.Raise(PhoneLineOrdersPlacedEventPublisher.PhoneLineOrdersPlacedStream, expectedEventData), Times.Once);
        }
    }
}
