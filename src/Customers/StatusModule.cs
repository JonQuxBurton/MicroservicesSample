using Customers.Config;
using Customers.Data;
using Customers.Events;
using Microsoft.Extensions.Options;
using Nancy;
using System;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Customers
{
    public class StatusModule : NancyModule
    {
        private readonly ILogger logger;

        public StatusModule(ICustomerDataStore customerStore, 
            IOptions<AppSettings> appSettings, 
            IPhoneLineOrdersPlacedEventGetter phoneLineOrdersPlacedEventGetter,
            ILoggerFactory loggerFactory) : base("/status")
        {
            this.logger = loggerFactory.CreateLogger<StatusModule>();

            Get("", x => {
                return HttpStatusCode.OK;
            });
            base.Get("/deep", x =>
            {
                return new
                {
                    Connectivity = new
                    {
                        Database = GetDatabaseConnectivity(customerStore),
                        PhoneLineOrdererService = GetPhoneLineOrdererServiceConnectivity(appSettings),
                        PhoneLineOrdersPlacedStream = GetPhoneLineOrdersPlacedStreamConnectivity(phoneLineOrdersPlacedEventGetter)
                    }
                };
            });
        }

        private bool GetPhoneLineOrdersPlacedStreamConnectivity(IPhoneLineOrdersPlacedEventGetter phoneLineOrdersPlacedEventGetter)
        {
            try
            {
                phoneLineOrdersPlacedEventGetter.GetEvents(1, 2).ToList();
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
            }

            return false;
        }

        private bool GetPhoneLineOrdererServiceConnectivity(IOptions<AppSettings> appSettings)
        {
            var url = new Url(appSettings.Value.PhoneLineOrdererServiceUrl);
            TcpClient client = null;

            try
            {
                client = new TcpClient(url.HostName, url.Port.GetValueOrDefault());
                return true;
            }
            catch
            { }
            finally
            {
                if (client != null)
                    client.Close();
            }

            return false;
        }

        private bool GetDatabaseConnectivity(ICustomerDataStore customerStore)
        {
            try
            {
                customerStore.GetById(0);
                return true;
            }
            catch (Exception)
            { }

            return false;
        }
    }
}