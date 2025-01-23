﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.ActionCommands.CloseAction;
using Equinor.ProCoSys.Preservation.Command.ActionCommands.CreateAction;
using Equinor.ProCoSys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.ProCoSys.Preservation.Command.RequirementCommands.DeleteAttachment;
using Equinor.ProCoSys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.ProCoSys.Preservation.Command.RequirementCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Delete;
using Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoScopeTags;
using Equinor.ProCoSys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CompletePreservation;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateTags;
using Equinor.ProCoSys.Preservation.Command.TagCommands.DeleteTag;
using Equinor.ProCoSys.Preservation.Command.TagCommands.Preserve;
using Equinor.ProCoSys.Preservation.Command.TagCommands.SetInService;
using Equinor.ProCoSys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.ProCoSys.Preservation.Command.TagCommands.Transfer;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UnvoidTag;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTag;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagStep;
using Equinor.ProCoSys.Preservation.Command.TagCommands.VoidTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Query.GetActionAttachment;
using Equinor.ProCoSys.Preservation.Query.GetActionAttachments;
using Equinor.ProCoSys.Preservation.Query.GetActionDetails;
using Equinor.ProCoSys.Preservation.Query.GetActions;
using Equinor.ProCoSys.Preservation.Query.GetProjectByName;
using Equinor.ProCoSys.Preservation.Query.GetTagAttachment;
using Equinor.ProCoSys.Preservation.Query.GetTagAttachments;
using Equinor.ProCoSys.Preservation.Query.GetTagDetails;
using Equinor.ProCoSys.Preservation.Query.GetTagRequirements;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagAreas;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagModes;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagResponsibles;
using Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RequirementPreserveCommand = Equinor.ProCoSys.Preservation.Command.RequirementCommands.Preserve.PreserveCommand;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class AccessValidatorTests
    {
        private AccessValidator _dut;
        private Mock<IRestrictionRolesChecker> _restrictionRolesCheckerMock;
        private Mock<IProjectAccessChecker> _projectAccessCheckerMock;
        private Mock<ILogger<AccessValidator>> _loggerMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private const int TagIdWithAccessToProject = 1;
        private const int TagIdWithoutAccessToProject = 2;
        private const string ProjectWithAccess = "TestProjectWithAccess";
        private const string ProjectWithoutAccess = "TestProjectWithoutAccess";
        private const string RestrictedToContent = "ResponsbleA";

        [TestInitialize]
        public void Setup()
        {
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();

            _projectAccessCheckerMock = new Mock<IProjectAccessChecker>();
            _restrictionRolesCheckerMock = new Mock<IRestrictionRolesChecker>();
            
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithoutAccess)).Returns(false);
            _projectAccessCheckerMock.Setup(p => p.HasCurrentUserAccessToProject(ProjectWithAccess)).Returns(true);
            
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(true);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(false);
            
            var tagHelperMock = new Mock<ITagHelper>();
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithAccessToProject)).Returns(Task.FromResult(ProjectWithAccess));
            tagHelperMock.Setup(p => p.GetProjectNameAsync(TagIdWithoutAccessToProject)).Returns(Task.FromResult(ProjectWithoutAccess));
            tagHelperMock.Setup(p => p.GetResponsibleCodeAsync(TagIdWithAccessToProject)).Returns(Task.FromResult(RestrictedToContent));

            _loggerMock = new Mock<ILogger<AccessValidator>>();

            _dut = new AccessValidator(
                _currentUserProviderMock.Object,
                _projectAccessCheckerMock.Object,
                _restrictionRolesCheckerMock.Object,
                tagHelperMock.Object,
                _loggerMock.Object);
        }

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
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new PreserveCommand(TagIdWithAccessToProject);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnPreserveCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
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
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new BulkPreserveCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnBulkPreserveCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
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
            var command = new CreateAreaTagCommand(ProjectWithAccess, TagType.PoArea, null, null, null, null, 1, null, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateAreaTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CreateAreaTagCommand(ProjectWithoutAccess, TagType.PoArea, null, null, null, null, 1, null, null, null, null);
            
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

        #region AutoScopeTagsCommand
        [TestMethod]
        public async Task ValidateAsync_OnAutoScopeTagsCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new AutoScopeTagsCommand(null, ProjectWithAccess, 1, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnAutoScopeTagsCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new AutoScopeTagsCommand(null, ProjectWithoutAccess, 1, null, null);
            
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
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnStartPreservationCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new StartPreservationCommand(new List<int>{TagIdWithAccessToProject});
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region SetInServiceCommand
        [TestMethod]
        public async Task ValidateAsync_OnSetInServiceCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new SetInServiceCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSetInServiceCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new SetInServiceCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithoutAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSetInServiceCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new SetInServiceCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnSetInServiceCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new SetInServiceCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UndoStartPreservationCommand
        [TestMethod]
        public async Task ValidateAsync_OnUndoStartPreservationCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UndoStartPreservationCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUndoStartPreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UndoStartPreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithoutAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUndoStartPreservationCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UndoStartPreservationCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUndoStartPreservationCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UndoStartPreservationCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

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
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new CreateActionCommand(TagIdWithAccessToProject, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCreateActionCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
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
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateActionCommand(TagIdWithoutAccessToProject, 0, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateActionCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UpdateActionCommand(TagIdWithAccessToProject, 0, null, null, null, null);
            
            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region CloseActionCommand
        [TestMethod]
        public async Task ValidateAsync_OnCloseActionCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new CloseActionCommand(TagIdWithAccessToProject, 0, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCloseActionCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CloseActionCommand(TagIdWithoutAccessToProject, 0, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCloseActionCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new CloseActionCommand(TagIdWithAccessToProject, 0, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCloseActionCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new CloseActionCommand(TagIdWithAccessToProject, 0, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region CompletePreservationCommand
        [TestMethod]
        public async Task ValidateAsync_OnCompletePreservationCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCompletePreservationCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithoutAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCompletePreservationCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnCompletePreservationCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region TransferCommand
        [TestMethod]
        public async Task ValidateAsync_OnTransferCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new TransferCommand(new List<IdAndRowVersion> {new IdAndRowVersion(TagIdWithAccessToProject, null)});

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnTransferCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new TransferCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithoutAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnTransferCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new TransferCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnTransferCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new TransferCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) });

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region VoidTagCommand
        [TestMethod]
        public async Task ValidateAsync_OnVoidTagCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new VoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnVoidTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new VoidTagCommand(TagIdWithoutAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnVoidTagCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new VoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnVoidTagCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new VoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UnvoidTagCommand
        [TestMethod]
        public async Task ValidateAsync_OnUnvoidTagCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UnvoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUnvoidTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UnvoidTagCommand(TagIdWithoutAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUnvoidTagCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UnvoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUnvoidTagCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UnvoidTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteTagCommand
        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new DeleteTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new DeleteTagCommand(TagIdWithoutAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new DeleteTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new DeleteTagCommand(TagIdWithAccessToProject, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateTagCommand
        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UpdateTagCommand(TagIdWithAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateTagCommand(TagIdWithoutAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UpdateTagCommand(TagIdWithAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UpdateTagCommand(TagIdWithAccessToProject, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion
        
        #region UpdateTagRequirementsCommand
        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagRequirementsCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UpdateTagRequirementsCommand(TagIdWithAccessToProject, null, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagRequirementsCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateTagRequirementsCommand(TagIdWithoutAccessToProject, null, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagRequirementsCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UpdateTagRequirementsCommand(TagIdWithAccessToProject, null, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagRequirementsCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UpdateTagRequirementsCommand(TagIdWithAccessToProject, null, null, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateTagStepCommand
        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagStepCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UpdateTagStepCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) }, 0);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagStepCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UpdateTagStepCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithoutAccessToProject, null) }, 0);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagStepCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UpdateTagStepCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) }, 0);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUpdateTagStepCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UpdateTagStepCommand(new List<IdAndRowVersion> { new IdAndRowVersion(TagIdWithAccessToProject, null) }, 0);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteTagAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new DeleteTagAttachmentCommand(TagIdWithAccessToProject, 1, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new DeleteTagAttachmentCommand(TagIdWithoutAccessToProject, 1, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new DeleteTagAttachmentCommand(TagIdWithAccessToProject, 1, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteTagAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new DeleteTagAttachmentCommand(TagIdWithAccessToProject, 1, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UploadTagAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_OnUploadTagAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UploadTagAttachmentCommand(TagIdWithAccessToProject, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadTagAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UploadTagAttachmentCommand(TagIdWithoutAccessToProject, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadTagAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UploadTagAttachmentCommand(TagIdWithAccessToProject, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadTagAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UploadTagAttachmentCommand(TagIdWithAccessToProject, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteFieldValueAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_OnDeleteFieldValueAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new DeleteFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteFieldValueAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new DeleteFieldValueAttachmentCommand(TagIdWithoutAccessToProject, 1, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteFieldValueAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new DeleteFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteFieldValueAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new DeleteFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteActionAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_OnDeleteActionAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new DeleteActionAttachmentCommand(TagIdWithAccessToProject, 1, 2, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteActionAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new DeleteActionAttachmentCommand(TagIdWithoutAccessToProject, 1, 2, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteActionAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new DeleteActionAttachmentCommand(TagIdWithAccessToProject, 1, 2, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnDeleteActionAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new DeleteActionAttachmentCommand(TagIdWithAccessToProject, 1, 2, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UploadActionAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_OnUploadActionAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UploadActionAttachmentCommand(TagIdWithAccessToProject, 1, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadActionAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UploadActionAttachmentCommand(TagIdWithoutAccessToProject, 1, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadActionAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UploadActionAttachmentCommand(TagIdWithAccessToProject, 1, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnUploadActionAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UploadActionAttachmentCommand(TagIdWithAccessToProject, 1, "F", true, new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region RequirementPreserveCommand
        [TestMethod]
        public async Task ValidateAsync_RequirementPreserveCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new RequirementPreserveCommand(TagIdWithAccessToProject, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RequirementPreserveCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new RequirementPreserveCommand(TagIdWithoutAccessToProject, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RequirementPreserveCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new RequirementPreserveCommand(TagIdWithAccessToProject, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RequirementPreserveCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new RequirementPreserveCommand(TagIdWithAccessToProject, 1);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region RecordValuesCommand
        [TestMethod]
        public async Task ValidateAsync_RecordValuesCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new RecordValuesCommand(TagIdWithAccessToProject, 1, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RecordValuesCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new RecordValuesCommand(TagIdWithoutAccessToProject, 1, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RecordValuesCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new RecordValuesCommand(TagIdWithAccessToProject, 1, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_RecordValuesCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new RecordValuesCommand(TagIdWithAccessToProject, 1, null, null, null);

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UploadFieldValueAttachmentCommand
        [TestMethod]
        public async Task ValidateAsync_UploadFieldValueAttachmentCommand_ShouldReturnTrue_WhenAccessToBothProjectAndContent()
        {
            // Arrange
            var command = new UploadFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1, "F", new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_UploadFieldValueAttachmentCommand_ShouldReturnFalse_WhenNoAccessToProject()
        {
            // Arrange
            var command = new UploadFieldValueAttachmentCommand(TagIdWithoutAccessToProject, 1, 1, "F", new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_UploadFieldValueAttachmentCommand_ShouldReturnFalse_WhenNoAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            var command = new UploadFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1, "F", new MemoryStream());

            // act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_UploadFieldValueAttachmentCommand_ShouldReturnTrue_WhenExplicitAccessToContent()
        {
            // Arrange
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitNoRestrictions()).Returns(false);
            _restrictionRolesCheckerMock.Setup(c => c.HasCurrentUserExplicitAccessToContent(RestrictedToContent)).Returns(true);
            var command = new UploadFieldValueAttachmentCommand(TagIdWithAccessToProject, 1, 1, "F", new MemoryStream());

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
        public async Task ValidateAsync_OnGetTagsForExportQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagsForExportQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagsForExportQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagsForExportQuery(ProjectWithoutAccess);
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
        public async Task ValidateAsync_OnGetActionsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetActionsQuery(TagIdWithAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetActionsQuery(TagIdWithoutAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagAttachmentsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagAttachmentsQuery(TagIdWithAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetGetTagAttachmentsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagAttachmentsQuery(TagIdWithoutAccessToProject);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagAttachmentQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetTagAttachmentQuery(TagIdWithAccessToProject, 1);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetGetTagAttachmentQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagAttachmentQuery(TagIdWithoutAccessToProject, 1);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionAttachmentsQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetActionAttachmentsQuery(TagIdWithAccessToProject, 1);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionAttachmentsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetActionAttachmentsQuery(TagIdWithoutAccessToProject, 1);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionAttachmentQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetActionAttachmentQuery(TagIdWithAccessToProject, 2, 1);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetActionAttachmentQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetActionAttachmentQuery(TagIdWithoutAccessToProject, 2, 1);
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
            var query = new GetTagRequirementsQuery(TagIdWithAccessToProject, false, false);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetTagRequirementsQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetTagRequirementsQuery(TagIdWithoutAccessToProject, false, false);
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

        [TestMethod]
        public async Task ValidateAsync_OnGetProjectByNameQuery_ShouldReturnTrue_WhenAccessToProject()
        {
            var query = new GetProjectByNameQuery(ProjectWithAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ValidateAsync_OnGetProjectByNameQuery_ShouldReturnFalse_WhenNoAccessToProject()
        {
            var query = new GetProjectByNameQuery(ProjectWithoutAccess);
            // act
            var result = await _dut.ValidateAsync(query);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
