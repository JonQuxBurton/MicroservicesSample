using Infrastructure.DateTimeUtilities;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using SmsSender.Data;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SmsSender
{
    public class OrderPlacedSmsSender : IOrderPlacedSmsSender
    {
        public const string TextMessage = "Thank you for your ordering a phone line.";

        private readonly ISmsSenderDataStore smsSenderDataStore;
        private readonly IWebServiceGetter webServiceGetter;
        private readonly IDeserializer deserializer;
        private readonly IDateTimeOffsetCreator dateTimeOffsetCreator;
        private readonly ILogger logger;

        public OrderPlacedSmsSender(ISmsSenderDataStore smsSenderDataStore, 
            IWebServiceGetter webServiceGetter,
            IDeserializer deserializer, 
            IDateTimeOffsetCreator dateTimeOffsetCreator,
            ILoggerFactory loggerFactory)
        {
            this.smsSenderDataStore = smsSenderDataStore;
            this.webServiceGetter = webServiceGetter;
            this.deserializer = deserializer;
            this.dateTimeOffsetCreator = dateTimeOffsetCreator;
            this.logger = loggerFactory.CreateLogger<OrderPlacedSmsSender>();
        }

        public async Task<bool> Send(int phoneLineId)
        {            
            var response = await this.webServiceGetter.Get($"/customers/phonelines/{phoneLineId}");

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.LogInformation($"response.StatuCode: {response?.StatusCode}");
                return false;
            }

            var customer = this.deserializer.Deserialize<Customer>(response.Content);

            if (customer != null)
                this.smsSenderDataStore.Send(customer, OrderPlacedSmsSender.TextMessage, this.dateTimeOffsetCreator.Now);

            return true;
        }
    }
}
