using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using PhoneLineOrderer.Config;
using Polly;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.DateTimeUtilities;

namespace PhoneLineOrderer
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;

        public CustomBootstrapper(IApplicationBuilder applicationBuilder)
        {
            this.applicationBuilder = applicationBuilder;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();

            container.Register(connection);

            IOptions<AppSettings> options = this.applicationBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();
            container.Register<IOptions<AppSettings>>(options);

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
            container.Register<IDateTimeOffsetCreator, DateTimeOffsetCreator>();
        }
    }
}
