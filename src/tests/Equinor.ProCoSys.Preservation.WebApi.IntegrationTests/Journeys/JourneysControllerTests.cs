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
                JourneyId1UnderTest);

            // Assert
            Assert.IsNotNull(journey);
            Assert.IsNotNull(journey.Steps);
            Assert.AreNotEqual(0, journey.Steps.Count());
            var step = journey.Steps.SingleOrDefault(s => s.Id == FirstStepIdInJourney1UnderTest);
            Assert.IsNotNull(step);
        }

        [TestMethod]
        public async Task UpdateJourney_AsAdmin_ShouldUpdateJourney()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var newTitle = Guid.NewGuid().ToString();
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            var currentRowVersion = journey.RowVersion;

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                newTitle,
                currentRowVersion);
            
            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            
            journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            Assert.IsNotNull(journey);
            Assert.AreEqual(newTitle, journey.Title);
        }

        [TestMethod]
        public async Task VoidJourney_AsAdmin_ShouldVoidJourney()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            var currentRowVersion = journey.RowVersion;

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);

            journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            Assert.IsTrue(journey.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidJourney_AsAdmin_ShouldUnvoidJourney()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidJourneyAsync(
               UserType.LibraryAdmin,
               TestFactory.PlantWithAccess,
               journeyId,
               journey.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);

            journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            Assert.IsFalse(journey.IsVoided);
        }

        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldDeleteJourneyWithoutStep()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidJourneyAsync(
               UserType.LibraryAdmin,
               TestFactory.PlantWithAccess,
               journeyId,
               journey.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                currentRowVersion);

            // Assert
            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            Assert.IsNull(journeys.SingleOrDefault(j => j.Id == journeyId));
        }

        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldDeleteJourneyWithStep()
        {
            // Arrange
            var (journeyId, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidJourneyAsync(
               UserType.LibraryAdmin,
               TestFactory.PlantWithAccess,
               journeyId,
               journey.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                currentRowVersion);

            // Assert
            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            Assert.IsNull(journeys.SingleOrDefault(j => j.Id == journeyId));
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateStep()
        {
            // Arrange
            var title = Guid.NewGuid().ToString();

            // Act
            var (_, step) = await CreateStepAsync(title, OtherModeIdUnderTest);

            // Assert
            Assert.IsNotNull(step);
            Assert.AreEqual(title, step.Title);
        }

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldCreateTwoSupplierSteps()
        {
            // Arrange
            var (journeyIdUnderTest, stepA) = await CreateStepAsync(Guid.NewGuid().ToString(), SupModeAIdUnderTest);

            // Act
            var stepBId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                SupModeBIdUnderTest,
                KnownTestData.ResponsibleCode);

            // Assert
            await AssertStepIsForSupplier(journeyIdUnderTest, stepA.Id);
            await AssertStepIsForSupplier(journeyIdUnderTest, stepBId);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateStepAndRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
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
            step = await GetStepDetailsAsync(journeyIdUnderTest, step.Id);
            Assert.AreEqual(newTitle, step.Title);
        }

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldUpdateSecondStepToBeSupplier()
        {
            // Arrange
            var (journeyIdUnderTest, firstStep) = await CreateStepAsync(Guid.NewGuid().ToString(), SupModeAIdUnderTest);
            var secondStepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode);
            var secondStep = await GetStepDetailsAsync(journeyIdUnderTest, secondStepId);
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
            await AssertStepIsForSupplier(journeyIdUnderTest, firstStep.Id);
            await AssertStepIsForSupplier(journeyIdUnderTest, secondStep.Id);
        }

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldVoidStep_AndUpdateRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            var currentRowVersion = step.RowVersion;
            Assert.IsFalse(step.IsVoided);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetailsAsync(journeyIdUnderTest, step.Id);
            Assert.IsTrue(step.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldUnvoidStep_AndUpdateRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                step.RowVersion);

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            step = await GetStepDetailsAsync(journeyIdUnderTest, step.Id);
            Assert.IsFalse(step.IsVoided);
        }

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldDeleteStep()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            var currentRowVersion = await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, 
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                step.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                currentRowVersion);

            // Assert
            step = await GetStepDetailsAsync(journeyIdUnderTest, step.Id);
            Assert.IsNull(step);
        }
        
        [TestMethod]
        public async Task SwapSteps_AsAdmin_ShouldSwapSteps_AndUpdateRowVersions()
        {
            // Arrange
            var (journeyIdUnderTest, originalFirstStep) = await CreateStepAsync(Guid.NewGuid().ToString(), SupModeAIdUnderTest);
            Assert.IsTrue(originalFirstStep.Mode.ForSupplier);

            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
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

        private async Task AssertStepIsForSupplier(int journeyId, int stepId)
        {
            var stepA = await GetStepDetailsAsync(journeyId, stepId);
            Assert.IsNotNull(stepA);
            Assert.IsTrue(stepA.Mode.ForSupplier);
        }
    }
}
