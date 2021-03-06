﻿using System;
using Dapper;
using FakeBt.Config;
using FakeBt.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FakeBt.Data
{
    public class BtOrdersDataStore : IBtOrdersDataStore
    {
        private readonly string connectionString;

        private const string DatabaseName = "Microservices";
        private const string SchemaName = "FakeBt";
        private const string PhoneLineOrdersTableName = "PhoneLineOrders";

        public BtOrdersDataStore(IOptions<AppSettings> appSettings)
        {
            this.connectionString = appSettings.Value.ConnectionString;
        }
        
        public IEnumerable<BtOrderInbound> GetNew()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var pendingOrdersDb = conn.Query($"select * from {SchemaName}.{PhoneLineOrdersTableName} where Status='New'");

                foreach (var row in pendingOrdersDb)
                {
                    yield return new BtOrderInbound
                    {
                        Id = row.Id,
                        Reference = row.Reference,
                        PhoneNumber = row.PhoneNumber
                    };
                }
            }
        }

        public void Receive(BtOrderInbound phoneLineOrder)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    var data = new {
                        HouseNumber = phoneLineOrder.HouseNumber,
                        Postcode = phoneLineOrder.Postcode,
                        Reference = phoneLineOrder.Reference,
                        Status = "New"
                    };
                    conn.Execute($"insert into {SchemaName}.{PhoneLineOrdersTableName}(HouseNumber, Postcode, Reference, Status) values (@HouseNumber, @Postcode, @Reference, @Status)", data, tx);
                    tx.Commit();
                }
            }
        }

        public void Complete(BtOrderInbound phoneLineOrder)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    var data = new
                    {
                        Id = phoneLineOrder.Id,
                        PhoneNumber = phoneLineOrder.PhoneNumber,
                        Status = "Complete"
                    };
                    conn.Execute($"update {SchemaName}.{PhoneLineOrdersTableName} set Status=@Status, PhoneNumber=@PhoneNumber where Id=@Id", data, tx);
                    tx.Commit();
                }
            }
        }
        
        public void Fail(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    var data = new
                    {
                        Id = id,
                        Status = "Failed"
                    };
                    conn.Execute($"update {SchemaName}.{PhoneLineOrdersTableName} set Status=@Status where Id=@Id", data, tx);
                    tx.Commit();
                }
            }
        }
    }
}
