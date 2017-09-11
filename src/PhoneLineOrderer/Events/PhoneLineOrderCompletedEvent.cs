namespace PhoneLineOrderer.Events
{
    public class PhoneLineOrderCompletedEvent
    {
        public int? PhoneLineId { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
    }
}
