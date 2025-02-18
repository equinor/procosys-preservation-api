using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Misc;

[TestClass]
public class GetConfigTests
{
    private IConfiguration _configuration;
    private const string TestKey = "TestKey";
    private const string TestValue = "TestValue";

    [TestInitialize]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string> { {TestKey, TestValue} };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
    
    [TestMethod]
    public void GetConfig_ShouldReturnExpectedValue()
    {
        // Act
        var result = _configuration.GetConfig<string>(TestKey);
        
        // Assert
        result.Should().Be(TestValue);
    }
    
    [TestMethod]
    public void GetConfig_ShouldThrowArgumentException_WhenKeyIsNotFound()
    {
        // Arrange
        var getConfigFunc = () => _configuration.GetConfig<string>("NonExistingKey");
        
        // Assert
        getConfigFunc.Should().Throw<ArgumentException>();
    }
}
