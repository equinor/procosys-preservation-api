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
        protected int TwoStepJourneyWithTagsIdUnderTest;
        protected int FirstStepInJourneyWithTagsIdUnderTest;
        protected int JourneyNotInUseIdUnderTest;
        protected int StepInJourneyNotInUseIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            OtherModeIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].OtherModeId;
            SupModeAIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].SupModeAId;
            SupModeBIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].SupModeBId;

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var journeyWithTags = journeys.Single(j => j.Title == KnownTestData.TwoStepJourneyWithTags);
            TwoStepJourneyWithTagsIdUnderTest = journeyWithTags.Id;
            FirstStepInJourneyWithTagsIdUnderTest = journeyWithTags.Steps.First().Id;
            var journeyNotInUse = journeys.Single(j => j.Title == KnownTestData.JourneyNotInUse);
            JourneyNotInUseIdUnderTest = journeyNotInUse.Id;
            StepInJourneyNotInUseIdUnderTest = journeyNotInUse.Steps.First().Id;

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
