FROM mcr.microsoft.com/mssql/server:latest

RUN mkdir -p /usr/src/databasesetup
WORKDIR /usr/src/databasesetup

COPY ./src/DatabaseSetup /usr/src/databasesetup

# Grant permissions for the import-data script to be executable
RUN chmod +x /usr/src/databasesetup/import-data.sh

CMD /bin/bash ./entrypoint.sh