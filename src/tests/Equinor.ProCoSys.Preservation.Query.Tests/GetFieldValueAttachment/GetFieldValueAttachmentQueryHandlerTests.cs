using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetFieldValueAttachment;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetFieldValueAttachment
{
    [TestClass]
    public class GetFieldValueAttachmentQueryHandlerTests : ReadOnlyTestsBase
    {
        private Mock<IBlobStorage> _blobStorageMock;
        private Uri _uri;
        private string BlobContainer = "bc";
        private Mock<IOptionsSnapshot<BlobStorageOptions>> _blobStorageOptionsMock;

        private int _requirementId;
        private int _tagId;
        private int _attachmentFieldId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            _uri = new Uri("http://whatever/file.txt");
            _blobStorageOptionsMock = new Mock<IOptionsSnapshot<BlobStorageOptions>>();
            var options = new BlobStorageOptions
            {
                BlobContainer = BlobContainer
            };

            _blobStorageOptionsMock
                .Setup(x => x.Value)
                .Returns(options);

            var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));

            var reqDef = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1", RequirementTypeIcon.Other).RequirementDefinitions.Single();
            var attachmentField = new Field(TestPlant, "Label", FieldType.Attachment, 0);
            reqDef.AddField(attachmentField);
            context.SaveChangesAsync().Wait();

            var requirement = new TagRequirement(TestPlant, 2, reqDef);

            var tag = new Tag(TestPlant,
                TagType.Standard,
                "TagNo",
                "Description",
                journey.Steps.ElementAt(0),
                new List<TagRequirement> {requirement});
                
            context.Tags.Add(tag);
            tag.StartPreservation();
            context.SaveChangesAsync().Wait();

            _tagId = tag.Id;

            _requirementId = requirement.Id;
            _attachmentFieldId = attachmentField.Id;

            var fieldValueAttachment = new FieldValueAttachment(TestPlant, Guid.Empty, "FilA.txt");
            requirement.RecordAttachment(fieldValueAttachment, _attachmentFieldId, reqDef);
            context.SaveChangesAsync().Wait();

            var fullBlobPath = fieldValueAttachment.GetFullBlobPath(BlobContainer);
            _blobStorageMock = new Mock<IBlobStorage>();
            _blobStorageMock
                .Setup(b => b.GetDownloadSasUri(fullBlobPath, It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
                .Returns(_uri);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachmentUri()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetFieldValueAttachmentQuery(_tagId, _requirementId, _attachmentFieldId);
            var dut = new GetFieldValueAttachmentQueryHandler(context, _blobStorageMock.Object, _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.Ok, result.ResultType);
                
            Assert.AreEqual(result.Data, _uri);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetFieldValueAttachmentQuery(0, _requirementId, _attachmentFieldId);
            var dut = new GetFieldValueAttachmentQueryHandler(context, _blobStorageMock.Object, _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}
