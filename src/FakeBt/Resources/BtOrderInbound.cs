using System;

namespace FakeBt.Resources
{
    public class BtOrderInbound
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int HouseNumber { get; set; }
        public string Postcode { get; set; }
        public Guid Reference { get; set; }
    }
}
