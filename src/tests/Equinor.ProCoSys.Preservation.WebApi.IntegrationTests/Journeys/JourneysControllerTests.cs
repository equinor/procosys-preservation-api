using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerTests : JourneysControllerTestsBase
    {
        [TestMethod]
        public async Task CreateJourney_AsAdmin_ShouldCreateJourney()
        {
            // Arrange
            var title = Guid.NewGuid().ToString();

            // Act
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                title);

            // Assert
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            Assert.IsNotNull(journey);
            Assert.AreEqual(title, journey.Title);
        }

        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldGetJourneysWithSteps()
        {
            // Act
            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess);

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
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                TwoStepJourneyWithTagsIdUnderTest);

            // Assert
            Assert.IsNotNull(journey);
            Assert.IsNotNull(journey.Steps);
            Assert.AreNotEqual(0, journey.Steps.Count());
            var step = journey.Steps.SingleOrDefault(s => s.Id == FirstStepInJourneyWithTagsIdUnderTest);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateStep()
        {
            // Arrange
            var journeyIdUnderTest = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var title = Guid.NewGuid().ToString();

            // Act
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                title,
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Assert
            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsNotNull(step);
            Assert.AreEqual(title, step.Title);
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateTwoSupplierSteps()
        {
            // Arrange
            var journeyIdUnderTest = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var stepAId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                SupModeAIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Act
            var stepBId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                SupModeBIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Assert
            await AssertStepIsForSupplier(journeyIdUnderTest, stepAId);
            await AssertStepIsForSupplier(journeyIdUnderTest, stepBId);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateStepAndRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = TwoStepJourneyWithTagsIdUnderTest;
            var stepIdUnderTest = FirstStepInJourneyWithTagsIdUnderTest;
            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepIdUnderTest);
            var currentRowVersion = step.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                newTitle,
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetailsAsync(journeyIdUnderTest, stepIdUnderTest);
            Assert.AreEqual(newTitle, step.Title);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateSecondStepToBeSupplier()
        {
            // Arrange
            var journeyIdUnderTest = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                SupModeAIdUnderTest,
                KnownTestData.ResponsibleCode);
            var firstStep = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsTrue(firstStep.Mode.ForSupplier);
            stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var secondStep = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsFalse(secondStep.Mode.ForSupplier);
            var currentRowVersion = secondStep.RowVersion;

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                secondStep.Id,
                secondStep.Title,
                SupModeBIdUnderTest,
                secondStep.Responsible.Code,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            secondStep = await GetStepDetailsAsync(journeyIdUnderTest, secondStep.Id);
            Assert.IsTrue(secondStep.Mode.ForSupplier);
        }

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldVoidStep_AndUpdateRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = TwoStepJourneyWithTagsIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            var currentRowVersion = step.RowVersion;
            Assert.IsFalse(step.IsVoided);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsTrue(step.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldUnvoidStep_AndUpdateRowVersion()
        {
            // Arrange
            var journeyIdUnderTest = TwoStepJourneyWithTagsIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsFalse(step.IsVoided);
        }

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldDeleteStep()
        {
            // Arrange
            var journeyIdUnderTest = JourneyNotInUseIdUnderTest;
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, 
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                step.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepId,
                currentRowVersion);

            // Assert
            step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsNull(step);
        }
        
        [TestMethod]
        public async Task SwapSteps_AsAdmin_ShouldSwapSteps_AndUpdateRowVersions()
        {
            // Arrange
            var journeyIdUnderTest = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                SupModeAIdUnderTest,
                KnownTestData.ResponsibleCode);
            var originalFirstStep = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsTrue(originalFirstStep.Mode.ForSupplier);
            stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var originalSecondStep = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            Assert.IsFalse(originalSecondStep.Mode.ForSupplier);

            // Act
            var newRowVersions = await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                new StepIdAndRowVersion
                {
                    Id = originalFirstStep.Id,
                    RowVersion = originalFirstStep.RowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = originalSecondStep.Id,
                    RowVersion = originalSecondStep.RowVersion
                });

            // Assert
            var updatedJourney = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest);
            var swappedStepA = updatedJourney.Steps.ElementAt(0);
            var swappedStepB = updatedJourney.Steps.ElementAt(1);
            Assert.AreEqual(originalSecondStep.Id, swappedStepA.Id);
            Assert.AreEqual(originalFirstStep.Id, swappedStepB.Id);
            Assert.IsFalse(swappedStepA.Mode.ForSupplier);
            Assert.IsTrue(swappedStepB.Mode.ForSupplier);
            
            var updatedFirstStepRowVersion = updatedJourney.Steps.Single(s => s.Id == originalFirstStep.Id).RowVersion;
            var updatedSecondStepRowVersion = updatedJourney.Steps.Single(s => s.Id == originalSecondStep.Id).RowVersion;
            AssertRowVersionChange(originalFirstStep.RowVersion, updatedFirstStepRowVersion);
            AssertRowVersionChange(originalSecondStep.RowVersion, updatedSecondStepRowVersion);

            Assert.AreEqual(updatedFirstStepRowVersion, newRowVersions.Single(r => r.Id == originalFirstStep.Id).RowVersion);
            Assert.AreEqual(updatedSecondStepRowVersion, newRowVersions.Single(r => r.Id == originalSecondStep.Id).RowVersion);
        }

        private async Task<StepDetailsDto> GetStepDetailsAsync(int journeyId, int stepId)
        {
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            return journey.Steps.SingleOrDefault(s => s.Id == stepId);
        }

        private async Task AssertStepIsForSupplier(int journeyId, int stepId)
        {
            var stepA = await GetStepDetailsAsync(journeyId, stepId);
            Assert.IsNotNull(stepA);
            Assert.IsTrue(stepA.Mode.ForSupplier);
        }
    }
}
