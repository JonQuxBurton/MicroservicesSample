using EventStore.ClientAPI;
using Infrastructure.Guid;
using Infrastructure.Serialization;
using System;
using System.Text;

namespace Infrastructure.Events
{
    public class EventDataCreator : IEventDataCreator
    {
        private ISerializer jsonSerializer;
        private readonly IGuidCreator guidCreator;

        public EventDataCreator(ISerializer jsonSerializer, IGuidCreator guidCreator)
        {
            this.jsonSerializer = jsonSerializer;
            this.guidCreator = guidCreator;
        }

        public EventData Create(string eventName, object content)
        {
            var contentJson = this.jsonSerializer.Serialize(content);
            var metaDataJson = this.jsonSerializer.Serialize(new EventMetaData
            {
                OccurredAt = DateTimeOffset.Now,
                EventName = eventName,
            });

            return new EventData(guidCreator.Create(), eventName, isJson: true, data: Encoding.UTF8.GetBytes(contentJson),
                metadata: Encoding.UTF8.GetBytes(metaDataJson));
        }
    }
}
