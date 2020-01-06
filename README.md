# pcs-preservation-api

Backend .Net Core 3 application for Preservation module in Project Completion System (ProCoSys).

Note: Before running the application, the secret-file need to be updated (right-click 'Equinor.Procosys.Preservation.WebApi' and select 'Manage User Secrets'). 

     {
       "API": {
         "Authority": "",
         "Audience": ""
       },
       "Swagger": {
         "AuthorizationUrl": "",
         "ClientId": "",
         "Scopes": {
         }
       },
       "ConnectionStrings": {
         "PreservationContext": ""
       },
       "MainApiBaseUrl": ""
     }

## How to run in Visual Studio
Choose to run as *Equinor.ProcoSys.Preservation.WebApi* in the dropdown menu and hit F5.

## How to run from command line
In folder
```
src\Equinor.Procosys.Preservation.WebApi\
```
run
```console
dotnet run
```

## How to run in Docker
In folder
```
src\
```
run
```console
docker-compose -f .\Docker-compose.yml -f .\Docker-compose.development.yml up
```
> Note: This does not work when connected to VPN, due to volume mounting issues.
