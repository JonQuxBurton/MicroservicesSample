using Nancy;
using Nancy.ModelBinding;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Events;
using System;

namespace PhoneLineOrderer
{
    public class PhoneLineOrderCompletedModule : NancyModule
    {
        public PhoneLineOrderCompletedModule(IPhoneLineOrdererDataStore phoneLineOrdererDataStore, IPhoneLineOrderCompletedEventPublisher orderCompletedEventPublisher)
        {
            Post("/PhoneLineOrderCompleted", x => {

                try
                {
                    var phoneLineOrderCompleted = this.Bind<Resources.PhoneLineOrderCompleted>();

                    var phoneLineOrder = phoneLineOrdererDataStore.GetByReference(phoneLineOrderCompleted.Reference);

                    orderCompletedEventPublisher.Publish(new PhoneLineOrderCompletedEvent
                    {
                        PhoneLineId = phoneLineOrder.PhoneLineId,
                        Status = phoneLineOrderCompleted.Status,
                        PhoneNumber = phoneLineOrderCompleted.PhoneNumber
                    });

                    phoneLineOrdererDataStore.Receive(phoneLineOrderCompleted);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    return HttpStatusCode.InternalServerError;
                }
                return HttpStatusCode.OK;
            });
        }
    }
}
