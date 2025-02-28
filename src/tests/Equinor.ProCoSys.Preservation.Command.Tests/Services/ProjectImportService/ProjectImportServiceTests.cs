using System;
using System.Threading;
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
    private Mock<IMainApiProjectApiForUserService> _projectApiServiceMock;
    private Mock<IPlantProvider> _plantProviderMock;
    private Command.Services.ProjectImportService.ProjectImportService _dut;
    private readonly string _plant = "TestPlant";

    [TestInitialize]
    public void Setup()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _projectApiServiceMock = new Mock<IMainApiProjectApiForUserService>();
        _plantProviderMock = new Mock<IPlantProvider>();
        _dut = new Command.Services.ProjectImportService.ProjectImportService(_projectRepositoryMock.Object,
            _projectApiServiceMock.Object, _plantProviderMock.Object);

        _plantProviderMock.Setup(p => p.Plant).Returns(_plant);
    }

    [TestMethod]
    public async Task ImportProjectAsync_ShouldImportProject_WhenProjectDoesNotExists()
    {
        // Arrange
        var projectName = "TestProject";
        var projectDescription = "TestDescription";
        var mainProject = new ProCoSysProject
        {
            Name = projectName, Description = projectDescription, ProCoSysGuid = Guid.NewGuid()
        };
        _projectApiServiceMock.Setup(p => p.TryGetProjectAsync(_plant, projectName, It.IsAny<CancellationToken>())).ReturnsAsync(mainProject);

        // Act
        var result = await _dut.TryGetOrImportProjectAsync(projectName, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(projectName, result.Name);
        Assert.AreEqual(mainProject.Description, result.Description);
        Assert.AreEqual(mainProject.ProCoSysGuid, result.Guid);
        _projectRepositoryMock.Verify(
            r => r.Add(It.Is<Project>(p => p.Name == projectName && p.Description == projectDescription)), Times.Once);
        _projectApiServiceMock.Verify(p => p.TryGetProjectAsync(_plant, projectName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ImportProjectAsync_ShouldGetProject_WhenProjectDoesExists()
    {
        // Arrange
        var projectName = "TestProject";
        var project = new Project(_plant, projectName, "TestDescription", Guid.NewGuid());
        _projectRepositoryMock.Setup(p => p.GetProjectOnlyByNameAsync(projectName)).ReturnsAsync(project);

        // Act
        var result = await _dut.TryGetOrImportProjectAsync(projectName, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(projectName, result.Name);
        Assert.AreEqual(project.Description, result.Description);
        _projectRepositoryMock.Verify(r => r.GetProjectOnlyByNameAsync(projectName), Times.Once);
        _projectApiServiceMock.Verify(p => p.TryGetProjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task ImportProjectAsync_ShouldReturnNull_WhenProjectIsNotFound()
    {
        // Arrange
        var projectName = "NonExistentProject";

        // Act
        var result = await _dut.TryGetOrImportProjectAsync(projectName, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
        _projectRepositoryMock.Verify(r => r.Add(It.IsAny<Project>()), Times.Never);
    }
}
