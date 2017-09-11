using System;

namespace Infrastructure.Events
{
    public class EventMetaData
    {
        public DateTimeOffset OccurredAt { get; set; }
        public string EventName { get; set; }
    }
}
