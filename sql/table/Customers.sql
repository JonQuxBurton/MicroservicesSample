CREATE SCHEMA [Customers]

CREATE TABLE [Customers].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] [nchar](100) NOT NULL,
	[MobilePhoneNumber] [nvarchar](50) NULL
)
GO