using System;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags;
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
                Guid.NewGuid().ToString(),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                Guid.NewGuid().ToString(),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);
        #endregion

        #region GetJourneys
        [TestMethod]
        public async Task GetJourneys_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourneys_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                HttpStatusCode.Forbidden);

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
                JourneyId1UnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                HttpStatusCode.Forbidden);
        #endregion

        #region UpdateJourney
        [TestMethod]
        public async Task UpdateJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateJourney_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            // Act
            await JourneysControllerTestsHelper.UpdateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                Guid.NewGuid().ToString(),
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region VoidJourney
        [TestMethod]
        public async Task VoidJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidJourney_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var journeyId = await JourneysControllerTestsHelper.CreateJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            // Act
            await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }

        #endregion

        #region UnvoidJourney
        [TestMethod]
        public async Task UnvoidJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidJourney_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
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

            await JourneysControllerTestsHelper.VoidJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                journey.RowVersion);

            // Act
            await JourneysControllerTestsHelper.UnvoidJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region CreateStep
        [TestMethod]
        public async Task CreateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                HttpStatusCode.Forbidden);
        #endregion

        #region UpdateStep
        [TestMethod]
        public async Task UpdateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyId2UnderTest,
                FirstStepIdInJourney1UnderTest, // step in other Journey
                Guid.NewGuid().ToString(),
                OtherModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
           
            // Act
            await JourneysControllerTestsHelper.UpdateStepAsync(
                           UserType.LibraryAdmin,
                           TestFactory.PlantWithAccess,
                           journeyIdUnderTest,
                           step.Id,
                           Guid.NewGuid().ToString(),
                           OtherModeIdUnderTest,
                           KnownTestData.ResponsibleCode,
                           TestFactory.WrongButValidRowVersion,
                           HttpStatusCode.Conflict);
        }
        #endregion

        #region VoidStep
        [TestMethod]
        public async Task VoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyId2UnderTest,
                FirstStepIdInJourney1UnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);

            // Act
            await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
         
        #endregion

        #region UnvoidStep
        [TestMethod]
        public async Task UnvoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyId2UnderTest,
                FirstStepIdInJourney1UnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            await JourneysControllerTestsHelper.VoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                step.RowVersion);
            
            // Act
            await JourneysControllerTestsHelper.UnvoidStepAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                step.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region DeleteJourney
        [TestMethod]
        public async Task DeleteJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteJourney_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldReturnBadRequest_WhenDeleteJourneyWithTagInStep()
        {
            // Arrange
            var (journeyId, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);

            await CreateStandardTagAsync(step.Id, false);

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
                currentRowVersion,
                HttpStatusCode.BadRequest,
                "Journey can not be deleted when preservation tags exists in journey!");
        }
        [TestMethod]
        public async Task DeleteJourney_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
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
            await JourneysControllerTestsHelper.VoidJourneyAsync(
               UserType.LibraryAdmin,
               TestFactory.PlantWithAccess,
               journeyId,
               journey.RowVersion);

            // Act
            await JourneysControllerTestsHelper.DeleteJourneyAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyId,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region DeleteStep
        [TestMethod]
        public async Task DeleteStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                JourneyId1UnderTest,
                FirstStepIdInJourney1UnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                JourneyId2UnderTest,
                FirstStepIdInJourney1UnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");


        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var (journeyIdUnderTest, step) = await CreateStepAsync(Guid.NewGuid().ToString(), OtherModeIdUnderTest);
            await JourneysControllerTestsHelper.VoidStepAsync(
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
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region SwapSteps
        [TestMethod]
        public async Task SwapSteps_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
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
        public async Task SwapSteps_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
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
        public async Task SwapSteps_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                JourneyId1UnderTest,
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
                JourneyId1UnderTest,
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
                JourneyId1UnderTest,
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
                JourneyId1UnderTest,
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
                JourneyId1UnderTest,
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
        public async Task SwapSteps_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
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
            await JourneysControllerTestsHelper.SwapStepsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                journeyIdUnderTest,
                new StepIdAndRowVersion
                {
                    Id = originalFirstStep.Id,
                    RowVersion = TestFactory.WrongButValidRowVersion
                },
                new StepIdAndRowVersion
                {
                    Id = originalSecondStep.Id,
                    RowVersion = originalSecondStep.RowVersion
                },
                HttpStatusCode.Conflict);

        }
        #endregion
    }
}
