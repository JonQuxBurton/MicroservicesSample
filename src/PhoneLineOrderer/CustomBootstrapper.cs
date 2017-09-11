using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Polly;
using RestSharp;
using System;
using System.Net;

namespace PhoneLineOrderer
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();

            container.Register(connection);

            var config = new ConfigGetter()
            {
                FakeBtWebServiceUrl = "http://localhost:5003"
            };

            container.Register<IConfigGetter>(config);

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));
            container.Register<ISyncPolicy>(exponentialRetryPolicy);

            container.Register<IEventStore, Infrastructure.Events.EventStore>().AsSingleton();
            container.Register<IEventDataCreator, EventDataCreator>().AsSingleton();
            container.Register<IEventStreamReader, EventStreamReader>();
            container.Register<IRestPosterFactory, RestPosterFactory>();
            container.Register<Infrastructure.Serialization.ISerializer, JsonSerializer>().AsSingleton();
            container.Register<IDeserializer, JsonDeserializer>().AsSingleton();
            container.Register<IGuidCreator, GuidCreator>();
        }
    }
}
