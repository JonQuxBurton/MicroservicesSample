using FakeBt.Config;
using FakeBt.Data;
using Infrastructure.Rest;
using Microsoft.Extensions.Options;
using System;

namespace FakeBt.OrderUpdater
{
    public class PhoneLineOrderUpdater
    {
        private readonly IBtOrdersDataStore btOrdersDataStore;
        private readonly IRestPosterFactory restPosterFactory;
        private readonly string phoneLineOrdererUrl;

        public PhoneLineOrderUpdater(IBtOrdersDataStore btOrdersDataStore, IOptions<AppSettings> appSettings, IRestPosterFactory restPosterFactory)
        {
            this.btOrdersDataStore = btOrdersDataStore;
            this.restPosterFactory = restPosterFactory;
            this.phoneLineOrdererUrl = appSettings.Value.PhoneLineOrdererWebServiceUrl;
        }

        public async void Update(object sender, EventArgs eventArgs)
        {
            var restPoster = this.restPosterFactory.Create(phoneLineOrdererUrl);

            var pendingOrders = this.btOrdersDataStore.GetNew();

            foreach (var pendingOrder in pendingOrders)
            {
                pendingOrder.PhoneNumber = $"0114{pendingOrder.Id.ToString().PadLeft(7, '0')}";

                var response = await restPoster.Post("PhoneLineOrderCompleted", new
                {
                    Reference = pendingOrder.Reference,
                    Status = "Complete",
                    PhoneNumber = pendingOrder.PhoneNumber
                });
                
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    this.btOrdersDataStore.Complete(pendingOrder);
                }
                else
                {
                    Console.WriteLine("Fail: " + response?.StatusCode);
                    this.btOrdersDataStore.Fail(pendingOrder.Id);
                }
            }
        }
    }
}
