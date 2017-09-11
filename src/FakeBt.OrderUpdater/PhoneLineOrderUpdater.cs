using FakeBt.Data;
using FakeBt.OrderUpdater.Configuration;
using Infrastructure.Rest;
using System;

namespace FakeBt.OrderUpdater
{
    public class PhoneLineOrderUpdater
    {
        private readonly IBtOrdersDataStore btOrdersDataStore;
        private readonly IRestPosterFactory restPosterFactory;
        private readonly IConfigGetter config;

        public PhoneLineOrderUpdater(IBtOrdersDataStore btOrdersDataStore, IConfigGetter config, IRestPosterFactory restPosterFactory)
        {
            this.btOrdersDataStore = btOrdersDataStore;
            this.restPosterFactory = restPosterFactory;
            this.config = config;
        }

        public void Update(object sender, EventArgs eventArgs)
        {
            var restPoster = this.restPosterFactory.Create(this.config.PhoneLineOrdererUrl);

            var pendingOrders = this.btOrdersDataStore.GetNew();

            foreach (var pendingOrder in pendingOrders)
            {
                pendingOrder.PhoneNumber = $"0114{pendingOrder.Id.ToString().PadLeft(7, '0')}";

                var response = restPoster.Post("PhoneLineOrderCompleted", new
                {
                    Reference = pendingOrder.Reference,
                    Status = "Complete",
                    PhoneNumber = pendingOrder.PhoneNumber
                });
                
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    this.btOrdersDataStore.Complete(pendingOrder);
                }
                else
                {
                    this.btOrdersDataStore.Fail(pendingOrder.Id);
                }
            }
        }
    }
}
