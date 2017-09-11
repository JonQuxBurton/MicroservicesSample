using Nancy;
using Nancy.ModelBinding;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Domain;

namespace PhoneLineOrderer
{
    public class PhoneLineOrdersModule : NancyModule
    {
        public PhoneLineOrdersModule(IPhoneLineOrdererDataStore phoneLineOrdersDataStore, IPhoneLineOrderSender orderSender)
        {
            Get("/Status", x => {
                return HttpStatusCode.OK;
            });
            Get("/PhoneLineOrders/{phoneLineId}", 
                x => phoneLineOrdersDataStore.GetByPhoneLineId(x.phoneLineId)
                );
            Post("/PhoneLineOrders", x => {
                var phoneLineOrder = this.Bind<Resources.PhoneLineOrder>();

                orderSender.Send(phoneLineOrder);
                
                return HttpStatusCode.Accepted;
            });
        }
    }
}
