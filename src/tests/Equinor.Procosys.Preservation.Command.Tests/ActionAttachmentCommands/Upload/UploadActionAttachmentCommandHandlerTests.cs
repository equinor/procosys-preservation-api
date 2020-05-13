using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionAttachmentCommands.Upload
{
    [TestClass]
    public class UploadActionAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 2;
        private const int ActionId = 3;
        private const string FileName = "AttachmentFileName";
        private const string BlobContainer = "bc";

        private UploadActionAttachmentCommand _commandWithoutOverwrite;
        private UploadActionAttachmentCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBlobStorage> _blobStorageMock;
        private Mock<Action> _actionMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _blobStorageMock = new Mock<IBlobStorage>();

            var attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions
            {
                MaxSizeKb = 2,
                BlobContainer = BlobContainer,
                ValidFileSuffixes = new[] {".gif", ".jpg"}
            };
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            tagMock.SetupGet(t => t.Id).Returns(TagId);
            _actionMock = new Mock<Action>();
            _actionMock.SetupGet(t => t.Plant).Returns(TestPlant);
            _actionMock.SetupGet(t => t.Id).Returns(ActionId);
            tagMock.Object.AddAction(_actionMock.Object);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(TagId))
                .Returns(Task.FromResult(tagMock.Object));

            _commandWithoutOverwrite = new UploadActionAttachmentCommand(TagId, ActionId, FileName, false, new MemoryStream());

            _dut = new UploadActionAttachmentCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _blobStorageMock.Object,
                attachmentOptionsMock.Object);
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldAddAttachmentToTag_WhenNotExist()
        {
            // Arrange
            Assert.IsTrue(_actionMock.Object.Attachments.Count == 0);

            // Act
            var result = await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_actionMock.Object.Attachments.Count == 1);
            var attachment = _actionMock.Object.Attachments.Single();
            Assert.AreEqual(FileName, attachment.FileName);
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldThrowException_WhenExistAndNotOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_actionMock.Object.Attachments.Count == 1);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _dut.Handle(_commandWithoutOverwrite, default));
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldUpdateExistingAttachment_WhenExistAndOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_actionMock.Object.Attachments.Count == 1);
            var commandWithOverwrite = new UploadActionAttachmentCommand(TagId, ActionId, FileName, true, new MemoryStream());

            // Act
            var result = await _dut.Handle(commandWithOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_actionMock.Object.Attachments.Count == 1);
            var attachment = _actionMock.Object.Attachments.Single();
            Assert.AreEqual(FileName, attachment.FileName);
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldUploadToBlobStorage()
        {
            // Act
            await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            var attachment = _actionMock.Object.Attachments.Single();
            var p = attachment.GetFullBlobPath(BlobContainer);
            _blobStorageMock.Verify(b 
                => b.UploadAsync(p, It.IsAny<Stream>(), _commandWithoutOverwrite.OverwriteIfExists, default), Times.Once);
        }
    }
}
