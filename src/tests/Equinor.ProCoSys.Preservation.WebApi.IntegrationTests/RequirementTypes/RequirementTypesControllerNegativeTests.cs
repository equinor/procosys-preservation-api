using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    [TestClass]
    public class RequirementTypesControllerNegativeTests : RequirementTypesControllerTestsBase
    {
        #region CreateRequirementType
        [TestMethod]
        public async Task CreateRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                Guid.NewGuid().ToString(),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                Guid.NewGuid().ToString(),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementType_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementType_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementTypeAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString(),
                HttpStatusCode.Forbidden);
        #endregion

        #region UpdateRequirementType
        [TestMethod]
        public async Task UpdateRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementType_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownReqTypeId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                Guid.NewGuid().ToString(),
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementType_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            // Act
            await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                Guid.NewGuid().ToString(),
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }

        #endregion

        #region GetRequirementTypes
        [TestMethod]
        public async Task GetRequirementTypes_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetRequirementTypes_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementTypes_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementTypes_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);
        #endregion

        #region GetRequirementType
        [TestMethod]
        public async Task GetRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetRequirementType_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementType_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                HttpStatusCode.Forbidden);
        #endregion

        #region VoidRequirementType
        [TestMethod]
        public async Task VoidRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementType_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementType_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementType_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            // Act
            await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqType.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region UnvoidRequirementType
        [TestMethod]
        public async Task UnvoidRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementType_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementType_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementType_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                reqType.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region DeleteRequirementType
        [TestMethod]
        public async Task DeleteRequirementType_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteRequirementType_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementType_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementType_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementType_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementType_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementType_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementType_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                reqType.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }

        #endregion

        #region CreateRequirementDefinition
        [TestMethod]
        public async Task CreateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                Guid.NewGuid().ToString(),
                expectedStatusCode: HttpStatusCode.Forbidden);
        #endregion

        #region UpdateRequirementDefinition
        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownReqTypeId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownReqDefId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest,   // known ReqDefId, but under other ReqType
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode: HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest: "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUpdatingUnknownFieldId()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = ReqDefIdUnderTest_ForReqDefWithCbField_InReqTypeA;
            var existingReqDefWithField = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDefIdUnderTest);

            var newReqDefWithField = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });

            var fieldsToUpdate = new List<FieldDetailsDto>();
            fieldsToUpdate.AddRange(existingReqDefWithField.Fields);

            // try to update a known field in another requirement definition
            fieldsToUpdate.Add(newReqDefWithField.Fields.Single());

            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDefIdUnderTest,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                fieldsToUpdate,
                HttpStatusCode.BadRequest,
                "Field doesn't exist in requirement!");
        }

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnConflict_WhenWrongRowVersionOnRequirementDefinition()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });

            reqDef.Fields.Single().Label = Guid.NewGuid().ToString();

            // Act
            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.Title,
                4,
                TestFactory.WrongButValidRowVersion,
                reqDef.Fields.ToList(),
                HttpStatusCode.Conflict);
        }

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnConflict_WhenWrongRowVersionOnField()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });

            var fieldDetailsDto = reqDef.Fields.Single();
            fieldDetailsDto.Label = Guid.NewGuid().ToString();
            fieldDetailsDto.RowVersion = TestFactory.WrongButValidRowVersion;

            // Act
            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.Title,
                4,
                reqDef.RowVersion,
                reqDef.Fields.ToList(),
                HttpStatusCode.Conflict);
        }

        #endregion

        #region VoidRequirementDefinition
        [TestMethod]
        public async Task VoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);

            // Act
            await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region UnvoidRequirementDefinition
        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);

            await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }
        #endregion

        #region DeleteRequirementDefinition
        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);

            await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        }

        #endregion
    }
}
