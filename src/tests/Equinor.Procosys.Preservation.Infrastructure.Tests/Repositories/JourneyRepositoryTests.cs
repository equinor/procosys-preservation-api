using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class JourneyRepositoryTests : RepositoryTestBase
    {
        private const int JourneyId = 5;
        private const int StepId = 51;
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _dbSetMock;

        private JourneyRepository _dut;
        private Step _step;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            
            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Plant).Returns(TestPlant);

            _step = new Step(TestPlant, "S", modeMock.Object, responsibleMock.Object);
            _step.SetProtectedIdForTesting(StepId);

            _journey = new Journey(TestPlant, "J");
            _journey.SetProtectedIdForTesting(5);

            _journey.AddStep(_step);
            
            _journeys = new List<Journey>
            {
                _journey
            };
            
            _dbSetMock = _journeys.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Journeys)
                .Returns(_dbSetMock.Object);

            _dut = new JourneyRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetStepByStepId_KnownId_ShouldReturnStep()
        {
            var result = await _dut.GetStepByStepIdAsync(StepId);

            Assert.AreEqual(StepId, result.Id);
        }

        [TestMethod]
        public async Task GetStepByStepId_UnknownId_ShouldReturnNull()
        {
            var result = await _dut.GetStepByStepIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetJourneysByStepIdsAsync_KnownId_ShouldReturnStep()
        {
            var result = await _dut.GetJourneysByStepIdsAsync(new List<int>{StepId});

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(JourneyId, result.Single().Id);
        }

        [TestMethod]
        public async Task GetJourneysByStepIdsAsync_UnknownId_ShouldReturnEmptyList()
        {
            var result = await _dut.GetJourneysByStepIdsAsync(new List<int> {6355});

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetJourneysWithAutoTransferStepsAsync_MatchingAutoTransferMethod_ShouldReturnJourneys()
        {
            // Arrange
            _step.AutoTransferMethod = AutoTransferMethod.OnRfccSign;

            // Act
            var result = await _dut.GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod.OnRfccSign);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(JourneyId, result.Single().Id);
        }

        [TestMethod]
        public void Remove_WhenJourneyIsVoided_ShouldCallRemoveForJourney()
        {
            // Arrange
            _journey.IsVoided = true;

            // Act
            _dut.Remove(_journey);

            // Arrange
            _dbSetMock.Verify(s => s.Remove(_journey), Times.Once);
        }

        [TestMethod]
        public void Remove_WhenJourneyIsNotVoided_ShouldThrowException()
            => Assert.ThrowsException<Exception>(() => _dut.Remove(_journey));
    }
}
