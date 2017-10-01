using System.Collections.Generic;

namespace Customers.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobilePhoneNumber { get; set; }
        public IEnumerable<PhoneLine> PhoneLine { get; set; }
    }
}
