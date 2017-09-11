using EventStore.ClientAPI;
using Infrastructure.Events;
using Moq;
using PhoneLineOrderer.Events;
using System;
using Xunit;

namespace PhoneLineOrderer.Tests.Events
{
    public class PhoneLineOrderCompletedEventPublisher_Publish_Should
    {
        [Fact]
        public void PublishEvent()
        {
            var expectedEventData = new EventData(Guid.NewGuid(), "PhoneLineOrderCompletedEvent", true, new byte[0], new byte[0]);
            var dummyPhoneLineOrderCompletedEvent = new PhoneLineOrderCompletedEvent();

            var eventDataCreatorMock = new Mock<IEventDataCreator>();
            var eventStoreMock = new Mock<IEventStore>();
            eventDataCreatorMock.Setup(x => x.Create("phoneLineOrderCompletedEvent", dummyPhoneLineOrderCompletedEvent)).Returns(expectedEventData);

            var sut = new PhoneLineOrderCompletedEventPublisher(eventDataCreatorMock.Object, eventStoreMock.Object);

            sut.Publish(dummyPhoneLineOrderCompletedEvent);

            eventStoreMock.Verify(x =>
                x.Raise(PhoneLineOrderCompletedEventPublisher.PhoneLineOrdersCompletedStream, expectedEventData), Times.Once);
        }
    }
}
