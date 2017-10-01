using Dapper;
using Microsoft.Extensions.Options;
using SmsSender.Config;
using System;
using System.Data.SqlClient;

namespace SmsSender.Data
{
    public class SmsSenderDataStore : ISmsSenderDataStore
    {
        private string connectionString;

        public SmsSenderDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }

        public void Send(Customer customer, string message, DateTimeOffset sentAt)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    conn.Execute("insert into SmsSender.Sent(CustomerId, MobilePhoneNumber, Message, SentAt) values (@CustomerId, @MobilePhoneNumber, @Message, @SentAt)", 
                        new { CustomerId = customer.Id, MobilePhoneNumber= customer.MobilePhoneNumber, Message = message, SentAt = sentAt}, tx);
                    tx.Commit();
                }
            }
        }
    }
}
