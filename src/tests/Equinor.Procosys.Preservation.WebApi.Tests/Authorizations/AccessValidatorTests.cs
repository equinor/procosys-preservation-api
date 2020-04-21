using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.TagCommands.StopPreservation;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.Query.GetUniqueTagAreas;
using Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.Procosys.Preservation.Query.GetUniqueTagModes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class AccessValidatorTests
    {
        private AccessValidator _dut;
        private Mock<IContentRestrictionsChecker> _contentRestrictionsCheckerMock;
        private Mock<IPlantAccessChecker> _plantAccessCheckerMock;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IProjectAccessChecker> _projectAccessCheckerMock;
        private Mock<ILogger<AccessValidator>> _loggerMock;
        private const string TestPlant = "PLANT";
        private const int TagIdWithAccessToProject = 1;
        private const int TagIdWithoutAccessToProject = 2;
        private const string ProjectWithAccess = "TestProjectWithAccess";
        private const string ProjectWithoutAccess = "TestProjectWithoutAccess";
        private const string RestrictedToContent = "ResponsbleA";

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(p => p.Plant).Returns(TestPlant);

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _plantAccessCheckerMock = new Mock<IPlantAccessChecker>();
            _plantAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToPlant(TestPlant)).Returns(true);

            _projectAccessCheckerMock = new Mock<IProjectAccessChecker>();
            _contentRestrictionsCheckerMock = new Mock<IContentRestrictionsChecker>();
            
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithoutAccess)).Returns(false);
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithAccess)).Returns(true);
            
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(true);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            
            var tagHelperMock = new Mock<ITagHelper>();
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithAccessToProject)).Returns(Task.FromResult(ProjectWithAccess));
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithoutAccessToProject)).Returns(Task.FromResult(ProjectWithoutAccess));
            tagHelperMock.Setup(p => p.GetResponsibleCodeAsync(TagIdWithAccessToProject)).Returns(Task.FromResult(RestrictedToContent));

            _loggerMock = new Mock<ILogger<AccessValidator>>();

            _dut = new AccessValidator(
                _plantProviderMock.Object,
                currentUserProviderMock.Object,
                _plantAccessCheckerMock.Object,
                _projectAccessCheckerMock.Object,
                _contentRestrictionsCheckerMock.Object,
                tagHelperMock.Object,
                _loggerMock.Object);
        }

        #region Test plant access

        [TestMethod]
        public async Task ValidateAsync_ShouldReturnFalse_WhenNoAccessToPlant()
        {
            // Arrange
            _plantAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToPlant(TestPlant)).Returns(false);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public async Task ValidateAsync_ShouldNotCheckOtherAccess_WhenNoAccessToPlant()
        {
            // Arrange
            _plantAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToPlant(TestPlant)).Returns(false);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            await _dut.ValidateAsync(command);

            // Assert
            _projectAccessCheckerMock.Verify(p => p.HasCurrentUserAccessToProject(It.IsAny<string>()), Times.Never);
            _contentRestrictionsCheckerMock.Verify(c => c.HasCurrentUserAnyRestrictions(), Times.Never);
            _contentRestrictionsCheckerMock.Verify(c => c.HasCurrentUserExplicitAccessToContent(It.IsAny<string>()), Times.Never);
        }
        
        [TestMethod]
        public async Task ValidateAsync_ShouldReturnTrue_WhenNoRequestForPlant()
        {
            // Arrange
            _plantProviderMock.SetupGet(p => p.Plant).Returns((string)null);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region commands

        #region PreserveCommand
        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new PreserveCommand(TagIdWithoutAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region BulkPreserveCommand
        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new BulkPreserveCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new BulkPreserveCommand(new List<int>{TagIdWithoutAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new BulkPreserveCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new BulkPreserveCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region CreateAreaTagCommand
        [TestMethod]
        public async Task ValidateAsync_OnCreateAreaTagCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new CreateAreaTagCommand(ProjectWithAccess, TagType.PoArea, null, null, null, 1, null, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateAreaTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CreateAreaTagCommand(ProjectWithoutAccess, TagType.PoArea, null, null, null, 1, null, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region CreateTagsCommand
        [TestMethod]
        public async Task ValidateAsync_OnCreateTagsCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new CreateTagsCommand(null, ProjectWithAccess, 1, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateTagsCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CreateTagsCommand(null, ProjectWithoutAccess, 1, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region StartPreservationCommand
        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new StartPreservationCommand(new List<int>{TagIdWithoutAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region CreateActionCommand
        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new CreateActionCommand(TagIdWithAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CreateActionCommand(TagIdWithoutAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new CreateActionCommand(TagIdWithAccessToProject, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new CreateActionCommand(TagIdWithAccessToProject, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateActionCommand
        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateActionCommand(TagIdWithoutAccessToProject, 0, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region StopPreservationCommand
        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new StopPreservationCommand(new List<int> { TagIdWithAccessToProject });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new StopPreservationCommand(new List<int> { TagIdWithoutAccessToProject });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnFalse_WhenNoExplicitAccessToContent()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new StopPreservationCommand(new List<int> { TagIdWithAccessToProject });
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnTrue_WhenNoContentRestrictions()
        {
            // Arrange
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserAnyRestrictions()).Returns(false);
            _contentRestrictionsCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            var command = new StopPreservationCommand(new List<int> { TagIdWithAccessToProject });
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #endregion

        #region queries

        [TestMethod]
        public async Task ValidateAsync_OnGetTagsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagsQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagsQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionDetailsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetActionDetailsQuery(TagIdWithAccessToProject, 0);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionDetailsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetActionDetailsQuery(TagIdWithoutAccessToProject, 0);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagActionsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagActionsQuery(TagIdWithAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagActionsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagActionsQuery(TagIdWithoutAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagDetailsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagDetailsQuery(TagIdWithAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagDetailsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagDetailsQuery(TagIdWithoutAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagRequirementsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagRequirementsQuery(TagIdWithAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagRequirementsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagRequirementsQuery(TagIdWithoutAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagAreasQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagAreasQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagAreasQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagAreasQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagDisciplinesQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagDisciplinesQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagDisciplinesQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagDisciplinesQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagFunctionsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagFunctionsQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagFunctionsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagFunctionsQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagJourneysQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagJourneysQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagJourneysQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagJourneysQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagModesQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagModesQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagModesQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagModesQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagRequirementTypesQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagRequirementTypesQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagRequirementTypesQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagRequirementTypesQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagResponsiblesQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetUniqueTagResponsiblesQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetUniqueTagResponsiblesQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetUniqueTagResponsiblesQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSearchTagsByTagFunctionQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new SearchTagsByTagFunctionQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSearchTagsByTagFunctionQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new SearchTagsByTagFunctionQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSearchTagsByTagNoQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new SearchTagsByTagNoQuery(ProjectWithAccess, null);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSearchTagsByTagNoQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new SearchTagsByTagNoQuery(ProjectWithoutAccess, null);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}
