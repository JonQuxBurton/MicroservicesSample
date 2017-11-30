using Nancy;
using PhoneLineOrderer.Events;
using System;
using System.Linq;

namespace PhoneLineOrderer
{
    public class PhoneLineOrdersCompletedModule : NancyModule
    {
        public PhoneLineOrdersCompletedModule(IPhoneLineOrderCompletedEventGetter phoneLineOrderCompletedEventGetter) : base("/PhoneLineOrdersCompleted")
        {
            Get("", x =>
            {
                if (!long.TryParse(this.Request.Query.start.Value, out long firstEventSequenceNumber))
                    firstEventSequenceNumber = 0;

                if (!long.TryParse(this.Request.Query.end.Value, out long lastEventSequenceNumber))
                    lastEventSequenceNumber = 100;

                try
                { 
                    return phoneLineOrderCompletedEventGetter.GetEvents(firstEventSequenceNumber, lastEventSequenceNumber).ToList();
                }
                catch (Exception)
                {
                    return HttpStatusCode.InternalServerError;
                }
            });
        }
    }
}
