using System;

namespace Customers.Entities
{
    public class PhoneLineOrder
    {
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public int Reference { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
