using Customers.Events;
using Nancy;

namespace Customers
{
    public class PhoneLineOrdersPlacedModule : NancyModule
    {
        public PhoneLineOrdersPlacedModule(IPhoneLineOrdersPlacedEventGetter phoneLineOrdersPlacedEventGetter) : base("/PhoneLineOrdersPlaced")
        {
            Get("", x =>
            {
                long firstEventSequenceNumber, lastEventSequenceNumber;

                if (!long.TryParse(this.Request.Query.start.Value, out firstEventSequenceNumber))
                    firstEventSequenceNumber = 0;

                if (!long.TryParse(this.Request.Query.end.Value, out lastEventSequenceNumber))
                    lastEventSequenceNumber = 100;

                return phoneLineOrdersPlacedEventGetter.GetEvents(firstEventSequenceNumber, lastEventSequenceNumber);
            });
        }
    }
}
