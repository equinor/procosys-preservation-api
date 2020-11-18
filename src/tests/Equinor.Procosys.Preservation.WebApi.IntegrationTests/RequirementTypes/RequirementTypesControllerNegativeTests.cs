using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    [TestClass]
    public class RequirementTypesControllerNegativeTests :  RequirementTypesControllerTestsBase
    {
        #region GetRequirementTypes
        [TestMethod]
        public async Task GetRequirementTypes_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetRequirementTypes_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementTypes_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);
        #endregion
       
        #region UpdateRequirementDefinition
        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUnknownReqTypeId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUnknownReqDefId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                ReqTypeAIdUnderTest, 
                ReqDefInReqTypeBIdUnderTest,   // known ReqDefId, but under other ReqType
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");
        #endregion
    }
}
