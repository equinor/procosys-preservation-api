using System.Linq;
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
                .Setup(service => service.TryGetResponsibleAsync(TestFactory.PlantWithAccess, KnownTestData.ResponsibleCode))
                .Returns(Task.FromResult(new PCSResponsible
                {
                    Code = KnownTestData.ResponsibleCode, Description = KnownTestData.ResponsibleDescription
                }));
        }
    }
}
