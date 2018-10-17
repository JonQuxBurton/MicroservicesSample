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
                 AND  TABLE_NAME = '{SentTableName}'))
BEGIN
CREATE TABLE [{SchemaName}].[{SentTableName}] (
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerId] [int] NULL,
	[MobilePhoneNumber] [nvarchar](50) NULL,
	[Message] [nvarchar](100) NULL,
	[SentAt] [datetime] NOT NULL
)
END
                    ".Replace("{SchemaName}", SchemaName)
                        .Replace("{SentTableName}", SentTableName);

                    conn.Execute(sql,
                        new { Dummy = "Dummy" },
                        tx);
                    tx.Commit();
                }
            }
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
