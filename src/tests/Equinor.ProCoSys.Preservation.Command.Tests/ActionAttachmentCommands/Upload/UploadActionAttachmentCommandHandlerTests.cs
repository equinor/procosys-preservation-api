using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionAttachmentCommands.Upload
{
    [TestClass]
    public class UploadActionAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _tagId = 2;
        private readonly int _actionId = 12;
        private readonly string _fileName = "AttachmentFileName";
        private readonly string _blobContainer = "bc";

        private UploadActionAttachmentCommand _commandWithoutOverwrite;
        private UploadActionAttachmentCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBlobStorage> _blobStorageMock;
        private Action _action;

        [TestInitialize]
        public void Setup()
        {
            _commandWithoutOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, false, new MemoryStream());

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _blobStorageMock = new Mock<IBlobStorage>();

            var attachmentOptionsMock = new Mock<IOptionsMonitor<AttachmentOptions>>();
            var options = new AttachmentOptions
            {
                MaxSizeMb = 2,
                BlobContainer = _blobContainer,
                ValidFileSuffixes = new[] {".gif", ".jpg"}
            };
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            var tagMock = new Mock<Tag>();
            tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            _action = new Action(TestPlant, "T", "D", null);
            _action.SetProtectedIdForTesting(_commandWithoutOverwrite.ActionId);
            tagMock.Object.AddAction(_action);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(tagMock.Object));

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
            Assert.IsTrue(_action.Attachments.Count == 0);

            // Act
            var result = await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_action.Attachments.Count == 1);
            var attachment = _action.Attachments.Single();
            Assert.AreEqual(_fileName, attachment.FileName);
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldThrowException_WhenExistAndNotOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_action.Attachments.Count == 1);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _dut.Handle(_commandWithoutOverwrite, default));
        }

        [TestMethod]
        public async Task HandlingUploadActionAttachmentCommand_ShouldUpdateExistingAttachment_WhenExistAndOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_action.Attachments.Count == 1);
            var commandWithOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, true, new MemoryStream());

            // Act
            var result = await _dut.Handle(commandWithOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_action.Attachments.Count == 1);
            var attachment = _action.Attachments.Single();
            Assert.AreEqual(_fileName, attachment.FileName);
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
            var attachment = _action.Attachments.Single();
            var p = attachment.GetFullBlobPath(_blobContainer);
            _blobStorageMock.Verify(b 
                => b.UploadAsync(p, It.IsAny<Stream>(), _commandWithoutOverwrite.OverwriteIfExists, default), Times.Once);
        }
    }
}
