using Customers.Entities;
using System.Collections.Generic;

namespace Customers.Data
{
    public interface ICustomerDataStore
    {
        Customer GetById(int id);
        void Add(string name, string mobilePhoneNumber);
        int AddPhoneLine(int customerId, Resources.PhoneLineOrder phoneLineOrder);
        void CompletePhoneLine(int phoneLineId, string status, string phoneNumber);
        IEnumerable<Customer> GetByPhoneLineId(int phoneLineId);
    }
}
