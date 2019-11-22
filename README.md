# pcs-preservation-api

Backend .Net Core 3 application for Preservation module in Project Completion System (ProCoSys).

## Run in Visual Studio
Choose to run as *Equinor.ProcoSys.Preservation.WebApi* in the dropdown menu and hit F5.

## Run from command line
In folder
```console
src\Equinor.Procosys.Preservation.WebApi\
```
run
```console
dotnet run
```

## Run in Docker
In folder
```console
src\
```
run
```console
docker-compose -f .\Docker-compose.yml -f .\Docker-compose.development.yml up
```
> Note: This does not work when connected to VPN, due to volume mounting issues.
