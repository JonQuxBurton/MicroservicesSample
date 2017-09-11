using System;

namespace PhoneLineOrderer.Resources
{
    public class PhoneLineOrderCompleted
    {
        public Guid Reference { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
    }
}
