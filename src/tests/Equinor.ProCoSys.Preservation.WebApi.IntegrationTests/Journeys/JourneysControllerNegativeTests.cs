using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerNegativeTests : JourneysControllerTestsBase
    {

        #region CreateJourney
        [TestMethod]
        public async Task CreateJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                "Journey1",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateJourney_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                "Journey1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                "Journey1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                "Journey1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                "Journey1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                "Journey1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                "Journey1",
                HttpStatusCode.Forbidden);
        #endregion

        #region GetJourneys
        [TestMethod]
        public async Task GetJourneys_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourneys_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourneys_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);
        #endregion

        #region GetJourney
        [TestMethod]
        public async Task GetJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);
        #endregion

        #region CreateStep
        [TestMethod]
        public async Task CreateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);
        #endregion

        #region UpdateStep
        [TestMethod]
        public async Task UpdateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyNotInUseIdUnderTest,
                FirstStepInJourneyWithTagsIdUnderTest, // step in other Journey
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        #endregion

        #region VoidStep
        [TestMethod]
        public async Task VoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyNotInUseIdUnderTest,
                FirstStepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");
        #endregion

        #region UnvoidStep
        [TestMethod]
        public async Task UnvoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyNotInUseIdUnderTest,
                FirstStepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");
        #endregion

        #region DeleteStep
        [TestMethod]
        public async Task DeleteStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyNotInUseIdUnderTest,
                FirstStepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        #endregion

        #region SwapSteps
        [TestMethod]
        public async Task SwapSteps_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task SwapSteps_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task SwapSteps_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task SwapSteps_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task SwapSteps_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task SwapSteps_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task SwapSteps_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                9999,
                new StepIdAndRowVersion
                {
                    Id = 2, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = 12, 
                    RowVersion = TestFactory.AValidRowVersion
                },
                HttpStatusCode.Forbidden);

        #endregion
    }
}
