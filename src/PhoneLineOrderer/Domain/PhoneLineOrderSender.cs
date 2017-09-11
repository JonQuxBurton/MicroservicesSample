using Infrastructure.Rest;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Entities;
using System;

namespace PhoneLineOrderer.Domain
{
    public class PhoneLineOrderSender : IPhoneLineOrderSender
    {
        private IPhoneLineOrdererDataStore phoneLineOrdersDataStore;
        private readonly IRestPosterFactory restPosterFactory;
        private readonly IConfigGetter config;

        public PhoneLineOrderSender(IPhoneLineOrdererDataStore phoneLineOrdersDataStore, IConfigGetter config, IRestPosterFactory restPosterFactory)
        {
            this.phoneLineOrdersDataStore = phoneLineOrdersDataStore;
            this.restPosterFactory = restPosterFactory;
            this.config = config;
        }

        public bool Send(Resources.PhoneLineOrder phoneLineOrder)
        {
            var phoneLineOrderData = new PhoneLineOrder
            {
                PhoneLineId = phoneLineOrder.PhoneLineId,
                CreatedAt = DateTime.Now,
                Status = "New",
                HouseNumber = phoneLineOrder.HouseNumber,
                Postcode = phoneLineOrder.Postcode,
                ExternalReference = phoneLineOrder.Reference
            };

            var id = this.phoneLineOrdersDataStore.Add(phoneLineOrderData);

            var restPoster = this.restPosterFactory.Create(this.config.FakeBtWebServiceUrl);

            var response = restPoster.Post("PhoneLineOrders", new
            {
                HouseNumber = phoneLineOrder.HouseNumber,
                Postcode = phoneLineOrder.Postcode,
                Reference = phoneLineOrder.Reference
            });

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                this.phoneLineOrdersDataStore.Sent(id);
            }
            else
            {
                this.phoneLineOrdersDataStore.Failed(id);
            }

            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
