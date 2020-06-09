using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagAttachmentCommands.Delete
{
    [TestClass]
    public class DeleteTagAttachmentCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string BlobContainer = "bc";
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private DeleteTagAttachmentCommand _command;
        private DeleteTagAttachmentCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBlobStorage> _blobStorageMock;
        private Tag _tag;

        [TestInitialize]
        public void Setup()
        {
            _command = new DeleteTagAttachmentCommand(1, 3, _rowVersion, TestUserOid);

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

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var reqMock = new Mock<TagRequirement>();
            reqMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> { reqMock.Object });

            var attachment = new TagAttachment(TestPlant, Guid.Empty, "Fil.txt");
            attachment.SetProtectedIdForTesting(_command.AttachmentId);
            _tag.AddAttachment(attachment);

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_command.TagId))
                .Returns(Task.FromResult(_tag));

            _dut = new DeleteTagAttachmentCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _blobStorageMock.Object,
                attachmentOptionsMock.Object);
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
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(_command.CurrentUserOid, default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteTagAttachmentCommand_ShouldDeleteFromBlobStorage()
        {
            // Arrange
            var attachment = _tag.Attachments.Single();
            var p = attachment.GetFullBlobPath(BlobContainer);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b  => b.DeleteAsync(p, default), Times.Once);
        }
    }
}
