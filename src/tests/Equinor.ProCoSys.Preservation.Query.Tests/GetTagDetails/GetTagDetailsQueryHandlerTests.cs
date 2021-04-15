using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagDetails;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagDetails
{
    [TestClass]
    public class GetTagDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _testTagId;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);
                _testTagId = _testDataSet.Project1.Tags.First().Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnCorrectTagDetails_AfterPreservationStarted()
        {
            Tag tag;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _testTagId);
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

                var query = new GetTagDetailsQuery(_testTagId);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                var step = context.Steps.Single(s => s.Id == tag.StepId);
                var mode = context.Modes.Single(m => m.Id == step.ModeId);
                var resp = context.Responsibles.Single(r => r.Id == step.ResponsibleId);
                var journey = context.Journeys.Single(j => j.Steps.Any(s => s.Id == step.Id));
                Assert.IsTrue(dto.IsInUse);
                Assert.AreEqual(tag.AreaCode, dto.AreaCode);
                Assert.AreEqual(tag.DisciplineCode, dto.DisciplineCode);
                Assert.AreEqual(tag.Calloff, dto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, dto.CommPkgNo);
                Assert.AreEqual(tag.Description, dto.Description);
                Assert.AreEqual(tag.Id, dto.Id);
                Assert.AreEqual(tag.McPkgNo, dto.McPkgNo);
                Assert.AreEqual(tag.PurchaseOrderNo, dto.PurchaseOrderNo);
                Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), dto.Status);
                Assert.AreEqual(tag.TagNo, dto.TagNo);
                Assert.AreEqual(tag.TagType, dto.TagType);
                Assert.AreEqual(tag.IsReadyToBePreserved(), dto.ReadyToBePreserved);
                Assert.IsNotNull(dto.Journey.Title);
                Assert.AreEqual(journey.Title, dto.Journey.Title);
                Assert.IsNotNull(dto.Step);
                Assert.AreEqual(step.Title, dto.Step.Title);
                Assert.IsNotNull(dto.Mode);
                Assert.AreEqual(mode.Title, dto.Mode.Title);
                Assert.IsNotNull(dto.Responsible);
                Assert.AreEqual(resp.Code, dto.Responsible.Code);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnIsInUseFalse_BeforePreservationStartedAndNoAttachmentsOrActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagDetailsQuery(_testTagId);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.IsFalse(dto.IsInUse);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnIsInUseTrue_BeforePreservationStartedButAttachmentExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _testTagId);
                tag.AddAttachment(new TagAttachment(TestPlant, Guid.Empty, "File"));
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagDetailsQuery(_testTagId);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.IsTrue(dto.IsInUse);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnIsInUseTrue_BeforePreservationStartedButActionExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _testTagId);
                tag.AddAction(new Action(TestPlant, "A", "D", null));
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagDetailsQuery(_testTagId);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.IsTrue(dto.IsInUse);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagDetailsQuery(0);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
