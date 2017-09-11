﻿using Nancy;
using PhoneLineOrderer.Events;

namespace PhoneLineOrderer
{
    public class PhoneLineOrdersCompletedModule : NancyModule
    {
        public PhoneLineOrdersCompletedModule(IPhoneLineOrderCompletedEventGetter phoneLineOrderCompletedEventGetter) : base("/PhoneLineOrdersCompleted")
        {
            Get("", x =>
            {
                long firstEventSequenceNumber, lastEventSequenceNumber;

                if (!long.TryParse(this.Request.Query.start.Value, out firstEventSequenceNumber))
                    firstEventSequenceNumber = 0;

                if (!long.TryParse(this.Request.Query.end.Value, out lastEventSequenceNumber))
                    lastEventSequenceNumber = 100;

                return phoneLineOrderCompletedEventGetter.GetEvents(firstEventSequenceNumber, lastEventSequenceNumber);
            });
        }
    }
}
