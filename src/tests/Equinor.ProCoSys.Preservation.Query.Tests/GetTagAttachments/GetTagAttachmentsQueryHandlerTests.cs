﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagAttachments;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagAttachments
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

                _attachment = new TagAttachment(TestPlant, Guid.NewGuid(), "FileA");
                tag.AddAttachment(_attachment);

                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnAttachments()
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
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
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
