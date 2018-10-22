using Customers.Data;
using Infrastructure.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Customers.PhoneLineOrderCompletedSubscriber
{
    public class Subscriber
    {
        private long start = 0;
        private readonly int chunkSize = 100;
        private readonly IEventGetter eventGetter;
        private readonly ICustomerDataStore customerDataStore;
        private readonly ILogger logger;
        private readonly string url = "PhoneLineOrdersCompleted";

        public Subscriber(IEventGetter eventGetter, ICustomerDataStore customerDataStore, ILoggerFactory loggerFactory)
        {
            this.eventGetter = eventGetter;
            this.customerDataStore = customerDataStore;
            this.logger = loggerFactory.CreateLogger<Subscriber>();
        }

        public void Poll(object sender, EventArgs eventArgs)
        {
            var response = eventGetter.Get(url, this.start, this.chunkSize);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                HandleEvents(response.Content);
            else
                this.logger.LogInformation($"Response:StatusCode: {response?.StatusCode}");
        }
        
        private void HandleEvents(string content)
        {
            var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(content);

            foreach (var ev in events)
            {
                dynamic eventData = ev.Content;

                this.customerDataStore.CompletePhoneLine((int)eventData.phoneLineId, (string)eventData.status, (string)eventData.phoneNumber);
                this.start = ev.SequenceNumber + 1;
            }
        }
    }
}
