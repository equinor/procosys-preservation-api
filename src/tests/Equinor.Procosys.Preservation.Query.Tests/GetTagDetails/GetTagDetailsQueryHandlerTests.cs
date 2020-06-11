using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagDetails
{
    [TestClass]
    public class GetTagDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private Tag _testTag;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _testTag = _testDataSet.Project1.Tags.First();

                _testTag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsTagDetails()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

                var query = new GetTagDetailsQuery(_testTag.Id);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                var step = context.Steps.Single(s => s.Id == _testTag.StepId);
                var mode = context.Modes.Single(m => m.Id == step.ModeId);
                Assert.AreEqual(_testTag.AreaCode, dto.AreaCode);
                Assert.AreEqual(_testTag.CommPkgNo, dto.CommPkgNo);
                Assert.AreEqual(_testTag.Description, dto.Description);
                Assert.AreEqual(_testTag.Id, dto.Id);
                Assert.AreEqual(_testTag.McPkgNo, dto.McPkgNo);
                Assert.AreEqual(_testTag.PurchaseOrderNo, dto.PurchaseOrderNo);
                Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), dto.Status);
                Assert.AreEqual(_testTag.TagNo, dto.TagNo);
                Assert.AreEqual(_testTag.TagType, dto.TagType);
                Assert.AreEqual(_testTag.IsReadyToBePreserved(), dto.ReadyToBePreserved);

                var resp = context.Responsibles.Single(r => r.Id == step.ResponsibleId);
                var journey = context.Journeys.Single(j => j.Steps.Any(s => s.Id == step.Id));
                Assert.AreEqual(journey.Title, dto.JourneyTitle);
                Assert.AreEqual(mode.Title, dto.Mode);
                Assert.AreEqual(resp.Code, dto.ResponsibleName);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
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
