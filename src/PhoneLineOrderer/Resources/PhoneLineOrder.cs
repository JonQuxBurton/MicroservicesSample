using System;

namespace PhoneLineOrderer.Resources
{
    public class PhoneLineOrder
    {
        public int? PhoneLineId { get; set; }
        public int HouseNumber { get; set; }
        public string Postcode { get; set; }
        public Guid Reference { get; set; }
    }
}
