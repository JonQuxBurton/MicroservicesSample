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
using PhoneLineOrderer.Data;
using Polly.Retry;

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
            RetryPolicy retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(80)
                });

            IEventStoreConnection connection = null;

            retry.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses("eventstore");
                connection = EventStoreConnection.Create(new IPEndPoint(addresses[0], 1113));
                connection.ConnectAsync().Wait(10 * 1000);
            });

            //var connection = EventStoreConnection.Create(new IPEndPoint(addresses[0], 1113));

            //var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            //connection.ConnectAsync().Wait();

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

            var dataStore = new PhoneLineOrdererDataStore(options);

            retry.Execute(() => dataStore.SetupDatabase());
        }
    }
}
