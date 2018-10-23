using System;
using System.Net;
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

namespace Customers
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly ILogger<CustomBootstrapper> logger;
        private readonly ILoggerFactory loggerFactory;

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
            this.logger.LogInformation($"EventStoreHostName: {options.Value.EventStoreHostName}");

            IEventStoreConnection connection = null;

            var isEventStoreServerUpCheckPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(20),
                    (exception, timeSpan, context) =>
                    {
                        this.logger.LogInformation($"IsEventStoreServerUpCheck exception: {exception}");
                    }
                );

            isEventStoreServerUpCheckPolicy.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses(options.Value.EventStoreHostName);
                this.logger.LogInformation($"EventStoreIpAddress: {addresses[0]}");
                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream,
                    System.Net.Sockets.ProtocolType.Tcp);
                sock.Connect(addresses[0], options.Value.EventStorePort);
                sock.Close();
                this.logger.LogInformation($"EventStore Server is listening on port {options.Value.EventStorePort}");
            });

            var isEventStoreUpCheckPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(30),
                    (exception, timeSpan, context) =>
                    {
                        this.logger.LogInformation($"IsEventStoreUpCheck exception: {exception}");
                    }
                );

            isEventStoreUpCheckPolicy.Execute(() =>
            {
                var addresses = System.Net.Dns.GetHostAddresses(options.Value.EventStoreHostName);
                this.logger.LogInformation($"EventStoreIpAddress: {addresses[0]}");
                connection = EventStoreConnection.Create(new IPEndPoint(addresses[0], options.Value.EventStorePort));

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