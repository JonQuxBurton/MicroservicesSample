CREATE DATABASE Microservices;
GO
USE Microservices;
GO

CREATE SCHEMA [Customers]
GO
CREATE SCHEMA [PhoneLineOrderer]
GO
CREATE SCHEMA [FakeBt]
GO
CREATE SCHEMA [SmsSender]
GO

CREATE LOGIN CustomersMicroservice WITH PASSWORD = 'Customers@123';
GO  
CREATE USER CustomersMicroservice FOR LOGIN CustomersMicroservice
GO
CREATE LOGIN PhoneLineOrdererMicroservice WITH PASSWORD = 'PhoneLineOrderer@456';
GO
CREATE USER PhoneLineOrdererMicroservice FOR LOGIN PhoneLineOrdererMicroservice
GO
CREATE LOGIN FakeBtMicroservice WITH PASSWORD = 'FakeBt@789';  
GO
CREATE USER FakeBtMicroservice FOR LOGIN FakeBtMicroservice
GO
CREATE LOGIN SmsSenderMicroservice WITH PASSWORD = 'SmsSender@012';  
GO
CREATE USER SmsSenderMicroservice FOR LOGIN SmsSenderMicroservice
GO

CREATE TABLE [Customers].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] [nchar](100) NOT NULL,
	[MobilePhoneNumber] [nvarchar](50) NULL
)
GO

CREATE TABLE [Customers].[PhoneLines](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PhoneNumber] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[HouseNumber] [int] NOT NULL,
	[Postcode] [nvarchar](50) NOT NULL
)
GO

CREATE TABLE [PhoneLineOrderer].[PhoneLineOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PhoneLineId] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[HouseNumber] [int] NULL,
	[Postcode] [nvarchar](50) NULL,
	[ExternalReference] [uniqueidentifier] NOT NULL,
	[PhoneNumber] [nvarchar](50) NULL
)
GO

CREATE TABLE [FakeBt].[PhoneLineOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PhoneNumber] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[HouseNumber] [int] NOT NULL,
	[Postcode] [nvarchar](50) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL
)
GO

CREATE TABLE [SmsSender].[Sent](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerId] [int] NULL,
	[MobilePhoneNumber] [nvarchar](50) NULL,
	[Message] [nvarchar](100) NULL,
	[SentAt] [datetime] NOT NULL
)
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: Customers TO CustomersMicroservice;
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: PhoneLineOrderer TO PhoneLineOrdererMicroservice;
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: FakeBt TO FakeBtMicroservice;
GO
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA :: SmsSender TO SmsSenderMicroservice;
GO

