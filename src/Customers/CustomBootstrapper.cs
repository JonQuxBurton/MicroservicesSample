using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Serialization;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System.Net;

namespace Customers
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();

            container.Register<IEventStoreConnection>(connection);
            container.Register<IEventStore, Infrastructure.Events.EventStore>().AsSingleton();
            container.Register<IEventDataCreator, EventDataCreator>().AsSingleton();
            container.Register<IEventStreamReader, EventStreamReader>();
            container.Register<Infrastructure.Serialization.ISerializer, JsonSerializer>().AsSingleton();
            container.Register<IDeserializer, JsonDeserializer>().AsSingleton();
            container.Register<IGuidCreator, GuidCreator>();
        }
    }
}
