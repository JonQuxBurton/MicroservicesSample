using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nancy;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Events;

namespace PhoneLineOrderer
{
    public class StatusModule : NancyModule
    {
        private readonly ILogger logger;

        public StatusModule(IPhoneLineOrdererDataStore phoneLineOrdererDataStore,
            IPhoneLineOrderCompletedEventGetter phoneLineOrderCompletedEventGetter,
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
                        Database = GetDatabaseConnectivity(phoneLineOrdererDataStore),

                        PhoneLineOrdersCompletedStream = PhoneLineOrdersCompletedStreamConnectivity(phoneLineOrderCompletedEventGetter)
                    }
                };
            });
        }

        private bool PhoneLineOrdersCompletedStreamConnectivity(IPhoneLineOrderCompletedEventGetter phoneLineOrderCompletedEventGetter)
        {
            try
            {
                phoneLineOrderCompletedEventGetter.GetEvents(1, 2).ToList();
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
            }

            return false;
        }

        private bool GetDatabaseConnectivity(IPhoneLineOrdererDataStore phoneLineOrdererDataStore)
        {
            try
            {
                phoneLineOrdererDataStore.GetByPhoneLineId(0);
                return true;
            }
            catch (Exception)
            { }

            return false;
        }
    }
}
