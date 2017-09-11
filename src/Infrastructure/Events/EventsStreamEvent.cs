namespace Infrastructure.Events
{
    public class EventsStreamEvent
    {
        public byte[] Data { get; set; }
        public byte[] Metadata { get; set; }
        public long OriginalEventNumber { get; set; }
    }
}
