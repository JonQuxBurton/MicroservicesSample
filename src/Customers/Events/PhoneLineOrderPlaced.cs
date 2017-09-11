namespace Customers.Events
{
    public class PhoneLineOrderPlaced
    {
        public int PhoneLineId { get; set; }
        public int HouseNumber { get; set; }
        public string Postcode { get; set; }
    }
}
