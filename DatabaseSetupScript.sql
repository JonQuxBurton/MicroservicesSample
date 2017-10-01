CREATE SCHEMA [Customers]
CREATE SCHEMA [PhoneLineOrderer]
CREATE SCHEMA [FakeBt]
CREATE SCHEMA [SmsSender]

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