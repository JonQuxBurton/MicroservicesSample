using Customers.Entities;
using Customers.Resources;

namespace Customers.Data
{
    public interface ICustomerDataStore
    {
        Customer GetById(int id);
        void Add(string name);
        int AddPhoneLine(int customerId, Resources.PhoneLineOrder phoneLineOrder);
        void CompletePhoneLine(int phoneLineId, string status, string phoneNumber);
    }
}
