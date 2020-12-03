# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app


COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out
# Stage 2
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LeaveManagment.dll"]

#run docker image :  docker build -t leavemanagmentimage .
#run docker container : docker run -d -p 8080:80 --name leavemanagmentapp leavemanagmentimage

# if connecting on local SQL Server,set port in Sql server manager (49712), open this port in firewall,find IP address for host and create a connection string
# "DefaultConnection": "Server=192.168.8.109,49712;Initial Catalog=LeaveManagment;User ID=sa;Password=1234"

# ---------- if dockerize SQL server , the command is below but app need to create DB and tables and create docker-compose.yml file------------
# run docker-compose up

#install sql docker localy : docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=##ofaster123A' -e 'MSSQL_PID=Developer' --cap-add SYS_PTRACE -p 1401:1433 -d mcr.microsoft.com/mssql/server:2017-latest            
