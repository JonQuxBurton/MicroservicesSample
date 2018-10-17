﻿using Dapper;
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

        private const string databaseName = "Microservices";
        private const string SchemaName = "FakeBt";
        private const string PhoneLineOrdersTableName = "PhoneLineOrders";

        public BtOrdersDataStore(IOptions<AppSettings> appSettings)
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
                        new {Dummy = "Dummy"},
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
    [Name] [nchar](100) NOT NULL,
    [MobilePhoneNumber] [nvarchar](50) NULL
    )
END
                    ".Replace("{SchemaName}", SchemaName)
                        .Replace("{PhoneLineOrdersTableName}", PhoneLineOrdersTableName);

                    conn.Execute(sql,
                        new {Dummy = "Dummy"},
                        tx);
                    tx.Commit();
                }
            }
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
