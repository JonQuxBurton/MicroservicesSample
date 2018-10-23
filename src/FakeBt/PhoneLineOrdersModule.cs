using System;
using FakeBt.Data;
using FakeBt.Resources;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;

namespace FakeBt
{
    public class PhoneLineOrdersModule : NancyModule
    {
        private readonly ILogger logger;

        public PhoneLineOrdersModule(IBtOrdersDataStore btOrdersStore, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<PhoneLineOrdersModule>();

            Get("/Status", x => {
                return HttpStatusCode.OK;
            });
            Post("/PhoneLineOrders", x =>
            {

                try
                {
                    var phoneLineOrder = this.Bind<BtOrderInbound>();

                    btOrdersStore.Receive(phoneLineOrder);

                    return HttpStatusCode.Accepted;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex.ToString());
                    throw;
                }
            });
        }
    }
}
