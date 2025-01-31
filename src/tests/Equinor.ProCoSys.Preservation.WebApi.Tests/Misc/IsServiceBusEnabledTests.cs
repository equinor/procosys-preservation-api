using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.WebApi.DiModules;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Misc;

[TestClass]
public class IsServiceBusEnabledTests
{
    [TestMethod]
    [DataRow("true", "false", "Production", true)]
    [DataRow("false", "false", "Production", false)]
    [DataRow("false", "true", "Production", false)]
    [DataRow("true", "false", "Development", false)]
    [DataRow("true", "true", "Development", true)]
    [DataRow("false", "false", "Development", false)]
    public void IsServiceBusEnabled_ShouldGiveExpectedIndicationGivenConfiguration(string enable, string enableInDevelopment, string environment, bool expected)
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        
        var builder = WebApplication.CreateBuilder();
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
