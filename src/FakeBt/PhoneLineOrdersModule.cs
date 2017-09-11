using FakeBt.Data;
using FakeBt.Resources;
using Nancy;
using Nancy.ModelBinding;

namespace FakeBt
{
    public class PhoneLineOrdersModule : NancyModule
    {
        public PhoneLineOrdersModule(IBtOrdersDataStore btOrdersStore)
        {
            Get("/Status", x => {
                return HttpStatusCode.OK;
            });
            Post("/PhoneLineOrders", x => {
                var phoneLineOrder = this.Bind<BtOrderInbound>();

                btOrdersStore.Receive(phoneLineOrder);
                
                return HttpStatusCode.Accepted;
            });
        }
    }
}
