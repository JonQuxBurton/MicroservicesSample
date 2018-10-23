using Infrastructure.Events;
using Infrastructure.Guid;
using Newtonsoft.Json;
using PhoneLineOrderer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PhoneLineOrderer.OrdersPlacedSubscriber
{
    public class Subscriber
    {
        private long start = 0;
        private readonly int chunkSize = 100;
        private readonly IEventGetter eventGetter;
        private readonly IPhoneLineOrderSender phoneLineOrderSender;
        private readonly IGuidCreator guidCreator;
        private readonly ILogger logger;
        private readonly string url = "PhoneLineOrdersPlaced";

        public Subscriber(IEventGetter eventGetter, 
            IPhoneLineOrderSender phoneLineOrderSender, 
            IGuidCreator guidCreator,
            ILoggerFactory loggerFactory)
        {
            this.eventGetter = eventGetter;
            this.phoneLineOrderSender = phoneLineOrderSender;
            this.guidCreator = guidCreator;
            this.logger = loggerFactory.CreateLogger<Subscriber>();
        }

        public void Poll(object sender, EventArgs eventArgs)
        {
            try
            {
                var response = eventGetter.Get(url, this.start, this.chunkSize);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    HandleEvents(response.Content);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception: {ex}");
            }
        }

        private void HandleEvents(string content)
        {
            var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(content);

            foreach (var ev in events)
            {
                dynamic eventData = ev.Content;

                this.phoneLineOrderSender.Send(new Resources.PhoneLineOrder {
                    PhoneLineId = eventData.phoneLineId,
                    HouseNumber = eventData.houseNumber,
                    Postcode = eventData.postcode,
                    Reference = this.guidCreator.Create()
                });

                this.start = ev.SequenceNumber + 1;
            }
        }
    }
}
