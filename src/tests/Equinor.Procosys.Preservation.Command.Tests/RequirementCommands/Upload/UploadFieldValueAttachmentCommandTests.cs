using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.RequirementCommands.Upload;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.Upload
{
    [TestClass]
    public class UploadFieldValueAttachmentCommandTests : CommandHandlerTestsBase
    {
        private readonly string _fileName = "AttachmentFileName";
        private readonly string _blobContainer = "bc";
        private Mock<RequirementDefinition> _requirementDefinition;
        private TagRequirement _requirement;
        private UploadFieldValueAttachmentCommand _command;
        private UploadFieldValueAttachmentCommandHandler _dut;
        private Mock<IBlobStorage> _blobStorageMock;

        [TestInitialize]
        public void Setup()
        {
            var _tagId = 1;
            var _attachmentFieldId = 12;
            var _reqId = 21;

            _command = new UploadFieldValueAttachmentCommand(
                _tagId, 
                _reqId,
                _attachmentFieldId,
                _fileName,
                new MemoryStream());

            _requirementDefinition = new Mock<RequirementDefinition>();
            _requirementDefinition.SetupGet(r => r.Id).Returns(_reqId);
            _requirementDefinition.SetupGet(r => r.Plant).Returns(TestPlant);

            var attachmentFieldMock = new Mock<Field>(TestPlant, "", FieldType.Attachment, 0, "", false);
            attachmentFieldMock.SetupGet(f => f.Id).Returns(_attachmentFieldId);
            attachmentFieldMock.SetupGet(f => f.Plant).Returns(TestPlant);
            _requirementDefinition.Object.AddField(attachmentFieldMock.Object);

            var requirementMock = new Mock<TagRequirement>(TestPlant, 2, _requirementDefinition.Object);
            requirementMock.SetupGet(r => r.Id).Returns(_reqId);
            requirementMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _requirement = requirementMock.Object;

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement>
            {
                _requirement
            });

            tag.StartPreservation();
            Assert.AreEqual(PreservationStatus.Active, tag.Status);
            Assert.IsTrue(_requirement.HasActivePeriod);

            var _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(tag));

            var _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(_reqId))
                .Returns(Task.FromResult(_requirementDefinition.Object));
            
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

            _dut = new UploadFieldValueAttachmentCommandHandler(
                _projectRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _blobStorageMock.Object,
                attachmentOptionsMock.Object);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldMakeActivePeriodReadyToBePreserved()
        {
            // Assert setup
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(1, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldCreateAttachmentValueWithAttachment()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            var attachmentValue = _requirement.ActivePeriod.FieldValues.Single() as AttachmentValue;
            Assert.IsNotNull(attachmentValue);
            Assert.IsNotNull(attachmentValue.FieldValueAttachment);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldUploadToBlobStorageWithOverwrite()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            var attachmentValue = (AttachmentValue)_requirement.ActivePeriod.FieldValues.Single();
            var p = attachmentValue.FieldValueAttachment.GetFullBlobPath(_blobContainer);
            _blobStorageMock.Verify(b => b.UploadAsync(p, It.IsAny<Stream>(), true, default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldBothDeleteAndUploadToBlobStorage_WhenAttachmentExistsInAdvance()
        {
            // Arrange
            _requirement.RecordAttachment(new FieldValueAttachment(TestPlant, Guid.Empty, "F"), _command.FieldId, _requirementDefinition.Object);
            var attachmentValue = (AttachmentValue)_requirement.ActivePeriod.FieldValues.Single();
            var p = attachmentValue.FieldValueAttachment.GetFullBlobPath(_blobContainer);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b => b.DeleteAsync(p, default), Times.Once);
            _blobStorageMock.Verify(b => b.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), true, default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldNotTryToDeleteFromBlobStorage_WhenAttachmentNotExistsInAdvance()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            _blobStorageMock.Verify(b => b.DeleteAsync(It.IsAny<string>(), default), Times.Never);
        }

        [TestMethod]
        public async Task HandlingUploadFieldValueAttachmentCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
