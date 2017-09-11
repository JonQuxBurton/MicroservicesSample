using Customers.Data;
using Infrastructure.Events;
using Infrastructure.Timers;
using RestSharp;
using System;

namespace Customers.PhoneLineOrderCompletedSubscriber
{
    class Program
    {
        private static string PhoneLineOrdererServiceUrl = "http://localhost:5002/";

        static void Main(string[] args)
        {
            var client = new RestClient(PhoneLineOrdererServiceUrl);
            var eventGetter = new EventGetter(client);
            var subscriber = new Subscriber(eventGetter, new CustomerDataStore());

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += subscriber.Poll;
            recurringTimer.Start();

            Console.WriteLine("Customers.PhoneLineOrderCompletedSubscriber is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}