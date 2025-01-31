using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.WebApi.DiModules;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Misc;

[TestClass]
public class IsServiceBusEnabledTests
{
    [TestMethod]
    [DataRow("true", "true", "Production", true)]
    [DataRow("true", "true", "Development", true)]
    [DataRow("true", "false", "Production", true)]
    [DataRow("true", "false", "Development", false)]
    [DataRow("false", "false", "Production", false)]
    [DataRow("false", "true", "Production", false)]
    [DataRow("false", "false", "Development", false)]
    public void IsServiceBusEnabled_ShouldGiveExpectedIndicationForConfiguration(string enable, string enableInDevelopment, string environment, bool expected)
    {
        // Arrange
        var options = new WebApplicationOptions
        {
            EnvironmentName = environment
        };
        
        var builder = WebApplication.CreateBuilder(options);
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ServiceBus:Enable", enable },
            { "ServiceBus:EnableInDevelopment", enableInDevelopment }
        });

        // Act
        var result = builder.IsServiceBusEnabled();
        
        // Assert
        result.Should().Be(expected);
    }
}
