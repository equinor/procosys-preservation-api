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
        public async Task UpdateStep_AsAdmin_ShouldUpdateStepAndRowVersion()
        {
            // Assert
            var journeyIdUnderTest = TestFactory.KnownTestData.JourneyIds.First();
            var stepId = await JourneysControllerTestsHelper.AddStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                journeyIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.KnownTestData.ModeIds.First(),
                KnownTestData.ResponsibleCode);
            var step = await JourneysControllerTestsHelper.GetStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                journeyIdUnderTest,
                stepId);
            var currentRowVersion = step.RowVersion;

            // Act
            var newRowVersion = await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                journeyIdUnderTest,
                step.Id,
                Guid.NewGuid().ToString(),
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
        }
    }
}
