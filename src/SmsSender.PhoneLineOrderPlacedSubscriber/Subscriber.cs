using Infrastructure.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmsSender.PhoneLineOrderPlacedSubscriber
{
    public class Subscriber
    {
        private long start = 0;
        private int chunkSize = 100;
        private readonly IEventGetter eventGetter;
        private readonly IOrderPlacedSmsSender orderPlacedSmsSender;
        private readonly string url = "PhoneLineOrdersPlaced";

        public Subscriber(IEventGetter eventGetter,
            IOrderPlacedSmsSender orderPlacedSmsSender)
        {
            this.eventGetter = eventGetter;
            this.orderPlacedSmsSender = orderPlacedSmsSender;
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

                this.orderPlacedSmsSender.Send((int)eventData.phoneLineId);

                this.start = ev.SequenceNumber + 1;
            }
        }
    }
}
