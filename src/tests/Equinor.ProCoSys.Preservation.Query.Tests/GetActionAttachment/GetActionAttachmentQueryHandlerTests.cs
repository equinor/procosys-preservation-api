using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetActionAttachment;
using Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionAttachment
{
    [TestClass]
    public class GetActionAttachmentQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _actionId;
        private int _attachmentId;
        private readonly DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private ActionAttachment _attachment;
        private TestDataSet _testDataSet;
        private Mock<IAzureBlobService> _blobStorageMock;
        private Mock<IUserDelegationProvider> _userDelegationProviderMock;
        private Uri _uri;
        private Mock<IOptionsSnapshot<BlobStorageOptions>> _blobStorageOptionsMock;
        private string _blobContainer = "bc";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            _blobStorageMock = new Mock<IAzureBlobService>();
            _uri = new Uri("http://whatever/file.txt");
            _blobStorageOptionsMock = new Mock<IOptionsSnapshot<BlobStorageOptions>>();
            var options = new BlobStorageOptions
            {
                BlobContainer = _blobContainer
            };

            _blobStorageOptionsMock
                .Setup(x => x.Value)
                .Returns(options);

            using var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            _testDataSet = AddTestDataSet(context);

            var tag = _testDataSet.Project1.Tags.First();
            var action = new Action(TestPlant, "Open", "Desc1", _utcNow);
            tag.AddAction(action);

            _attachment = new ActionAttachment(TestPlant, Guid.NewGuid(), "FileA");
            action.AddAttachment(_attachment);

            context.SaveChangesAsync().Wait();

            _tagId = tag.Id;
            _actionId = action.Id;
            _attachmentId = _attachment.Id;

            var fullBlobPath = _attachment.GetFullBlobPath();
            _blobStorageMock
                .Setup(b => b.GetDownloadSasUri(
                    _blobContainer,
                    fullBlobPath,
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<UserDelegationKey>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(_uri);

            _userDelegationProviderMock = new Mock<IUserDelegationProvider>();
            _userDelegationProviderMock.Setup(u => u.GetUserDelegationKey()).Returns(new Mock<UserDelegationKey>().Object);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachmentUri()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetActionAttachmentQuery(_tagId, _actionId, _attachmentId);
            var dut = new GetActionAttachmentQueryHandler(
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
        public async Task Handler_ShouldReturnNotFound_IfActionIsNotFound()
        {
            await using var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

            var query = new GetActionAttachmentQuery(_tagId, _actionId, 0);
            var dut = new GetActionAttachmentQueryHandler(
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
