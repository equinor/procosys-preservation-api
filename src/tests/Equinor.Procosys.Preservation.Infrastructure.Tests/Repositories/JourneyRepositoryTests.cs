using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class JourneyRepositoryTests : RepositoryTestBase
    {
        private const string TestJourney = "J1";
        private const int ModeId = 11;
        private const int StepId1 = 51;
        private const int StepId2 = 52;
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _dbSetMock;

        private JourneyRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(s => s.Id).Returns(ModeId);
            var stepMock1 = new Mock<Step>();
            stepMock1.SetupGet(s => s.Id).Returns(StepId1);
            var stepMock2 = new Mock<Step>(TestPlant, modeMock.Object, new Mock<Responsible>().Object);
            stepMock2.SetupGet(s => s.Id).Returns(StepId2);

            var journeyWithSteps = new Journey(TestPlant, TestJourney);
            journeyWithSteps.AddStep(new Mock<Step>().Object);
            journeyWithSteps.AddStep(stepMock1.Object);
            journeyWithSteps.AddStep(stepMock2.Object);
            
            _journeys = new List<Journey>
            {
                journeyWithSteps,
                new Journey(TestPlant, "J2"),
                new Journey(TestPlant, "J3")
            };
            
            _dbSetMock = _journeys.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Journeys)
                .Returns(_dbSetMock.Object);

            _dut = new JourneyRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetByTitle_KnownTitle_ReturnsJourneysWith3Steps()
        {
            var result = await _dut.GetByTitleAsync(TestJourney);

            Assert.AreEqual(TestJourney, result.Title);
            Assert.AreEqual(3, result.Steps.Count);
        }

        [TestMethod]
        public async Task GetByTitle_UnknownTitle_ReturnsNull()
        {
            var result = await _dut.GetByTitleAsync("XJ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetJourneyByStepId_KnownId_ReturnsJourneysWith3Steps()
        {
            var result = await _dut.GetJourneyByStepIdAsync(StepId1);

            Assert.AreEqual(TestJourney, result.Title);
            Assert.AreEqual(3, result.Steps.Count);
        }

        [TestMethod]
        public async Task GetJourneyByStepId_UnknownId_ReturnsNull()
        {
            var result = await _dut.GetJourneyByStepIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStepByStepId_KnownId_ReturnsStep()
        {
            var result = await _dut.GetStepByStepIdAsync(StepId1);

            Assert.AreEqual(StepId1, result.Id);
        }

        [TestMethod]
        public async Task GetStepsByStepIds_KnownIds_Returns2Steps()
        {
            var result = await _dut.GetStepsByStepIdsAsync(new List<int>{StepId1, StepId2});

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(s => s.Id == StepId1));
            Assert.IsTrue(result.Any(s => s.Id == StepId2));
        }

        [TestMethod]
        public async Task GetStepsByStepIds_UnKnownIds_ReturnsEmptyList()
        {
            var result = await _dut.GetStepsByStepIdsAsync(new List<int>{123512});

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetStepsByStepIds_NoIds_ReturnsEmptyList()
        {
            var result = await _dut.GetStepsByStepIdsAsync(new List<int>());

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetStepByStepId_UnknownId_ReturnsNull()
        {
            var result = await _dut.GetStepByStepIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStepsByModeId_KnownId_ReturnsStep()
        {
            var result = await _dut.GetStepsByModeIdAsync(ModeId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ModeId, result.First().ModeId);
        }
    }
}
