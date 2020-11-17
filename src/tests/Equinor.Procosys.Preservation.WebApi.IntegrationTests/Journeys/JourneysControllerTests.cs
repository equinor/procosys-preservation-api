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
        public async Task GetJourney_AsAdmin_ShouldGetJourneyWithStep()
        {
            // Act
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest);

            // Assert
            Assert.IsNotNull(journey);
            Assert.IsNotNull(journey.Steps);
            Assert.AreNotEqual(0, journey.Steps.Count());
            var step = journey.Steps.SingleOrDefault(s => s.Id == StepIdUnderTest);
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
                JourneyIdUnderTest,
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
            var step = await GetStepDetails(StepIdUnderTest);
            var currentRowVersion = step.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest,
                step.Id,
                newTitle,
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(StepIdUnderTest);
            Assert.AreEqual(newTitle, step.Title);
        }

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldVoidStep()
        {
            // Arrange
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(stepId);
            var currentRowVersion = step.RowVersion;
            Assert.IsFalse(step.IsVoided);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest,
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
                JourneyIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyIdUnderTest,
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
                JourneyIdUnderTest);
            return journey.Steps.SingleOrDefault(s => s.Id == stepId);
        }
    }
}
