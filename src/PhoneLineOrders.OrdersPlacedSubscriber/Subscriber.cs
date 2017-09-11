using Infrastructure.Events;
using Infrastructure.Guid;
using Infrastructure.Rest;
using Newtonsoft.Json;
using PhoneLineOrderer.Domain;
using System;
using System.Collections.Generic;

namespace PhoneLineOrderer.OrdersPlacedSubscriber
{
    public class Subscriber
    {
        private long start = 0;
        private int chunkSize = 100;
        private readonly IEventGetter eventGetter;
        private readonly IPhoneLineOrderSender phoneLineOrderSender;
        private readonly IRestPoster restPoster;
        private readonly IGuidCreator guidCreator;
        private readonly string url = "PhoneLineOrdersPlaced";

        public Subscriber(IEventGetter eventGetter, 
            IPhoneLineOrderSender phoneLineOrderSender, 
            IRestPoster restPoster, 
            IGuidCreator guidCreator)
        {
            this.eventGetter = eventGetter;
            this.phoneLineOrderSender = phoneLineOrderSender;
            this.restPoster = restPoster;
            this.guidCreator = guidCreator;
        }

        public void Poll(object sender, EventArgs eventArgs)
        {
            var response = eventGetter.Get(url, this.start, this.chunkSize);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                HandleEvents(response.Content);
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
