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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class AccessValidatorTests
    {
        private AccessValidator _dut;
        private const int TagIdWithAccess = 1;
        private const int TagIdWithoutAccess = 2;
        private const string ProjectWithAccess = "TestProjectWithAccess";
        private const string ProjectWithoutAccess = "TestProjectWithoutAccess";

        [TestInitialize]
        public void Setup()
        {
            var projectAccessCheckerMock = new Mock<IProjectAccessChecker>();
            var contentRestrictionsCheckerMock = new Mock<IContentRestrictionsChecker>();
            var tagHelperMock = new Mock<ITagHelper>();

            _dut = new AccessValidator(
                projectAccessCheckerMock.Object,
                contentRestrictionsCheckerMock.Object,
                tagHelperMock.Object);
            projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithoutAccess)).Returns(false);
            projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithAccess)).Returns(true);
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithAccess)).Returns(Task.FromResult(ProjectWithAccess));
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithoutAccess)).Returns(Task.FromResult(ProjectWithoutAccess));

            // todo tests for content restrictions
        }

        #region commands

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new PreserveCommand(TagIdWithAccess);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new PreserveCommand(TagIdWithoutAccess);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new BulkPreserveCommand(new List<int>{TagIdWithAccess});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new BulkPreserveCommand(new List<int>{TagIdWithoutAccess});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateAreaTagCommand_ShouldReturnTrue_WhenAccessToProject()
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

        [TestMethod]
        public async Task ValidateAsync_OnCreateTagsCommand_ShouldReturnTrue_WhenAccessToProject()
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

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccess});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new StartPreservationCommand(new List<int>{TagIdWithoutAccess});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new CreateActionCommand(TagIdWithAccess, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CreateActionCommand(TagIdWithoutAccess, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new UpdateActionCommand(TagIdWithAccess, 0, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateActionCommand(TagIdWithoutAccess, 0, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnTrue_WhenAccessToProject()
        {
            // Arrange
            var command = new StopPreservationCommand(new List<int> { TagIdWithAccess });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStopPreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new StopPreservationCommand(new List<int> { TagIdWithoutAccess });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

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
            var query = new GetActionDetailsQuery(TagIdWithAccess, 0);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionDetailsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetActionDetailsQuery(TagIdWithoutAccess, 0);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagActionsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagActionsQuery(TagIdWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagActionsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagActionsQuery(TagIdWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagDetailsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagDetailsQuery(TagIdWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagDetailsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagDetailsQuery(TagIdWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagRequirementsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagRequirementsQuery(TagIdWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagRequirementsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagRequirementsQuery(TagIdWithoutAccess);
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
