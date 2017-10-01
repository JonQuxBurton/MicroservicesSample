using Infrastructure.Rest;
using Infrastructure.Serialization;
using Microsoft.Extensions.Options;
using SmsSender.Config;
using SmsSender.Data;
using System;

namespace SmsSender
{
    public class OrderPlacedSmsSender : IOrderPlacedSmsSender
    {
        public const string TextMessage = "Thank you for your ordering a phone line.";

        private readonly ISmsSenderDataStore smsSenderDataStore;
        private readonly IWebServiceGetter webServiceGetter;
        private readonly IDeserializer deserializer;
        private readonly string customersMicroserviceUrl;

        public OrderPlacedSmsSender(ISmsSenderDataStore smsSenderDataStore, 
            IWebServiceGetter webServiceGetter,
            IOptions<AppSettings> appSettings,
            IDeserializer deserializer)
        {
            this.smsSenderDataStore = smsSenderDataStore;
            this.webServiceGetter = webServiceGetter;
            this.deserializer = deserializer;
            customersMicroserviceUrl = appSettings.Value.CustomersMicroserviceUrl;
        }

        public bool Send(int phoneLineId)
        {            
            var response = this.webServiceGetter.Get($"/customers/phonelines/{phoneLineId}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var customer = this.deserializer.Deserialize<Customer>(response.Content);

            if (customer != null)
                this.smsSenderDataStore.Send(customer, OrderPlacedSmsSender.TextMessage, new DateTimeOffset(DateTime.Now));

            return true;
        }
    }
}
