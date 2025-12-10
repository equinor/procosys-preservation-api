using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetHistoricalFieldValueAttachment;
using Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetHistoricalFieldValueAttachment
{
    [TestClass]
    public class GetHistoricalFieldValueAttachmentQueryHandlerTests : ReadOnlyTestsBase
    {
        private Mock<IAzureBlobService> _blobStorageMock;
        private Mock<IUserDelegationProvider> _userDelegationProviderMock;
        private Uri _uri;
        private string BlobContainer = "bc";
        private Mock<IOptionsSnapshot<BlobStorageOptions>> _blobStorageOptionsMock;

        private int _requirementIdWithAttachment;
        private int _requirementIdWithCheckbox;
        private int _tagId;
        private int _attachmentFieldId;
        private int _checkboxFieldId;
        private Guid _preservationRecordGuidAttachment;
        private Guid _preservationRecordGuidCheckbox;

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

            var reqDef2 = AddRequirementTypeWith1DefWithoutField(context, "T2", "D2", RequirementTypeIcon.Other).RequirementDefinitions.Single();
            var checkboxField = new Field(TestPlant, "Label Checkbox", FieldType.CheckBox, 0);
            reqDef2.AddField(checkboxField);
            context.SaveChangesAsync().Wait();

            var requirement = new TagRequirement(TestPlant, 2, reqDef);
            var requirement2 = new TagRequirement(TestPlant, 4, reqDef2);

            var tag = new Tag(TestPlant,
                TagType.Standard,
                Guid.NewGuid(),
                "TagNo",
                "Description",
                journey.Steps.ElementAt(0),
                new List<TagRequirement> { requirement, requirement2 });

            context.Tags.Add(tag);
            Assert.IsNull(requirement.ActivePeriod);
            Assert.IsNull(requirement2.ActivePeriod);

            tag.StartPreservation();
            context.SaveChangesAsync().Wait();
            Assert.IsNotNull(requirement.ActivePeriod);
            Assert.IsNotNull(requirement2.ActivePeriod);

            var activePeriodForRequirementWithCheckboxField =
                tag.Requirements.Single(r => r.Id == requirement2.Id).ActivePeriod;
            var activePeriodForRequirementWithAttachmentField =
                tag.Requirements.Single(r => r.Id == requirement.Id).ActivePeriod;

            Assert.IsNull(activePeriodForRequirementWithAttachmentField.PreservationRecord);
            Assert.IsNull(activePeriodForRequirementWithCheckboxField.PreservationRecord);

            _checkboxFieldId = checkboxField.Id;
            requirement2.RecordCheckBoxValues(
                new Dictionary<int, bool>
                {
                    {_checkboxFieldId, true}
                },
                reqDef2);

            _attachmentFieldId = attachmentField.Id;
            var fieldValueAttachment = new FieldValueAttachment(TestPlant, Guid.Empty, "FilA.txt");
            requirement.RecordAttachment(fieldValueAttachment, _attachmentFieldId, reqDef);

            context.SaveChangesAsync().Wait();

            tag.Preserve(new Mock<Person>().Object, requirement2.Id);
            tag.Preserve(new Mock<Person>().Object, requirement.Id);
            context.SaveChangesAsync().Wait();

            _tagId = tag.Id;
            _requirementIdWithAttachment = requirement.Id;
            _requirementIdWithCheckbox = requirement2.Id;

            var fullBlobPath = fieldValueAttachment.GetFullBlobPath();
            _blobStorageMock = new Mock<IAzureBlobService>();
            _blobStorageMock
                .Setup(b => b.GetDownloadSasUri(
                    BlobContainer,
                    fullBlobPath,
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<UserDelegationKey>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(_uri);

            _userDelegationProviderMock = new Mock<IUserDelegationProvider>();
            _userDelegationProviderMock.Setup(u => u.GetUserDelegationKey()).Returns(new Mock<UserDelegationKey>().Object);

            Assert.IsNotNull(activePeriodForRequirementWithAttachmentField.PreservationRecord);
            Assert.IsNotNull(activePeriodForRequirementWithCheckboxField.PreservationRecord);
            context.SaveChangesAsync().Wait();

            _preservationRecordGuidCheckbox = activePeriodForRequirementWithCheckboxField.PreservationRecord.Guid;
            _preservationRecordGuidAttachment = activePeriodForRequirementWithAttachmentField.PreservationRecord.Guid;
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachmentUri()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetHistoricalFieldValueAttachmentQuery(_tagId, _requirementIdWithAttachment, _preservationRecordGuidAttachment);
            var dut = new GetHistoricalFieldValueAttachmentQueryHandler(
                context,
                _blobStorageMock.Object,
                _userDelegationProviderMock.Object,
                _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.Ok, result.ResultType);
            Assert.AreEqual(result.Data, _uri);
        }

        [TestMethod]
        public async Task Handler_NotAttachmentFieldType()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetHistoricalFieldValueAttachmentQuery(_tagId, _requirementIdWithCheckbox, _preservationRecordGuidCheckbox);
            var dut = new GetHistoricalFieldValueAttachmentQueryHandler(
                context,
                _blobStorageMock.Object,
                _userDelegationProviderMock.Object,
                _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].StartsWith($"{nameof(FieldValue)} with type attachment was not found"));
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task Handler_NoPreservationPeriod_InvalidGuid()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetHistoricalFieldValueAttachmentQuery(_tagId, _requirementIdWithAttachment, new Guid());
            var dut = new GetHistoricalFieldValueAttachmentQueryHandler(
                context,
                _blobStorageMock.Object,
                _userDelegationProviderMock.Object,
                _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].StartsWith($"{nameof(PreservationPeriod)} not found"));
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetHistoricalFieldValueAttachmentQuery(0, _requirementIdWithAttachment, _preservationRecordGuidAttachment);
            var dut = new GetHistoricalFieldValueAttachmentQueryHandler(
                context,
                _blobStorageMock.Object,
                _userDelegationProviderMock.Object,
                _blobStorageOptionsMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}
