using System;
using Customers.Config;
using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System.Net;
using Customers.Data;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Polly;
using Polly.Retry;

namespace Customers
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;

        public CustomBootstrapper(IApplicationBuilder applicationBuilder)
        {
            this.applicationBuilder = applicationBuilder;
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IOptions<AppSettings> options = this.applicationBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();

            Console.WriteLine("AppSettings:");
            Console.WriteLine($"ConnectionString: {options.Value.ConnectionString}");
            Console.WriteLine($"EventStoreUrl: {options.Value.EventStoreUrl}");

            //Console.WriteLine($"EventStoreIpAddress: {addresses[0]}");

            //var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse(options.Value.EventStoreIpAddress), int.Parse(options.Value.EventStorePort)));
            //var connection = EventStoreConnection.Create(options.Value.EventStoreUrl);

            IEventStoreConnection connection = null;

            RetryPolicy retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(80)
                });

            retry.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses("eventstore");
                connection = EventStoreConnection.Create(new IPEndPoint(addresses[0], 1113));
                connection.ConnectAsync().Wait(10 * 1000);
            });
            
            container.Register(options);

            container.Register(connection);
            container.Register<IEventStore, Infrastructure.Events.EventStore>().AsSingleton();
            container.Register<IEventDataCreator, EventDataCreator>().AsSingleton();
            container.Register<IEventStreamReader, EventStreamReader>();
            container.Register<Infrastructure.Serialization.ISerializer, JsonSerializer>().AsSingleton();
            container.Register<IDeserializer, JsonDeserializer>().AsSingleton();
            container.Register<IGuidCreator, GuidCreator>();
        }
    }
}
