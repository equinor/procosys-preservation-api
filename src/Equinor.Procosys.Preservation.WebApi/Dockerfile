FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY . .
RUN ls
WORKDIR "/Equinor.Procosys.Preservation.WebApi"
RUN ls

# FROM build AS publish

# RUN dotnet publish "Equinor.Procosys.Preservation.WebApi.csproj" -c Release -o /app/publish

# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443
# ENV ASPNETCORE_HTTPS_PORT 443

# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Equinor.Procosys.Preservation.WebApi.dll"]