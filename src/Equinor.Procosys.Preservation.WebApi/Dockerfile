FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Equinor.Procosys.Preservation.WebApi.csproj", ""]
RUN dotnet restore "./Equinor.Procosys.Preservation.WebApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Equinor.Procosys.Preservation.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Equinor.Procosys.Preservation.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Preservation.WebApi.dll"]