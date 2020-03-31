using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
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
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldReturnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(TestPlant, _testDataSet.Journey2With1Step.Id), default);

                var journey = result.Data;

                Assert.AreEqual(_testDataSet.Journey2With1Step.Title, journey.Title);

                var steps = journey.Steps.ToList();
                Assert.AreEqual(1, steps.Count);

                var step = steps.First();
                Assert.IsNotNull(step.Mode);
                Assert.IsNotNull(step.Responsible);
                Assert.AreEqual(_testDataSet.Mode1.Id, step.Mode.Id);
                Assert.AreEqual(_testDataSet.Responsible1.Id, step.Responsible.Id);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context);
                var result = await dut.Handle(new GetJourneyByIdQuery(TestPlant, 1525), default);

                Assert.IsNull(result.Data);
            }
        }
    }
}
