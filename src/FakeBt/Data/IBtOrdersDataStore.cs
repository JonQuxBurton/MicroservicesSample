using FakeBt.Resources;
using System.Collections.Generic;

namespace FakeBt.Data
{
    public interface IBtOrdersDataStore
    {
        void Receive(BtOrderInbound phoneLineOrder);
        IEnumerable<BtOrderInbound> GetNew();
        void Complete(BtOrderInbound phoneLineOrder);
        void Fail(int id);
    }
}
