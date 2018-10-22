using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Customers.Config;
using EventStore.ClientAPI;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.TinyIoc;
using Polly;
using Polly.Retry;

namespace Customers
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<CustomBootstrapper> logger;

        public CustomBootstrapper(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory)
        {
            this.applicationBuilder = applicationBuilder;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<CustomBootstrapper>();
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            IOptions<AppSettings> options =
                this.applicationBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();

            this.logger.LogInformation("AppSettings");
            this.logger.LogInformation($"ConnectionString: {options.Value.ConnectionString}");
            this.logger.LogInformation($"EventStoreUrl: {options.Value.EventStoreUrl}");
            
            //var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Parse(options.Value.EventStoreIpAddress), int.Parse(options.Value.EventStorePort)));
            //var connection = EventStoreConnection.Create(options.Value.EventStoreUrl);

            IEventStoreConnection connection = null;

            RetryPolicy retry = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(40),
                    TimeSpan.FromSeconds(80),
                    TimeSpan.FromSeconds(80)
                });

            retry.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses("eventstore");
                this.logger.LogInformation($"EventStoreIpAddress: {addresses[0]}");
                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                sock.Connect(addresses[0], 1113);
                sock.Close();
                this.logger.LogInformation($"EventStore is listening on port 1113");
            });

            retry.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses("eventstore");
                this.logger.LogInformation($"EventStoreIpAddress: {addresses[0]}");
                connection = EventStoreConnection.Create(new IPEndPoint(addresses[0], 1113));

                connection.ConnectAsync().Wait(20 * 1000);
                this.logger.LogInformation("connection.ConnectAsync - wait completed");
            });

            container.Register(options);

            container.Register(connection);
            container.Register<IEventStore, Infrastructure.Events.EventStore>().AsSingleton();
            container.Register<IEventDataCreator, EventDataCreator>().AsSingleton();
            container.Register<IEventStreamReader, EventStreamReader>();
            container.Register<Infrastructure.Serialization.ISerializer, JsonSerializer>().AsSingleton();
            container.Register<IDeserializer, JsonDeserializer>().AsSingleton();
            container.Register<IGuidCreator, GuidCreator>();
            container.Register(this.loggerFactory);
        }
    }
}