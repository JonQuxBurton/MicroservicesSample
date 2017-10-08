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

        public CustomerDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }

        public Customer GetById(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbCustomers = conn.Query(@"select * from Customers.Customers where Id=@Id", new { Id = id });
                var dbCustomer = dbCustomers.FirstOrDefault();

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
                    conn.Execute("insert into Customers.Customers(Name, MobilePhoneNumber) values (@Name, @mobilePhoneNumber)", new { Name = name, MobilePhoneNumber = mobilePhoneNumber }, tx);
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
                    var sql = "insert into Customers.PhoneLines(CustomerId, Status, HouseNumber, Postcode) values (@CustomerId, @Status, @HouseNumber, @Postcode); select cast(scope_identity() as int);";

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
                    var sql = "update Customers.PhoneLines set Status=@Status, PhoneNumber=@PhoneNumber where Id=@PhoneLineId";

                    connection.Execute(sql, new { PhoneLineId = phoneLineId, Status = status, PhoneNumber = phoneNumber }, transaction);

                    transaction.Commit();
                }
            }
        }

        public IEnumerable<Customer> GetByPhoneLineId(int phoneLineId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbCustomers = conn.Query(@"select c.* from Customers.Customers c join Customers.PhoneLines pl on c.Id=pl.CustomerId where pl.Id=@PhoneLineId", new { PhoneLineId = phoneLineId });

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
