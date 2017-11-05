using Customers.Config;
using Customers.Data;
using Infrastructure.Events;
using Infrastructure.Rest;
using Infrastructure.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.IO;

namespace Customers.PhoneLineOrderCompletedSubscriber
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

            var restGetterFactory = new RestGetterFactory();
            var restGetter = restGetterFactory.Create(appSettings.PhoneLineOrdererServiceUrl);
            var eventGetter = new EventGetter(restGetter);
            var subscriber = new Subscriber(eventGetter, new CustomerDataStore(options));

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Console.WriteLine("Customers.PhoneLineOrderCompletedSubscriber is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}