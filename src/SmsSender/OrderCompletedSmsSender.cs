using Infrastructure.DateTimeUtilities;
using Infrastructure.Rest;
using Infrastructure.Serialization;
using SmsSender.Data;

namespace SmsSender
{
    public class OrderCompletedSmsSender : IOrderCompletedSmsSender
    {
        public const string TextMessage = "Your phone line is now ready for use.";

        private readonly ISmsSenderDataStore smsSenderDataStore;
        private readonly IWebServiceGetter webServiceGetter;
        private readonly IDeserializer deserializer;
        private readonly IDateTimeOffsetCreator dateTimeOffsetCreator;

        public OrderCompletedSmsSender(ISmsSenderDataStore smsSenderDataStore,
            IWebServiceGetter webServiceGetter,
            IDeserializer deserializer,
            IDateTimeOffsetCreator dateTimeOffsetCreator)
        {
            this.smsSenderDataStore = smsSenderDataStore;
            this.webServiceGetter = webServiceGetter;
            this.deserializer = deserializer;
            this.dateTimeOffsetCreator = dateTimeOffsetCreator;
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
                this.smsSenderDataStore.Send(customer, OrderCompletedSmsSender.TextMessage, this.dateTimeOffsetCreator.Now);

            return true;
        }
    }
}
