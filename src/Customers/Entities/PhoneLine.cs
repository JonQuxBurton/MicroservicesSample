using System.Collections.Generic;

namespace Customers.Entities
{
    public class PhoneLine
    {
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public List<PhoneLineOrder> PhoneLineOrders { get; set; }
    }
}
