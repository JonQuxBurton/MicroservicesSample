Microservices sample
====================


A small sample of a Microservices architecture based on the book [Microservices in .NET Core by Christian Horsdal Gammelgaard](https://www.manning.com/books/microservices-in-net-core)

(Warning: This code is not suitable for Production use)


Tech
----
WebApi: [NancyFx](https://github.com/NancyFx/Nancy)

Data access: [Dapper](https://github.com/StackExchange/Dapper)

Event-based collaboration: [EventStore](https://github.com/EventStore/EventStore)

Robutness: [Polly](https://github.com/App-vNext/Polly)

Unit tests: [Moq](https://github.com/Moq/moq4/wiki/Quickstart), [xUnit](https://xunit.github.io/)


Domain
------
Modelled on a simple Telecoms domain of:

Customers (Customers microservice)

...ordering PhoneLines (PhoneLineOrderer microservice)

...from a Wholesaler (FakeBt microservice)


Data
----
Database creation script (T-SQL):
DatabaseSetupScript.sql


Launching
---------
The Launch.ps1 Powershell script will launch the system (six projects).

Will also need to install and then launch EventStore:
EventStore.ClusterNode --db ./db --log ./logs


Usage
-----

#### Create a Customer:

POST http://localhost:5001/customers

Content-Type: application/json

Body:
{
	"Name" : "Apollo Ltd"
}

#### Order a PhoneLine:

POST http://localhost:5001/customer/1/phonelines

Content-Type: application/json

Body:
{
	"HouseNumber" : 101,
	"Postcode": "S1 Z01"
}
