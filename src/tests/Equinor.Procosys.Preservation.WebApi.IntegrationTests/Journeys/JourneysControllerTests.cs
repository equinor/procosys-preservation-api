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
                UserType.LibraryAdmin, TestFactory.PlantWithAccess);

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
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyWithTagsIdUnderTest);

            // Assert
            Assert.IsNotNull(journey);
            Assert.IsNotNull(journey.Steps);
            Assert.AreNotEqual(0, journey.Steps.Count());
            var step = journey.Steps.SingleOrDefault(s => s.Id == StepInJourneyWithTagsIdUnderTest);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateStep()
        {
            // Arrange
            var journeyIdUnderTest = JourneyWithTagsIdUnderTest;
            var title = Guid.NewGuid().ToString();

            // Act
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                title,
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Assert
            var step = await GetStepDetails(journeyIdUnderTest, stepId);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateStepAndRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = JourneyWithTagsIdUnderTest;
            var stepIdUnderTest = StepInJourneyWithTagsIdUnderTest;
            var step = await GetStepDetails(journeyIdUnderTest, stepIdUnderTest);
            var currentRowVersion = step.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                newTitle,
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(journeyIdUnderTest, stepIdUnderTest);
            Assert.AreEqual(newTitle, step.Title);
        }

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldVoidStep_AndUpdateAndRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = JourneyWithTagsIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(journeyIdUnderTest, stepId);
            var currentRowVersion = step.RowVersion;
            Assert.IsFalse(step.IsVoided);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(journeyIdUnderTest, stepId);
            Assert.IsTrue(step.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldUnvoidStep_AndUpdateAndRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = JourneyWithTagsIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(journeyIdUnderTest, stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetails(journeyIdUnderTest, stepId);
            Assert.IsFalse(step.IsVoided);
        }

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldDeleteStep()
        {
            // Arrange
            var journeyIdUnderTest = JourneyNotInUseIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetails(journeyIdUnderTest, stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            step = await GetStepDetails(journeyIdUnderTest, stepId);
            Assert.IsNull(step);
        }

        private async Task<StepDetailsDto> GetStepDetails(int journeyId, int stepId)
        {
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                journeyId);
            return journey.Steps.SingleOrDefault(s => s.Id == stepId);
        }
    }
}
