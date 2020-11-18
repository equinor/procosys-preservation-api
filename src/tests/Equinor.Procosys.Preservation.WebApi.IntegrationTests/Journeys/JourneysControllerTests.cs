using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerTests : JourneysControllerTestsBase
    {
        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldGetJourneysWithSteps()
        {
            // Act
            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess));

            // Assert
            Assert.IsNotNull(journeys);
            Assert.AreNotEqual(0, journeys.Count);
            var step = journeys.First().Steps.FirstOrDefault();
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldGetJourneyWithStep()
        {
            // Act
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest);

            // Assert
            Assert.IsNotNull(journey);
            Assert.IsNotNull(journey.Steps);
            Assert.AreNotEqual(0, journey.Steps.Count());
            var step = journey.Steps.SingleOrDefault(s => s.Id == StepInJourneyAIdUnderTest);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateStep()
        {
            // Arrange
            var title = Guid.NewGuid().ToString();

            // Act
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                title,
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Assert
            var step = await GetStepDetails(stepId);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateStepAndRowVersion()
        {
            // Arrange
            var step = await GetStepDetails(StepInJourneyAIdUnderTest);
            var currentRowVersion = step.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                step.Id,
                newTitle,
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(StepInJourneyAIdUnderTest);
            Assert.AreEqual(newTitle, step.Title);
        }

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldVoidStep()
        {
            // Arrange
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(stepId);
            var currentRowVersion = step.RowVersion;
            Assert.IsFalse(step.IsVoided);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(stepId);
            Assert.IsTrue(step.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldUnvoidStep()
        {
            // Arrange
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(stepId);
            Assert.IsFalse(step.IsVoided);
        }

        private async Task<StepDetailsDto> GetStepDetails(int stepId)
        {
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyAIdUnderTest);
            return journey.Steps.SingleOrDefault(s => s.Id == stepId);
        }
    }
}
