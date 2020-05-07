using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagAttachments;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagAttachments
{
    [TestClass]
    public class GetTagAttachmentsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private TagAttachment _attachment;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                var tag = _testDataSet.Project1.Tags.First();

                _attachment = new TagAttachment(TestPlant, new Guid("{C3412890-1EF8-4E34-B96C-5488200A5AF5}"), "FileA");
                tag.AddAttachment(_attachment);

                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsAttachments()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagAttachmentsQuery(_tagId);
                var dut = new GetTagAttachmentsQueryHandler(context);

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
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagAttachmentsQuery(0);
                var dut = new GetTagAttachmentsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
