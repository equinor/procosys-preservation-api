using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class JourneyRepositoryTests
    {
        private const string TestPlant = "PCS$TESTPLANT";
        private const string TestJourney = "J1";
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _dbSetMock;
        private ContextHelper _contextHelper;

        private JourneyRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var step = new Mock<Step>().Object;
            var journey = new Journey(TestPlant, TestJourney);
            //journey.AddStep(step);
            //journey.AddStep(step);
            //journey.AddStep(step);
            _journeys = new List<Journey>
            {
                journey,
                new Journey(TestPlant, "J2"),
                new Journey(TestPlant, "J3")
            };
            
            _dbSetMock = _journeys.AsQueryable().BuildMockDbSet();

            _contextHelper = new ContextHelper();
            _contextHelper
                .ContextMock
                .Setup(x => x.Journeys)
                .Returns(_dbSetMock.Object);

            _dut = new JourneyRepository(_contextHelper.ContextMock.Object);
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
    }
}
