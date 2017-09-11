using PhoneLineOrderer.Entities;
using System;
using System.Collections.Generic;

namespace PhoneLineOrderer.Data
{
    public interface IPhoneLineOrdererDataStore
    {
        IEnumerable<PhoneLineOrder> GetByPhoneLineId(int phoneLineId);
        int Add(PhoneLineOrder phoneLineOrder);
        void Sent(int id);
        void Failed(int id);
        void Receive(Resources.PhoneLineOrderCompleted phoneLineOrderReceived);
        PhoneLineOrder GetByReference(Guid reference);
    }
}
