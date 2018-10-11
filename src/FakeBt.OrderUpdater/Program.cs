using FakeBt.Config;
using FakeBt.Data;
using Infrastructure.Rest;
using Infrastructure.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FakeBt.OrderUpdater
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();

            var appSettings = configuration.Get<AppSettings>();
            var options = Options.Create(appSettings);

            Policy exponentialRetryPolicy =
                Policy.Handle<Exception>().WaitAndRetry(3, attempt =>
                     TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

            var restPosterFactory = new RestPosterFactory(exponentialRetryPolicy);
            var btOrdersDataStore = new BtOrdersDataStore(options);
            var phoneLineOrderUpdater = new PhoneLineOrderUpdater(btOrdersDataStore, options, restPosterFactory);

            var recurringTimer = new RecurringTimer(500, 5000);
            recurringTimer.Target += phoneLineOrderUpdater.Update;
            recurringTimer.Start();

            Console.WriteLine("FakeBt.OrderUpdater is running.");

            await hostBuilder.RunConsoleAsync();
        }
    }
}
