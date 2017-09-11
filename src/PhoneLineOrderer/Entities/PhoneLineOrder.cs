using System;

namespace PhoneLineOrderer.Entities
{
    public class PhoneLineOrder
    {
        public int Id { get; set; }
        public int? PhoneLineId { get; set; }
        public Guid ExternalReference { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HouseNumber { get; set; }
        public string Postcode { get; set; }
    }
}
