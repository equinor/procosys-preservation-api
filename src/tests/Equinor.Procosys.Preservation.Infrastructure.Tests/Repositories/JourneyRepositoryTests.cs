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
        private const int StepId = 51;
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _dbSetMock;

        private JourneyRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(s => s.Id).Returns(ModeId);
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Id).Returns(StepId);
            var step = new Step(TestPlant, modeMock.Object, new Mock<Responsible>().Object);
            
            var journeyWithSteps = new Journey(TestPlant, TestJourney);
            journeyWithSteps.AddStep(new Mock<Step>().Object);
            journeyWithSteps.AddStep(stepMock.Object);
            journeyWithSteps.AddStep(step);
            
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
        public async Task GetByTitle_KnownJourney_ReturnsJourneysWith3Steps()
        {
            var result = await _dut.GetByTitleAsync(TestJourney);

            Assert.AreEqual(TestJourney, result.Title);
            Assert.AreEqual(3, result.Steps.Count);
        }

        [TestMethod]
        public async Task GetByTitle_UnknownJourney_ReturnsNull()
        {
            var result = await _dut.GetByTitleAsync("XJ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetJourneyByStepId_KnownStepId_ReturnsJourneysWith3Steps()
        {
            var result = await _dut.GetJourneyByStepIdAsync(StepId);

            Assert.AreEqual(TestJourney, result.Title);
            Assert.AreEqual(3, result.Steps.Count);
        }

        [TestMethod]
        public async Task GetJourneyByStepId_UnknownStepId_ReturnsNull()
        {
            var result = await _dut.GetJourneyByStepIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStepByStepId_KnownStepInJourney_ReturnsStep()
        {
            var result = await _dut.GetStepByStepIdAsync(StepId);

            Assert.AreEqual(StepId, result.Id);
        }

        [TestMethod]
        public async Task GetStepByStepId_UnknownStepId_ReturnsNull()
        {
            var result = await _dut.GetStepByStepIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetStepsByModeId_KnownModeId_ReturnsStep()
        {
            var result = await _dut.GetStepsByModeIdAsync(ModeId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ModeId, result.First().ModeId);
        }
    }
}
