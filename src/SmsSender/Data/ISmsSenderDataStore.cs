using System;

namespace SmsSender.Data
{
    public interface ISmsSenderDataStore
    {
        void Send(Customer customer, string message, DateTimeOffset sentAt);
    }
}