using Dapper;
using Microsoft.Extensions.Options;
using PhoneLineOrderer.Config;
using PhoneLineOrderer.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace PhoneLineOrderer.Data
{
    public class PhoneLineOrdererDataStore : IPhoneLineOrdererDataStore
    {
        private string connectionString;

        public PhoneLineOrdererDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }

        public IEnumerable<PhoneLineOrder> GetByPhoneLineId(int phoneLineId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query(@"select * from PhoneLineOrderer.PhoneLineOrders where PhoneLineId=@phoneLineId order by CreatedAt desc", new { phoneLineId = phoneLineId });

                foreach (var dbOrder in dbOrders)
                {
                    yield return new PhoneLineOrder { Id = dbOrder.Id };
                }
            }
        }

        public PhoneLineOrder GetByReference(Guid reference)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrder = conn.Query(@"select * from PhoneLineOrderer.PhoneLineOrders where ExternalReference=@reference order by CreatedAt desc", new { reference = reference }).FirstOrDefault();

                if (dbOrder == null)
                    return null;

                return new PhoneLineOrder
                {
                    PhoneLineId = dbOrder.PhoneLineId
                };
            }
        }
        
        public int Add(PhoneLineOrder phoneLineOrder)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {                    
                    var sql = @"insert into PhoneLineOrderer.PhoneLineOrders(PhoneLineId, CreatedAt, Status, HouseNumber, Postcode, ExternalReference) values (@PhoneLineId, @CreatedAt, @Status, @HouseNumber, @Postcode, @ExternalReference); select cast(scope_identity() as int);";
                    var lastInsertedId = connection.Query<int>(sql, new { PhoneLineId = phoneLineOrder.PhoneLineId, CreatedAt = phoneLineOrder.CreatedAt, Status = phoneLineOrder.Status, HouseNumber = phoneLineOrder.HouseNumber, Postcode = phoneLineOrder.Postcode, ExternalReference = phoneLineOrder.ExternalReference }, transaction).Single();

                    transaction.Commit();

                    return lastInsertedId;
                }
            }
        }

        public void Sent(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var sql = @"update PhoneLineOrderer.PhoneLineOrders set Status=@Status where Id=@Id";
                    connection.Execute(sql, new { Id = id, Status = "Sent" }, transaction);
                    transaction.Commit();
                }
            }
        }

        public void Failed(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var sql = @"update PhoneLineOrderer.PhoneLineOrders set Status=@Status where Id=@Id";
                    connection.Execute(sql, new { Id = id, Status = "Failed" }, transaction);
                    transaction.Commit();
                }
            }
        }

        public void Receive(Resources.PhoneLineOrderCompleted phoneLineOrderReceived)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    conn.Execute("update PhoneLineOrderer.PhoneLineOrders set Status=@Status, PhoneNumber=@PhoneNumber where ExternalReference=@ExternalReference", new { Status = phoneLineOrderReceived.Status, ExternalReference = phoneLineOrderReceived.Reference, PhoneNumber = phoneLineOrderReceived.PhoneNumber }, tx);

                    tx.Commit();
                }
            }
        }
    }
}
