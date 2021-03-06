﻿using Infrastructure.DateTimeUtilities;
using Infrastructure.Events;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using Infrastructure.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using SmsSender.Config;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SmsSender.Subscribers
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            var services = new ServiceCollection()
                .AddLogging(x =>
                {
                    x.AddSerilog();
                });

            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            var hostBuilder = new HostBuilder();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var appSettings = configuration.Get<AppSettings>();
            var options = Options.Create(appSettings);

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                     TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

            var restGetterFactory = new RestGetterFactory();
            var restGetter = restGetterFactory.Create(appSettings.CustomersMicroserviceUrl);
            var eventGetter = new EventGetter(restGetter);
            var client = new RestClient(appSettings.CustomersMicroserviceUrl);
            var webServiceGetter = new WebServiceGetter(client, exponentialRetryPolicy);
            
            var orderPlacedSmsSender = new OrderPlacedSmsSender(
                new Data.SmsSenderDataStore(options), 
                webServiceGetter,
                new JsonDeserializer(),
                new DateTimeOffsetCreator(),
                loggerFactory);

            var phoneLineOrdersPlacedSubscriber = new PhoneLineOrdersPlacedSubscriber(eventGetter, orderPlacedSmsSender);
            var phoneLineOrdersPlacedSubscriberTimer = new RecurringTimer(500, 5000);
            phoneLineOrdersPlacedSubscriberTimer.Target += phoneLineOrdersPlacedSubscriber.Poll;
            phoneLineOrdersPlacedSubscriberTimer.Start();

            var phoneLineOrdererRestGetter = restGetterFactory.Create(appSettings.CustomersMicroserviceUrl);
            var phoneLineOrdersCompletedEventGetter = new EventGetter(phoneLineOrdererRestGetter);

            var phoneLineOrdersCompletedSmsSender = new OrderCompletedSmsSender(
                new Data.SmsSenderDataStore(options),
                new WebServiceGetter(client, exponentialRetryPolicy),
                new JsonDeserializer(),
                new DateTimeOffsetCreator());

            var phoneLineOrdersCompletedSubscriber = new PhoneLineOrdersCompletedSubscriber(phoneLineOrdersCompletedEventGetter, phoneLineOrdersCompletedSmsSender);
            var phoneLineOrdersCompletedSubscriberTimer = new RecurringTimer(500, 5000);
            phoneLineOrdersCompletedSubscriberTimer.Target += phoneLineOrdersCompletedSubscriber.Poll;
            phoneLineOrdersCompletedSubscriberTimer.Start();

            Log.Logger.Information("SmsSender.Subscribers is running.");

            await hostBuilder.RunConsoleAsync();
        }
    }
}
