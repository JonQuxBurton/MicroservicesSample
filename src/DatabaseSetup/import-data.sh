#wait for the SQL Server to come up
while [ ! -f /var/opt/mssql/log/errorlog ]
do
  sleep 10s
done

sleep 20s

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Pass@word -d master -i setup.sql

#import the data from the csv file
#/opt/mssql-tools/bin/bcp Microservices.dbo.Customers in "usr/src/databasesetup/Products.csv" -c -t',' -S localhost -U sa -P Pass@word