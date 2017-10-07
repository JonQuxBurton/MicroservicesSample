using Infrastructure.DateTimeUtilities;
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

namespace SmsSender.PhoneLineOrderPlacedSubscriber
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

            var client = new RestClient(appSettings.CustomersMicroserviceUrl);
            var eventGetter = new EventGetter(client);
            var webServiceGetter = new WebServiceGetter(client, exponentialRetryPolicy);
            
            var orderPlacedSmsSender = new OrderPlacedSmsSender(
                new SmsSender.Data.SmsSenderDataStore(options), 
                webServiceGetter,
                new JsonDeserializer(),
                new DateTimeOffsetCreator());

            var subscriber = new Subscriber(eventGetter, orderPlacedSmsSender);

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Console.WriteLine("SmsSender.PhoneLineOrderPlacedSubscriber is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
