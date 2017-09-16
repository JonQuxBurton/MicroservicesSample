using Customers.Config;
using Customers.Events;
using EventStore.ClientAPI;
using Infrastructure.Events;
using Microsoft.Extensions.Options;
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

            var appSettings = new AppSettings
            {
                PhoneLineOrdersPlacedStream = "PhoneLineOrdersPlacedStream"
            };
            var options = Options.Create(appSettings);

            var sut = new PhoneLineOrdersPlacedEventPublisher(eventDataCreatorMock.Object, eventStoreMock.Object, options);

            sut.Publish(dummyPhoneLineOrderPlaced);

            eventStoreMock.Verify(x =>
                x.Raise(appSettings.PhoneLineOrdersPlacedStream, expectedEventData), Times.Once);
        }
    }
}
