using Nancy;
using Nancy.ModelBinding;
using PhoneLineOrderer.Data;
using PhoneLineOrderer.Events;
using System;
using Microsoft.Extensions.Logging;

namespace PhoneLineOrderer
{
    public class PhoneLineOrderCompletedModule : NancyModule
    {
        private ILogger<PhoneLineOrderCompletedModule> logger;

        public PhoneLineOrderCompletedModule(IPhoneLineOrdererDataStore phoneLineOrdererDataStore, 
            IPhoneLineOrderCompletedEventPublisher orderCompletedEventPublisher, 
            ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<PhoneLineOrderCompletedModule>();

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
                    this.logger.LogError(ex.ToString());

                    return HttpStatusCode.InternalServerError;
                }
                return HttpStatusCode.OK;
            });
        }
    }
}
