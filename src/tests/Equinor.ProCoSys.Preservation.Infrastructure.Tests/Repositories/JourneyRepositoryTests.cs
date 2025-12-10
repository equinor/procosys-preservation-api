using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class JourneyRepositoryTests : RepositoryTestBase
    {
        private const int JourneyId = 5;
        private const int StepId1 = 51;
        private const int StepId2 = 52;
        private List<Journey> _journeys;
        private Mock<DbSet<Journey>> _journeySetMock;
        private Mock<DbSet<Step>> _stepSetMock;

        private JourneyRepository _dut;
        private Step _step1;
        private Step _step2;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Plant).Returns(TestPlant);

            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Plant).Returns(TestPlant);

            _step1 = new Step(TestPlant, "S1", modeMock.Object, responsibleMock.Object);
            _step1.SetProtectedIdForTesting(StepId1);

            _step2 = new Step(TestPlant, "S2", modeMock.Object, responsibleMock.Object);
            _step2.SetProtectedIdForTesting(StepId2);

            _journey = new Journey(TestPlant, "J");
            _journey.SetProtectedIdForTesting(5);

            _journey.AddStep(_step1);
            _journey.AddStep(_step2);

            _journeys = new List<Journey>
            {
                _journey
            };

            _journeySetMock = _journeys.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Journeys)
                .Returns(_journeySetMock.Object);

            var steps = new List<Step> { _step1, _step2 };
            _stepSetMock = steps.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Steps)
                .Returns(_stepSetMock.Object);

            _dut = new JourneyRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetStepByStepId_KnownId_ShouldReturnStep()
        {
            var result = await _dut.GetStepByStepIdAsync(StepId1);

            Assert.AreEqual(StepId1, result.Id);
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
            var result = await _dut.GetJourneysByStepIdsAsync(new List<int> { StepId1 });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(JourneyId, result.Single().Id);
        }

        [TestMethod]
        public async Task GetJourneysByStepIdsAsync_UnknownId_ShouldReturnEmptyList()
        {
            var result = await _dut.GetJourneysByStepIdsAsync(new List<int> { 6355 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetJourneysWithAutoTransferStepsAsync_MatchingAutoTransferMethod_ShouldReturnJourneys()
        {
            // Arrange
            _step1.AutoTransferMethod = AutoTransferMethod.OnRfccSign;

            // Act
            var result = await _dut.GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod.OnRfccSign);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(JourneyId, result.Single().Id);
        }

        [TestMethod]
        public void Remove_WhenJourneyIsVoided_ShouldRemoveJourneyAndStepsFromContext()
        {
            // Arrange
            _journey.IsVoided = true;

            // Act
            _dut.Remove(_journey);

            // Arrange
            _journeySetMock.Verify(s => s.Remove(_journey), Times.Once);
            _stepSetMock.Verify(s => s.Remove(_step1), Times.Once);
            _stepSetMock.Verify(s => s.Remove(_step2), Times.Once);
        }

        [TestMethod]
        public void Remove_WhenJourneyIsNotVoided_ShouldThrowException()
            => Assert.ThrowsException<Exception>(() => _dut.Remove(_journey));
    }
}
