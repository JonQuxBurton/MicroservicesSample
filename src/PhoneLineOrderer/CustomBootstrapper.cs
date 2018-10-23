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
using Microsoft.Extensions.Logging;
using Polly.Retry;

namespace PhoneLineOrderer
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

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                    TimeSpan.FromMilliseconds(100 * Math.Pow(2,
                                                  attempt)));
            container.Register(connection);
            container.Register<IOptions<AppSettings>>(options);
            container.Register<ISyncPolicy>(exponentialRetryPolicy);
            container.Register<IEventStore, Infrastructure.Events.EventStore>().AsSingleton();
            container.Register<IEventDataCreator, EventDataCreator>().AsSingleton();
            container.Register<IEventStreamReader, EventStreamReader>();
            container.Register<IRestPosterFactory, RestPosterFactory>();
            container.Register<Infrastructure.Serialization.ISerializer, JsonSerializer>().AsSingleton();
            container.Register<IDeserializer, JsonDeserializer>().AsSingleton();
            container.Register<IGuidCreator, GuidCreator>();
            container.Register<IDateTimeOffsetCreator, DateTimeOffsetCreator>();
            container.Register(loggerFactory);
        }
    }
}
