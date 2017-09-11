using EventStore.ClientAPI;

namespace Infrastructure.Events
{
    public interface IEventDataCreator
    {
        EventData Create(string eventName, object content);
    }
}
