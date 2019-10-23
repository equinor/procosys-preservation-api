FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Equinor.Procosys.Preservation.WebApi/Equinor.Procosys.Preservation.WebApi.csproj", "Equinor.Procosys.Preservation.WebApi/"]
COPY ["Equinor.Procosys.Preservation.Query/Equinor.Procosys.Preservation.Query.csproj", "Equinor.Procosys.Preservation.Query/"]
COPY ["Equinor.Procosys.Preservation.Domain/Equinor.Procosys.Preservation.Domain.csproj", "Equinor.Procosys.Preservation.Domain/"]
COPY ["Equinor.Procosys.Preservation.Infrastructure/Equinor.Procosys.Preservation.Infrastructure.csproj", "Equinor.Procosys.Preservation.Infrastructure/"]
COPY ["Equinor.Procosys.Preservation.Command/Equinor.Procosys.Preservation.Command.csproj", "Equinor.Procosys.Preservation.Command/"]
RUN dotnet restore "Equinor.Procosys.Preservation.WebApi/Equinor.Procosys.Preservation.WebApi.csproj"
COPY . .
WORKDIR "/src/Equinor.Procosys.Preservation.WebApi"
RUN dotnet build "Equinor.Procosys.Preservation.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Equinor.Procosys.Preservation.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Preservation.WebApi.dll"]