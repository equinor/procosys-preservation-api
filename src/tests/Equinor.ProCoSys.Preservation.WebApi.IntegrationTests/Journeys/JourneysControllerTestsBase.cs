using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class JourneysControllerTestsBase : TestBase
    {
        protected int OtherModeIdUnderTest;
        protected int SupModeAIdUnderTest;
        protected int SupModeBIdUnderTest;
        protected int JourneyId1UnderTest;
        protected int FirstStepIdInJourney1UnderTest;
        protected int JourneyId2UnderTest;
        protected int FirstStepIdInJourney2UnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            OtherModeIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].OtherModeId;
            SupModeAIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].SupModeAId;
            SupModeBIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].SupModeBId;

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var journey = journeys.Single(j => j.Title == KnownTestData.TwoStepJourneyWithoutTags);
            JourneyId1UnderTest = journey.Id;
            FirstStepIdInJourney1UnderTest = journey.Steps.First().Id;
            journey = journeys.Single(j => j.Title == KnownTestData.OneStepJourneyWithoutTags);
            JourneyId2UnderTest = journey.Id;
            FirstStepIdInJourney2UnderTest = journey.Steps.First().Id;

            TestFactory.Instance
                .ResponsibleApiServiceMock
                .Setup(service => service.TryGetResponsibleAsync(TestFactory.PlantWithAccess, KnownTestData.ResponsibleCode, CancellationToken.None))
                .Returns(Task.FromResult(new PCSResponsible
                {
                    Code = KnownTestData.ResponsibleCode, Description = KnownTestData.ResponsibleDescription
                }));
        }

        internal async Task<StepDetailsDto> GetStepDetailsAsync(int journeyId, int stepId)
        {
            var journey = await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId);
            return journey.Steps.SingleOrDefault(s => s.Id == stepId);
        }

        internal async Task<(int, StepDetailsDto)> CreateStepAsync(string stepTitle, int modeId)
        {
            var journeyIdUnderTest = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            // Act
            var stepId = await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                stepTitle,
                modeId,
                KnownTestData.ResponsibleCode);

            var step = await GetStepDetailsAsync(journeyIdUnderTest, stepId);
            return (journeyIdUnderTest, step);
        }
    }
}
