using Infrastructure.Rest;
using Microsoft.Extensions.Options;
using PhoneLineOrderer.Config;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Entities;
using System;

namespace PhoneLineOrderer.Domain
{
    public class PhoneLineOrderSender : IPhoneLineOrderSender
    {
        private IPhoneLineOrdererDataStore phoneLineOrdersDataStore;
        private readonly IRestPosterFactory restPosterFactory;
        private readonly string fakeBtWebServiceUrl;

        public PhoneLineOrderSender(IPhoneLineOrdererDataStore phoneLineOrdersDataStore, IOptions<AppSettings> appSettings, IRestPosterFactory restPosterFactory)
        {
            this.phoneLineOrdersDataStore = phoneLineOrdersDataStore;
            this.restPosterFactory = restPosterFactory;
            this.fakeBtWebServiceUrl = appSettings.Value?.FakeBtWebServiceUrl;
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

            var restPoster = this.restPosterFactory.Create(this.fakeBtWebServiceUrl);

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
