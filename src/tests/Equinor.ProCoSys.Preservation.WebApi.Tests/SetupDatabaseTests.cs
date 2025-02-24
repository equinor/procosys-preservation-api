using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.WebApi.DiModules;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Equinor.ProCoSys.Preservation.WebApi.Seeding;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests;

[TestClass]
public class SetupDatabaseTests
{
    [TestMethod]
    public void ConfigureDatabase_ShouldNotAddServices_WhenNotInDevelopment()
    {
        // Arrange
        var builderMock = new Mock<IHostApplicationBuilder>();
        
        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Production);
        builderMock.Setup(b => b.Environment).Returns(environmentMock.Object);
        
        var configuration = new ConfigurationManager();
        configuration["MigrateDatabase"] = "true";
        configuration["SeedDummyData"] = "true";
        builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        var services = new List<ServiceDescriptor>();
        var servicesMock = new Mock<IServiceCollection>();
        servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback<ServiceDescriptor>(sd => services.Add(sd));
        builderMock.Setup(b => b.Services).Returns(servicesMock.Object);

        // Act
        builderMock.Object.ConfigureDatabase();

        // Assert
        services.Should().BeEmpty();
    }
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddServices_WhenInDevelopmentAndConfigurationsAreFalse()
    {
        // Arrange
        var builderMock = new Mock<IHostApplicationBuilder>();
        
        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        builderMock.Setup(b => b.Environment).Returns(environmentMock.Object);
        
        var configuration = new ConfigurationManager();
        configuration["MigrateDatabase"] = "false";
        configuration["SeedDummyData"] = "false";
        builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        var services = new List<ServiceDescriptor>();
        var servicesMock = new Mock<IServiceCollection>();
        servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback<ServiceDescriptor>(sd => services.Add(sd));
        builderMock.Setup(b => b.Services).Returns(servicesMock.Object);

        // Act
        builderMock.Object.ConfigureDatabase();
        var serviceTypes = services.Select(s => s.ImplementationType);

        // Assert
        services.Should().BeEmpty();
    }
    
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddDatabaseMigratorService_WhenInDevelopmentAndConfigurationIsTrue()
    {
        // Arrange
        var builderMock = new Mock<IHostApplicationBuilder>();
        
        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        builderMock.Setup(b => b.Environment).Returns(environmentMock.Object);
        
        var configuration = new ConfigurationManager();
        configuration["MigrateDatabase"] = "true";
        builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        var services = new List<ServiceDescriptor>();
        var servicesMock = new Mock<IServiceCollection>();
        servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback<ServiceDescriptor>(sd => services.Add(sd));
        builderMock.Setup(b => b.Services).Returns(servicesMock.Object);

        // Act
        builderMock.Object.ConfigureDatabase();
        var serviceTypes = services.Select(s => s.ImplementationType);

        // Assert
        serviceTypes.Should().Contain(typeof(DatabaseMigrator));
    }
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddSeederService_WhenInDevelopmentAndConfigurationIsTrue()
    {
        // Arrange
        var builderMock = new Mock<IHostApplicationBuilder>();
        
        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        builderMock.Setup(b => b.Environment).Returns(environmentMock.Object);
        
        var configuration = new ConfigurationManager();
        configuration["SeedDummyData"] = "true";
        builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        var services = new List<ServiceDescriptor>();
        var servicesMock = new Mock<IServiceCollection>();
        servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback<ServiceDescriptor>(sd => services.Add(sd));
        builderMock.Setup(b => b.Services).Returns(servicesMock.Object);

        // Act
        builderMock.Object.ConfigureDatabase();
        var serviceTypes = services.Select(s => s.ImplementationType);

        // Assert
        serviceTypes.Should().Contain(typeof(Seeder));
    }
}
