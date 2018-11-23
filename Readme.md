Microservices sample
====================


A small sample of a Microservices architecture inspired by the book [Microservices in .NET Core by Christian Horsdal Gammelgaard](https://www.manning.com/books/microservices-in-net-core)

(Warning: This code is not suitable for Production use)


Tech
----
WebApi: [NancyFx](https://github.com/NancyFx/Nancy)
Data access: [Dapper](https://github.com/StackExchange/Dapper)
Event-based collaboration: [EventStore](https://github.com/EventStore/EventStore)
Robustness: [Polly](https://github.com/App-vNext/Polly)
Unit tests: [Moq](https://github.com/Moq/moq4/wiki/Quickstart), [xUnit](https://xunit.github.io/)
Containers: [Docker](https://www.docker.com/)

Domain
------
Modelled on a simple Telecoms domain of:

Customers (Customers Microservice)

...ordering PhoneLines (PhoneLineOrderer Microservice)

...from a Wholesaler (FakeBt Microservice)

...and receiving an SMS when it is completed (Sms Microservice)

![alt text](https://raw.githubusercontent.com/JonQuxBurton/MicroservicesSample/master/MsSampleDiagram1.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MicroservicesSample/master/MsSampleDiagram2.png)
![alt text](https://raw.githubusercontent.com/JonQuxBurton/MicroservicesSample/master/MsSampleDiagram3.png)


Launching
---------
Launch the system using docker:

$ docker-compose up -d --build


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
