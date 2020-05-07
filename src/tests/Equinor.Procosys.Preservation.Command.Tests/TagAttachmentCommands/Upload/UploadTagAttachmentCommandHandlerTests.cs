using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 2;
        private const string FileName = "AttachmentFileName";
        private const string Path = "AttachmentPath";

        private UploadTagAttachmentCommand _commandWithoutOverwrite;
        private UploadTagAttachmentCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBlobStorage> _blobStorageMock;
        private Mock<Tag> _tagMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _blobStorageMock = new Mock<IBlobStorage>();

            _tagMock = new Mock<Tag>();
            _tagMock.SetupGet(t => t.Plant).Returns(TestPlant);
            _tagMock.SetupGet(t => t.Id).Returns(TagId);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(TagId))
                .Returns(Task.FromResult(_tagMock.Object));

            var blobPathProviderMock = new Mock<IBlobPathProvider>();
            blobPathProviderMock.Setup(b => b.CreateFullBlobPathForAttachment(It.IsAny<Attachment>()))
                .Returns(Path);

            _commandWithoutOverwrite = new UploadTagAttachmentCommand(TagId, FileName, false, new MemoryStream());

            _dut = new UploadTagAttachmentCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _blobStorageMock.Object,
                blobPathProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingUploadTagAttachmentCommand_ShouldAddAttachmentToTag_WhenNotExist()
        {
            // Arrange
            Assert.IsTrue(_tagMock.Object.Attachments.Count == 0);

            // Act
            var result = await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_tagMock.Object.Attachments.Count == 1);
            var attachment = _tagMock.Object.Attachments.Single();
            Assert.AreEqual(FileName, attachment.FileName);
        }

        [TestMethod]
        public async Task HandlingUploadTagAttachmentCommand_ShouldThrowException_WhenExistAndNotOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_tagMock.Object.Attachments.Count == 1);

            // Act and Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _dut.Handle(_commandWithoutOverwrite, default));
        }

        [TestMethod]
        public async Task HandlingUploadTagAttachmentCommand_ShouldUpdateExistingAttachment_WhenExistAndOverWrite()
        {
            // Arrange
            await _dut.Handle(_commandWithoutOverwrite, default);
            Assert.IsTrue(_tagMock.Object.Attachments.Count == 1);
            var commandWithOverwrite = new UploadTagAttachmentCommand(TagId, FileName, true, new MemoryStream());

            // Act
            var result = await _dut.Handle(commandWithOverwrite, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.IsTrue(_tagMock.Object.Attachments.Count == 1);
            var attachment = _tagMock.Object.Attachments.Single();
            Assert.AreEqual(FileName, attachment.FileName);
        }

        [TestMethod]
        public async Task HandlingUploadTagAttachmentCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUploadTagAttachmentCommand_ShouldUploadToBlobStorage()
        {
            // Act
            await _dut.Handle(_commandWithoutOverwrite, default);

            // Assert
            _blobStorageMock.Verify(b 
                => b.UploadAsync(Path, It.IsAny<Stream>(), _commandWithoutOverwrite.OverwriteIfExists, default), Times.Once);
        }
    }
}
