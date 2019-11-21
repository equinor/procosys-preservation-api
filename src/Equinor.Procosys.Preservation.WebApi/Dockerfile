FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src

# Copy solution and project files separately and restore NuGet packages to create layers
COPY *.sln .
COPY Equinor.Procosys.Preservation.Command/*.csproj ./Equinor.Procosys.Preservation.Command/
COPY Equinor.Procosys.Preservation.Command.Tests/*.csproj ./Equinor.Procosys.Preservation.Command.Tests/
COPY Equinor.Procosys.Preservation.Domain/*.csproj ./Equinor.Procosys.Preservation.Domain/
COPY Equinor.Procosys.Preservation.Domain.Tests/*.csproj ./Equinor.Procosys.Preservation.Domain.Tests/
COPY Equinor.Procosys.Preservation.Infrastructure/*.csproj ./Equinor.Procosys.Preservation.Infrastructure/
COPY Equinor.Procosys.Preservation.Infrastructure.Tests/*.csproj ./Equinor.Procosys.Preservation.Infrastructure.Tests/
COPY Equinor.Procosys.Preservation.Query/*.csproj ./Equinor.Procosys.Preservation.Query/
COPY Equinor.Procosys.Preservation.Query.Tests/*.csproj ./Equinor.Procosys.Preservation.Query.Tests/
COPY Equinor.Procosys.Preservation.WebApi/*.csproj ./Equinor.Procosys.Preservation.WebApi/
COPY Equinor.Procosys.Preservation.WebApi.Tests/*.csproj ./Equinor.Procosys.Preservation.WebApi.Tests/
RUN dotnet restore

# Copy full solution
COPY . .

RUN dotnet build -c Release --no-restore

# Test runner target
FROM build AS testrunner
WORKDIR /src
RUN dotnet test -c Release --no-build --no-restore
#CMD ["dotnet", "test", "-c Release", "--no-build", "--no-restore"]

# Publish the application
FROM build AS publish
WORKDIR "/src/Equinor.Procosys.Preservation.WebApi"
RUN dotnet publish "Equinor.Procosys.Preservation.WebApi.csproj" -c Release --no-restore -o /app/publish

# Create a runtime image and copy the application over
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Preservation.WebApi.dll"]