using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Requirement = Equinor.Procosys.Preservation.Command.TagCommands.CreateTag.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateTag
{
    [TestClass]
    public class CreateTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int StepId = 11;
        private const int RequirementDefinitionId = 99;

        private Mock<Step> _stepMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Project _projectAddedToRepository;
        private Tag _tagAddedToProject;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<RequirementDefinition> _rdMock;
        private Mock<ITagApiService> _tagApiServiceMock;

        private ProcosysTagDetails _mainTagDetails;
        private CreateTagCommand _command;
        private CreateTagCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(StepId);
            
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId))
                .Returns(Task.FromResult(_stepMock.Object));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(x => x.Add(It.IsAny<Project>()))
                .Callback<Project>(project =>
                {
                    _projectAddedToRepository = project;
                });

            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            _rdMock = new Mock<RequirementDefinition>();
            _rdMock.SetupGet(x => x.Id).Returns(RequirementDefinitionId);
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(RequirementDefinitionId))
                .Returns(Task.FromResult(_rdMock.Object));

            _mainTagDetails =
                new ProcosysTagDetails
                {
                    AreaCode = "AreaCode",
                    CallOffNo = "CalloffNo",
                    CommPkgNo = "CommPkgNo",
                    Description = "Description",
                    DisciplineCode = "DisciplineCode",
                    McPkgNo = "McPkgNo",
                    PurchaseOrderNo = "PurchaseOrderNo",
                    TagFunctionCode = "TagFunctionCode",
                    TagNo = "TagNo"
                };
            
            _tagApiServiceMock = new Mock<ITagApiService>();
            _tagApiServiceMock
                .Setup(x => x.GetTagDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_mainTagDetails));

            _command = new CreateTagCommand(
                "TagNo",
                "ProjectName",
                _stepMock.Object.Id,
                new List<Requirement>
                {
                    new Requirement(RequirementDefinitionId, 1)
                });
            
            _dut = new CreateTagCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _tagApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldAddTagToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual("AreaCode", _tagAddedToProject.AreaCode);
            Assert.AreEqual("CalloffNo", _tagAddedToProject.Calloff);
            Assert.AreEqual("CommPkgNo", _tagAddedToProject.CommPkgNo);
            Assert.AreEqual("DisciplineCode", _tagAddedToProject.DisciplineCode);
            Assert.AreEqual(0, _tagAddedToProject.Id);
            Assert.AreEqual(false, _tagAddedToProject.IsAreaTag);
            Assert.AreEqual("McPkgNo", _tagAddedToProject.McPkgNo);
            Assert.AreEqual("ProjectName", _tagAddedToProject.ProjectName);
            Assert.AreEqual("PurchaseOrderNo", _tagAddedToProject.PurchaseOrderNo);
            Assert.AreEqual("TestPlant", _tagAddedToProject.Schema);
            Assert.AreEqual(StepId, _tagAddedToProject.StepId);
            Assert.AreEqual("TagFunctionCode", _tagAddedToProject.TagFunctionCode);
            Assert.AreEqual("TagNo", _tagAddedToProject.TagNo);
            Assert.AreEqual(1, _tagAddedToProject.Requirements.Count);
            Assert.AreEqual(RequirementDefinitionId, _tagAddedToProject.Requirements.First().RequirementDefinitionId);
        }

        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
