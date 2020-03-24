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
        private const int StepId = 51;
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _dbSetMock;

        private JourneyRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            
            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Plant).Returns(TestPlant);

            var stepMock1 = new Mock<Step>();
            stepMock1.SetupGet(s => s.Id).Returns(StepId);
            stepMock1.SetupGet(s => s.Plant).Returns(TestPlant);

            var journey = new Journey(TestPlant, "J");

            journey.AddStep(stepMock1.Object);
            
            _journeys = new List<Journey>
            {
                journey
            };
            
            _dbSetMock = _journeys.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Journeys)
                .Returns(_dbSetMock.Object);

            _dut = new JourneyRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetStepByStepId_KnownId_ReturnsStep()
        {
            var result = await _dut.GetStepByStepIdAsync(StepId);

            Assert.AreEqual(StepId, result.Id);
        }

        [TestMethod]
        public async Task GetStepByStepId_UnknownId_ReturnsNull()
        {
            var result = await _dut.GetStepByStepIdAsync(99);

            Assert.IsNull(result);
        }
    }
}
