FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# Copy project files separately and restore NuGet packages to create layers. Skip test projects!
COPY Equinor.Procosys.Preservation.Command/*.csproj ./Equinor.Procosys.Preservation.Command/
COPY Equinor.Procosys.Preservation.Domain/*.csproj ./Equinor.Procosys.Preservation.Domain/
COPY Equinor.Procosys.Preservation.Infrastructure/*.csproj ./Equinor.Procosys.Preservation.Infrastructure/
COPY Equinor.Procosys.Preservation.MainApi/*.csproj ./Equinor.Procosys.Preservation.MainApi/
COPY Equinor.Procosys.Preservation.BlobStorage/*.csproj ./Equinor.Procosys.Preservation.BlobStorage/
COPY Equinor.Procosys.Preservation.Query/*.csproj ./Equinor.Procosys.Preservation.Query/
COPY Equinor.Procosys.Preservation.WebApi/*.csproj ./Equinor.Procosys.Preservation.WebApi/

# Set workdir where main project is
WORKDIR "/src/Equinor.Procosys.Preservation.WebApi"

# Copy full solution
COPY . .

# Build
RUN dotnet build "Equinor.Procosys.Preservation.WebApi.csproj" -c Release

# Publish the application
FROM build AS publish
WORKDIR "/src/Equinor.Procosys.Preservation.WebApi"
RUN dotnet publish "Equinor.Procosys.Preservation.WebApi.csproj" -c Release --no-restore -o /app/publish

# Define the image used for the final result
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

# Install System.Drawing native dependencies (added because of Excel export (ClosedXML library) support).
RUN apt-get update
RUN apt-get install -y libc6 libgdiplus
RUN rm -rf /var/lib/apt/lists/*

# Create non-root user. Set ui to 9999 to avoid conflicts with host OS just in case
RUN adduser --disabled-password --uid 9999 --gecos "" apprunner
# Create the folder and set the non-root as owner
RUN mkdir /app && chown apprunner.apprunner /app
# Change the user from root to non-root- From now on, all Docker commands are run as non-root user (except for COPY)
USER apprunner
# Set the port to 5000 since the default production port is 80 and on-root users cannot bind to this port
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000
WORKDIR /app
# Copy the published files from the build image into this one
# COPY defaults to copying files as root, specify the user that should be the owner
COPY --chown=apprunner:apprunner --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Equinor.Procosys.Preservation.WebApi.dll"]