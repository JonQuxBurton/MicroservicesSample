using FakeBt.Data;
using FakeBt.OrderUpdater.Configuration;
using Infrastructure.Rest;
using Infrastructure.Timers;
using Polly;
using System;

namespace FakeBt.OrderUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigGetter()
            {
                PhoneLineOrdererUrl = "http://localhost:5002"
            };

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                     TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

            var restPosterFactory = new RestPosterFactory(exponentialRetryPolicy);
            var btOrdersDataStore = new BtOrdersDataStore();
            var phoneLineOrderUpdater = new PhoneLineOrderUpdater(btOrdersDataStore, config, restPosterFactory);

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += phoneLineOrderUpdater.Update;
            recurringTimer.Start();

            Console.WriteLine("FakeBt.OrderUpdater is running.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
