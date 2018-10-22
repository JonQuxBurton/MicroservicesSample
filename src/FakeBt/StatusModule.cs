using FakeBt.Data;
using Microsoft.Extensions.Logging;
using Nancy;
using System;

namespace FakeBt
{
    public class StatusModule : NancyModule
    {
        private readonly ILogger logger;

        public StatusModule(IBtOrdersDataStore btOrdersDataStore, ILoggerFactory loggerFactory) : base("/status")
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
                        Database = GetDatabaseConnectivity(btOrdersDataStore),
                    }
                };
            });
        }

        private bool GetDatabaseConnectivity(IBtOrdersDataStore customerStore)
        {
            try
            {
                customerStore.GetNew();
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
            }

            return false;
        }

    }
}
