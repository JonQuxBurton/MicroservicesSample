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

        private const string databaseName = "Microservices";
        private const string SchemaName = "PhoneLineOrderer";
        private const string PhoneLineOrdersTableName = "PhoneLineOrders";

        public PhoneLineOrdererDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }

        public void SetupDatabase()
        {
            using (var conn = new SqlConnection(connectionString.Replace("Initial Catalog=Microservices;", "")))
            {
                conn.Open();

                var sql = @"
if db_id('{databaseName}') is null
BEGIN
    CREATE DATABASE [{databaseName}]
END

".Replace("{databaseName}", databaseName);
                conn.Execute(sql);
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    var sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{SchemaName}')
BEGIN
    EXEC( 'CREATE SCHEMA {SchemaName}' );
END
".Replace("{SchemaName}", SchemaName);

                    conn.Execute(sql,
                        new { Dummy = "Dummy" },
                        tx);
                    tx.Commit();
                }
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    var sql = @"
IF NOT (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = '{SchemaName}' 
                 AND  TABLE_NAME = '{PhoneLineOrdersTableName}'))
BEGIN
    CREATE TABLE [{SchemaName}].[{PhoneLineOrdersTableName}] (
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PhoneLineId] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[HouseNumber] [int] NULL,
	[Postcode] [nvarchar](50) NULL,
	[ExternalReference] [uniqueidentifier] NOT NULL,
	[PhoneNumber] [nvarchar](50) NULL
)
END
                    ".Replace("{SchemaName}", SchemaName)
                        .Replace("{PhoneLineOrdersTableName}", PhoneLineOrdersTableName);

                    conn.Execute(sql,
                        new { Dummy = "Dummy" },
                        tx);
                    tx.Commit();
                }
            }
        }

        public IEnumerable<PhoneLineOrder> GetByPhoneLineId(int phoneLineId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbOrders = conn.Query($"select * from {SchemaName}.{PhoneLineOrdersTableName} where PhoneLineId=@phoneLineId order by CreatedAt desc", new { phoneLineId = phoneLineId });

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
                var dbOrder = conn.Query($"select * from {SchemaName}.{PhoneLineOrdersTableName} where ExternalReference=@reference order by CreatedAt desc", new { reference = reference }).FirstOrDefault();

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
            Console.WriteLine("Add()...");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        var sql = $"insert into {SchemaName}.{PhoneLineOrdersTableName}(PhoneLineId, CreatedAt, Status, HouseNumber, Postcode, ExternalReference) values (@PhoneLineId, @CreatedAt, @Status, @HouseNumber, @Postcode, @ExternalReference); select cast(scope_identity() as int);";
                        var lastInsertedId = connection.Query<int>(sql, new { PhoneLineId = phoneLineOrder.PhoneLineId, CreatedAt = phoneLineOrder.CreatedAt, Status = phoneLineOrder.Status, HouseNumber = phoneLineOrder.HouseNumber, Postcode = phoneLineOrder.Postcode, ExternalReference = phoneLineOrder.ExternalReference }, transaction).Single();

                        transaction.Commit();

                        return lastInsertedId;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Sent(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var sql = $"update {SchemaName}.{PhoneLineOrdersTableName} set Status=@Status where Id=@Id";
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
                    var sql = $"update {SchemaName}.{PhoneLineOrdersTableName} set Status=@Status where Id=@Id";
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
                    conn.Execute($"update {SchemaName}.{PhoneLineOrdersTableName} set Status=@Status, PhoneNumber=@PhoneNumber where ExternalReference=@ExternalReference", new { Status = phoneLineOrderReceived.Status, ExternalReference = phoneLineOrderReceived.Reference, PhoneNumber = phoneLineOrderReceived.PhoneNumber }, tx);

                    tx.Commit();
                }
            }
        }
    }
}
