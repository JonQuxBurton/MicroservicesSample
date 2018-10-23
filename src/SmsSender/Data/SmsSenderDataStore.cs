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

        private const string databaseName = "Microservices";
        private const string SchemaName = "SmsSender";
        private const string SentTableName = "Sent";

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
                    conn.Execute($"insert into {SchemaName}.{SentTableName}(CustomerId, MobilePhoneNumber, Message, SentAt) values (@CustomerId, @MobilePhoneNumber, @Message, @SentAt)", 
                        new { CustomerId = customer.Id, MobilePhoneNumber= customer.MobilePhoneNumber, Message = message, SentAt = sentAt}, tx);
                    tx.Commit();
                }
            }
        }
    }
}
