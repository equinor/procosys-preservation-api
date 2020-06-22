using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.Services;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetJourneyById;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.GetJourneyById
{
    [TestClass]
    public class GetJourneyByIdQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private Mock<IJourneyService> _journeyServiceMock;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            _journeyServiceMock = new Mock<IJourneyService>();

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
                var dut = new GetJourneyByIdQueryHandler(context, _journeyServiceMock.Object);
                var result = await dut.Handle(new GetJourneyByIdQuery(_testDataSet.Journey2With1Step.Id), default);

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
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldReturnJourneyInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeyId = _testDataSet.Journey2With1Step.Id;
                _journeyServiceMock.Setup(j => j.IsJourneyInUseAsync(journeyId, default)).Returns(Task.FromResult(true));

                var dut = new GetJourneyByIdQueryHandler(context, _journeyServiceMock.Object);
                var result = await dut.Handle(new GetJourneyByIdQuery(journeyId), default);

                var journey = result.Data;
                Assert.IsTrue(journey.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetJourneyByIdQueryHandler(context, _journeyServiceMock.Object);
                var result = await dut.Handle(new GetJourneyByIdQuery(1525), default);

                Assert.IsNull(result.Data);
            }
        }
    }
}
