using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using RestSharp;
using Customers.Events;
using Customers.Data;
using Customers.Entities;
using System;

namespace Customers
{
    public class CustomerModule : NancyModule
    {
        public CustomerModule(ICustomerDataStore customerStore, IPhoneLineOrdersPlacedEventPublisher phoneLineOrdersPlacedEventPublisher) : base("/customer")
        {
            Get("/Status", x => {
                return HttpStatusCode.OK;
            });
            base.Post("/{id}/PhoneLines", x =>
            {
                var phoneLineOrder = this.Bind<Resources.PhoneLineOrder>();

                var phoneLineId = customerStore.AddPhoneLine(x.id, phoneLineOrder);

                phoneLineOrdersPlacedEventPublisher.Publish(new PhoneLineOrderPlaced
                {
                    PhoneLineId = phoneLineId,
                    HouseNumber = phoneLineOrder.HouseNumber,
                    Postcode = phoneLineOrder.Postcode
                });

                return HttpStatusCode.Accepted;
            });
        }
    }
}
