using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
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
        private int _step1Id;
        private int _step2Id;
        private readonly AutoTransferMethod _autoTransferMethodOnStep1 = AutoTransferMethod.OnRfccSign;
        private readonly AutoTransferMethod _autoTransferMethodOnStep2 = AutoTransferMethod.OnRfocSign;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);
                var step1 = _testDataSet.Journey1With2Steps.Steps.First();
                step1.AutoTransferMethod = _autoTransferMethodOnStep1;
                step1.IsVoided = true;
                _step1Id = step1.Id;
                var step2 = _testDataSet.Journey1With2Steps.Steps.Last();
                step2.AutoTransferMethod = _autoTransferMethodOnStep2;
                _step2Id = step2.Id;
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
                Assert.AreEqual(_testDataSet.Journey1With2Steps.Title, journey.Title);

                var steps = journey.Steps.ToList();
                Assert.AreEqual(2, steps.Count);

                var step = steps.Single(s => s.Id == _step1Id);
                Assert.IsNotNull(step.Mode);
                Assert.IsNotNull(step.Responsible);
                Assert.AreEqual(_testDataSet.Mode1.Id, step.Mode.Id);
                Assert.AreEqual(_testDataSet.Responsible1.Id, step.Responsible.Id);
                Assert.AreEqual(_autoTransferMethodOnStep1, step.AutoTransferMethod);

                step = steps.Single(s => s.Id == _step2Id);
                Assert.AreEqual(_autoTransferMethodOnStep2, step.AutoTransferMethod);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_JourneyWithSteps_ShouldReturnJourneyInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeyId = _testDataSet.Journey2With1Step.Id;

                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(journeyId, true), default);

                var journey = result.Data;
                Assert.IsTrue(journey.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_BeforeTagsAddedToStep_ShouldReturnNoStepsInUse()
        {
            int journeyId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                journeyId = AddJourneyWithStep(context, "J3", "Step1", _testDataSet.Mode1, _testDataSet.Responsible1).Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(journeyId, true), default);

                var journey = result.Data;
                Assert.IsFalse(journey.Steps.Any(s => s.IsInUse));
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_AfterTagsAddedToAStep_ShouldReturnAllStepsInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(_testDataSet.Journey1With2Steps.Id, true), default);

                var journey = result.Data;
                Assert.IsTrue(journey.Steps.All(s => s.IsInUse));
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
