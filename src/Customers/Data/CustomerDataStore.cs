using System.Linq;
using Dapper;
using System.Data.SqlClient;
using Customers.Entities;
using Microsoft.Extensions.Options;
using Customers.Config;
using System.Collections.Generic;

namespace Customers.Data
{
    public class CustomerDataStore : ICustomerDataStore
    {
        private string connectionString;

        private const string databaseName = "Microservices";
        private const string SchemaName = "Customers";
        private const string CustomersTableName = "Customers";
        private const string PhoneLinesTableName = "PhoneLines";

        public CustomerDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }

        public void CreateTables()
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
                 AND  TABLE_NAME = '{CustomersTableName}'))
BEGIN
    CREATE TABLE [{SchemaName}].[{CustomersTableName}] (
    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] [nchar](100) NOT NULL,
    [MobilePhoneNumber] [nvarchar](50) NULL
    )
END
                    ".Replace("{SchemaName}", SchemaName)
                        .Replace("{CustomersTableName}", CustomersTableName);

                    conn.Execute(sql, 
                        new {Dummy="Dummy"},
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
                 AND  TABLE_NAME = '{PhoneLinesTableName}'))
BEGIN
    CREATE TABLE [{SchemaName}].[{PhoneLinesTableName}](
	    [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	    [PhoneNumber] [nvarchar](50) NULL,
	    [Status] [nvarchar](50) NOT NULL,
	    [CustomerId] [int] NOT NULL,
	    [HouseNumber] [int] NOT NULL,
	    [Postcode] [nvarchar](50) NOT NULL
    )
END
                    ".Replace("{SchemaName}", SchemaName)
                        .Replace("{PhoneLinesTableName}", PhoneLinesTableName);

                    conn.Execute(sql, 
                        new {Dummy="Dummy"},
                        tx);
                    tx.Commit();
                }
            }
        }

        public Customer GetById(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbCustomers = conn.Query($"select * from {SchemaName}.{CustomersTableName} where Id=@Id", new { Id = id });
                var dbCustomer = dbCustomers.FirstOrDefault();

                if (dbCustomer == null)
                    return null;

                return new Customer { Id = dbCustomer.Id, Name = dbCustomer.Name };
            }
        }

        public void Add(string name, string mobilePhoneNumber)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open(); 
                using (var tx = conn.BeginTransaction())
                {
                    conn.Execute($"insert into {SchemaName}.{CustomersTableName}(Name, MobilePhoneNumber) values (@Name, @mobilePhoneNumber)", new { Name = name, MobilePhoneNumber = mobilePhoneNumber }, tx);
                    tx.Commit();
                }
            }
        }

        public int AddPhoneLine(int customerId, Resources.PhoneLineOrder phoneLineOrder)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var status = "Pending";
                    var sql = $"insert into {SchemaName}.{PhoneLinesTableName}(CustomerId, Status, HouseNumber, Postcode) values (@CustomerId, @Status, @HouseNumber, @Postcode); select cast(scope_identity() as int);";

                    var lastInsertedId = connection.Query<int>(sql, new { CustomerId = customerId, Status = status, HouseNumber = phoneLineOrder.HouseNumber, Postcode = phoneLineOrder.Postcode }, transaction).Single();

                    transaction.Commit();

                    return lastInsertedId;
                }
            }
        }

        public void CompletePhoneLine(int phoneLineId, string status, string phoneNumber)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var sql = $"update {SchemaName}.{PhoneLinesTableName} set Status=@Status, PhoneNumber=@PhoneNumber where Id=@PhoneLineId";

                    connection.Execute(sql, new { PhoneLineId = phoneLineId, Status = status, PhoneNumber = phoneNumber }, transaction);

                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Customer> GetByPhoneLineId(int phoneLineId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbCustomers = conn.Query($"select c.* from {SchemaName}.{CustomersTableName} c join Customers.PhoneLines pl on c.Id=pl.CustomerId where pl.Id=@PhoneLineId", new { PhoneLineId = phoneLineId });

                return dbCustomers.Select(
                    x => new Customer {
                        Id = x.Id,
                        Name = x.Name,
                        MobilePhoneNumber = x.MobilePhoneNumber
                    });
            }
        }
    }
}
