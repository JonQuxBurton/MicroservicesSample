using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Infrastructure.Timers;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;
using Polly;
using RestSharp;
using System;

namespace PhoneLineOrderer.OrdersPlacedSubscriber
{
    class Program
    {
        private static string CustomersServiceUrl = "http://localhost:5001/";
        private static string FakeBtWebServiceUrl = "http://localhost:5003/";

        static void Main(string[] args)
        {
            var config = new ConfigGetter()
            {
                FakeBtWebServiceUrl = "http://localhost:5003"
            };

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                     TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

            var client = new RestClient(CustomersServiceUrl);
            var eventGetter = new EventGetter(client);
            var restPoster = new RestPoster(new RestClient(FakeBtWebServiceUrl), exponentialRetryPolicy);
            var orderSender = new PhoneLineOrderSender(new PhoneLineOrdererDataStore(), config, new RestPosterFactory(exponentialRetryPolicy));
            var subscriber = new Subscriber(eventGetter, orderSender, restPoster, new GuidCreator());

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Console.WriteLine("PhoneLineOrderer.OrdersPlacedSubscriber is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}