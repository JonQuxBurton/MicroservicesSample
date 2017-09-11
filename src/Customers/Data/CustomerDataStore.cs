﻿using System.Linq;
using Dapper;
using System.Data.SqlClient;
using Customers.Entities;
using Customers.Resources;

namespace Customers.Data
{
    public class CustomerDataStore : ICustomerDataStore
    {
        private string connectionString = @"Server=SQLEXPRESS;Initial Catalog=Microservices;Integrated Security=True";

        public Customer GetById(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var dbCustomers = conn.Query(@"select * from Customers.Customers where Id=@Id", new { Id = id });
                var dbCustomer = dbCustomers.FirstOrDefault();

                return new Customer { Id = dbCustomer.Id, Name = dbCustomer.Name };
            }
        }

        public void Add(string name)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open(); 
                using (var tx = conn.BeginTransaction())
                {
                    conn.Execute("insert into Customers.Customers(Name) values (@Name)", new { Name = name }, tx);
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
    }
}
