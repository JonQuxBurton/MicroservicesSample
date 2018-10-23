namespace Customers.Config
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string PhoneLineOrdersPlacedStream { get; set; }
        public string PhoneLineOrdererServiceUrl { get; set; }
        public string EventStoreHostName { get; set; }
        public int EventStorePort { get; set; }
    }
}
