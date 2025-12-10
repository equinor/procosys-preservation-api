using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetActionAttachments;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionAttachments
{
    [TestClass]
    public class GetActionAttachmentsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _actionId;
        private readonly DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private TestDataSet _testDataSet;
        private ActionAttachment _attachment;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                var tag = _testDataSet.Project1.Tags.First();

                var action = new Action(TestPlant, "Open", "Desc1", _utcNow);
                tag.AddAction(action);

                _attachment = new ActionAttachment(TestPlant, new Guid("{C3412890-1EF8-4E34-B96C-5488200A5AF5}"), "FileA");
                action.AddAttachment(_attachment);

                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
                _actionId = action.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachments()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionAttachmentsQuery(_tagId, _actionId);
                var dut = new GetActionAttachmentsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var attachmentDtos = result.Data;
                Assert.AreEqual(1, attachmentDtos.Count);

                var attachment = attachmentDtos.Single();
                Assert.AreEqual(_attachment.Id, attachment.Id);
                Assert.AreEqual(_attachment.FileName, attachment.FileName);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionAttachmentsQuery(0, _actionId);
                var dut = new GetActionAttachmentsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_IfActionIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionAttachmentsQuery(_tagId, 0);
                var dut = new GetActionAttachmentsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
