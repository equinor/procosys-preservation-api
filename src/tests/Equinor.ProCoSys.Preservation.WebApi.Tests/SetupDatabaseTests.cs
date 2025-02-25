using System;
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
    private Mock<IHostApplicationBuilder> _builderMock;
    private Mock<IWebHostEnvironment> _environmentMock;
    
    private IList<ServiceDescriptor> _services;
    private IEnumerable<Type> ServiceTypes => _services.Select(s => s.ImplementationType);
    
    [TestInitialize]
    public void TestInitialize()
    {
        _builderMock = new Mock<IHostApplicationBuilder>();
        
        _environmentMock = new Mock<IWebHostEnvironment>();
        _builderMock.Setup(b => b.Environment).Returns(_environmentMock.Object);
        
        _services = new List<ServiceDescriptor>();
        var servicesMock = new Mock<IServiceCollection>();
        servicesMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback<ServiceDescriptor>(sd => _services.Add(sd));
        _builderMock.Setup(b => b.Services).Returns(servicesMock.Object);
    }
    
    [TestMethod]
    public void ConfigureDatabase_ShouldNotAddServices_WhenNotInDevelopment()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Production);
        
        var configuration = new ConfigurationManager();
        configuration["Application:MigrateDatabase"] = "true";
        configuration["Application:SeedDummyData"] = "true";
        _builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        // Act
        _builderMock.Object.ConfigureDatabase();

        // Assert
        ServiceTypes.Should().BeEmpty();
    }
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddServices_WhenInDevelopmentAndConfigurationsAreFalse()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        
        var configuration = new ConfigurationManager();
        configuration["Application:MigrateDatabase"] = "false";
        configuration["Application:SeedDummyData"] = "false";
        _builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        // Act
        _builderMock.Object.ConfigureDatabase();

        // Assert
        ServiceTypes.Should().BeEmpty();
    }
    
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddDatabaseMigratorService_WhenInDevelopmentAndConfigurationIsTrue()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        
        var configuration = new ConfigurationManager();
        configuration["Application:MigrateDatabase"] = "true";
        _builderMock.Setup(b => b.Configuration).Returns(configuration);
        
        // Act
        _builderMock.Object.ConfigureDatabase();

        // Assert
        ServiceTypes.Should().Contain(typeof(DatabaseMigrator));
    }
    
    [TestMethod]
    public void ConfigureDatabase_ShouldAddSeederService_WhenInDevelopmentAndConfigurationIsTrue()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        
        var configuration = new ConfigurationManager();
        configuration["Application:SeedDummyData"] = "true";
        _builderMock.Setup(b => b.Configuration).Returns(configuration);

        // Act
        _builderMock.Object.ConfigureDatabase();

        // Assert
        ServiceTypes.Should().Contain(typeof(Seeder));
    }
}
