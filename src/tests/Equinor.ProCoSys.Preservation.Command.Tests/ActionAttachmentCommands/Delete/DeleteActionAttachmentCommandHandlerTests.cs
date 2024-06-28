using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionAttachmentCommands.Delete
{
    [TestClass]
    public class DeleteActionAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string BlobContainer = "bc";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private DeleteActionAttachmentCommand _command;
        private DeleteActionAttachmentCommandHandler _dut;

        private Mock<IAzureBlobService> _blobStorageMock;
        private Action _action;

        [TestInitialize]
        public void Setup()
        {
            _command = new DeleteActionAttachmentCommand(1, 2, 3, _rowVersion);

            _blobStorageMock = new Mock<IAzureBlobService>();

            var blobStorageOptionsMock = new Mock<IOptionsSnapshot<BlobStorageOptions>>();
            var options = new BlobStorageOptions
            {
                MaxSizeMb = 2,
                BlobContainer = BlobContainer,
                BlockedFileSuffixes = new[] {".exe", ".zip"}
            };
            blobStorageOptionsMock
                .Setup(x => x.Value)
                .Returns(options);

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);

            _action = new Action(Guid.Empty, TestPlant, "T", "D", null);
            _action.SetProtectedIdForTesting(_command.ActionId);
            tagMock.Object.AddAction(_action);

            var attachment = new ActionAttachment(TestPlant, Guid.Empty, "Fil.txt");
            attachment.SetProtectedIdForTesting(_command.AttachmentId);
            _action.AddAttachment(attachment);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(r => r.GetTagWithActionsByTagIdAsync(_command.TagId))
                .Returns(Task.FromResult(tagMock.Object));

            _dut = new DeleteActionAttachmentCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _blobStorageMock.Object,
                blobStorageOptionsMock.Object);
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
            var p = attachment.GetFullBlobPath();

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b => b.DeleteAsync(BlobContainer, p, default), Times.Once);
        }
    }
}
