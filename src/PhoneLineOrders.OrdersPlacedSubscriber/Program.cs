﻿using Infrastructure.DateTimeUtilities;
using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Infrastructure.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PhoneLineOrderer.Config;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using Polly;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PhoneLineOrderer.OrdersPlacedSubscriber
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
            var restGetter = restGetterFactory.Create(appSettings.CustomersWebServiceUrl);
            var eventGetter = new EventGetter(restGetter);
            var orderSender = new PhoneLineOrderSender(new PhoneLineOrdererDataStore(options), 
                options, 
                new RestPosterFactory(exponentialRetryPolicy),
                new DateTimeOffsetCreator());
            var subscriber = new Subscriber(eventGetter, orderSender, new GuidCreator(), loggerFactory);

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Log.Logger.Information("PhoneLineOrderer.OrdersPlacedSubscriber is running.");

            await hostBuilder.RunConsoleAsync();
        }
    }
}