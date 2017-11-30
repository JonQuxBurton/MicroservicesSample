using Infrastructure.DateTimeUtilities;
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
using RestSharp;
using System;
using System.IO;

namespace PhoneLineOrderer.OrdersPlacedSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

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
            var subscriber = new Subscriber(eventGetter, orderSender, new GuidCreator());

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Console.WriteLine("PhoneLineOrderer.OrdersPlacedSubscriber is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}