using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionAttachmentCommands.Delete
{
    [TestClass]
    public class DeleteActionAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string BlobContainer = "bc";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private DeleteActionAttachmentCommand _command;
        private DeleteActionAttachmentCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBlobStorage> _blobStorageMock;
        private Action _action;

        [TestInitialize]
        public void Setup()
        {
            _command = new DeleteActionAttachmentCommand(1, 2, 3, _rowVersion);

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _blobStorageMock = new Mock<IBlobStorage>();

            var attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions
            {
                MaxSizeKb = 2,
                BlobContainer = BlobContainer,
                ValidFileSuffixes = new[] { ".gif", ".jpg" }
            };
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);

            _action = new Action(TestPlant, "T", "D", null);
            _action.SetProtectedIdForTesting(_command.ActionId);
            tagMock.Object.AddAction(_action);

            var attachment = new ActionAttachment(TestPlant, Guid.Empty, "Fil.txt");
            attachment.SetProtectedIdForTesting(_command.AttachmentId);
            _action.AddAttachment(attachment);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_command.TagId))
                .Returns(Task.FromResult(tagMock.Object));

            _dut = new DeleteActionAttachmentCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _blobStorageMock.Object,
                attachmentOptionsMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteActionAttachmentCommand_ShouldDeleteAttachmentFromAction_WhenExists()
        {
            // Arrange
            Assert.AreEqual(1, _action.Attachments.Count);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Unit.Value, result.Data);
            Assert.AreEqual(0, _action.Attachments.Count);
        }

        [TestMethod]
        public async Task HandlingDeleteActionAttachmentCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteActionAttachmentCommand_ShouldDeleteFromBlobStorage()
        {
            // Arrange
            var attachment = _action.Attachments.Single();
            var p = attachment.GetFullBlobPath(BlobContainer);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b => b.DeleteAsync(p, default), Times.Once);
        }
    }
}
