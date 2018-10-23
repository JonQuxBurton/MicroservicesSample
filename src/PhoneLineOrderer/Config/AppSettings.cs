namespace PhoneLineOrderer.Config
{
    public class AppSettings
    {
        public string FakeBtWebServiceUrl { get; set; }
        public string ConnectionString { get; set; }
        public string PhoneLineOrdersCompletedStream { get; set; }
        public string CustomersWebServiceUrl { get; set; }
        public string EventStoreHostName { get; set; }
        public int EventStorePort { get; set; }
    }
}
