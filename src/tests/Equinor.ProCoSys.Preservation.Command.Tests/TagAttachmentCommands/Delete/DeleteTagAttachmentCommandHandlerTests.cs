using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Delete;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagAttachmentCommands.Delete
{
    [TestClass]
    public class DeleteTagAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string BlobContainer = "bc";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private DeleteTagAttachmentCommand _command;
        private DeleteTagAttachmentCommandHandler _dut;

        private Mock<IAzureBlobService> _blobStorageMock;
        private Tag _tag;

        [TestInitialize]
        public void Setup()
        {
            _command = new DeleteTagAttachmentCommand(1, 3, _rowVersion);

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

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var reqMock = new Mock<TagRequirement>();
            reqMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", stepMock.Object, new List<TagRequirement> { reqMock.Object });

            var attachment = new TagAttachment(TestPlant, Guid.Empty, "Fil.txt");
            attachment.SetProtectedIdForTesting(_command.AttachmentId);
            _tag.AddAttachment(attachment);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(r => r.GetTagWithAttachmentsByTagIdAsync(_command.TagId))
                .Returns(Task.FromResult(_tag));

            _dut = new DeleteTagAttachmentCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _blobStorageMock.Object,
                blobStorageOptionsMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteTagAttachmentCommand_ShouldDeleteAttachmentFromTag_WhenExists()
        {
            // Arrange
            Assert.AreEqual(1, _tag.Attachments.Count);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Unit.Value, result.Data);
            Assert.AreEqual(0, _tag.Attachments.Count);
        }

        [TestMethod]
        public async Task HandlingDeleteTagAttachmentCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteTagAttachmentCommand_ShouldDeleteFromBlobStorage()
        {
            // Arrange
            var attachment = _tag.Attachments.Single();
            var p = attachment.GetFullBlobPath();

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b  => b.DeleteAsync(BlobContainer, p, default), Times.Once);
        }
    }
}
