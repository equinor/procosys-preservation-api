using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetJourneyById;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetJourneyById
{
    [TestClass]
    public class GetJourneyByIdQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);
                var step1 = _testDataSet.Journey1With2Steps.Steps.First();
                step1.Void();
                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldReturnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(_testDataSet.Journey1With2Steps.Id, true), default);

                var journey = result.Data;
                Assert.AreEqual(_testDataSet.Journey2With1Step.Title, journey.Title);

                var steps = journey.Steps.ToList();
                Assert.AreEqual(2, steps.Count);

                var step = steps.First();
                Assert.IsNotNull(step.Mode);
                Assert.IsNotNull(step.Responsible);
                Assert.AreEqual(_testDataSet.Mode1.Id, step.Mode.Id);
                Assert.AreEqual(_testDataSet.Responsible1.Id, step.Responsible.Id);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldReturnJourneyInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeyId = _testDataSet.Journey2With1Step.Id;

                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(journeyId), default);

                var journey = result.Data;
                Assert.IsTrue(journey.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_JourneyBeforeTagsAdded_ShouldReturnJourneyNotInUse()
        {
            int journeyId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                journeyId = AddJourneyWithStep(context, "J3", "Step1", _testDataSet.Mode1, _testDataSet.Responsible1).Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(journeyId), default);

                var journey = result.Data;
                Assert.IsFalse(journey.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldNotReturnVoidedStepsInJourneyWhenExcludedInRequest()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(_testDataSet.Journey1With2Steps.Id, false), default);

                var journey = result.Data;
                var steps = journey.Steps.ToList();
                Assert.AreEqual(1, steps.Count);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(1525, false), default);

                Assert.IsNull(result.Data);
            }
        }
    }
}
