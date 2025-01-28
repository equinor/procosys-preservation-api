using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Services.ProjectImportService;

[TestClass]
public class ProjectImportServiceTests
{
    private Mock<IProjectRepository> _projectRepositoryMock;
    private Mock<IProjectApiService> _projectApiServiceMock;
    private Mock<IPlantProvider> _plantProviderMock;
    private Command.Services.ProjectImportService.ProjectImportService _dut;
    private readonly string _plant = "TestPlant";

    [TestInitialize]
    public void Setup()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _projectApiServiceMock = new Mock<IProjectApiService>();
        _plantProviderMock = new Mock<IPlantProvider>();
        _dut = new Command.Services.ProjectImportService.ProjectImportService(_projectRepositoryMock.Object,
            _projectApiServiceMock.Object, _plantProviderMock.Object);

        _plantProviderMock.Setup(p => p.Plant).Returns(_plant);
    }

    [TestMethod]
    public async Task ImportProjectAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var projectName = "TestProject";
        var mainProject = new ProCoSysProject
        {
            Name = projectName, Description = "TestDescription", ProCoSysGuid = Guid.NewGuid()
        };
        _projectApiServiceMock.Setup(p => p.TryGetProjectAsync(_plant, projectName)).ReturnsAsync(mainProject);

        // Act
        var result = await _dut.ImportProjectAsync(projectName);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(projectName, result.Name);
        Assert.AreEqual(mainProject.Description, result.Description);
        Assert.AreEqual(mainProject.ProCoSysGuid, result.Guid);
        _projectRepositoryMock.Verify(r => r.Add(It.IsAny<Project>()), Times.Once);
    }

    [TestMethod]
    public async Task ImportProjectAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectName = "NonExistentProject";

        // Act
        var result = await _dut.ImportProjectAsync(projectName);

        // Assert
        Assert.IsNull(result);
        _projectRepositoryMock.Verify(r => r.Add(It.IsAny<Project>()), Times.Never);
    }
}
