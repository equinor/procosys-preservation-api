using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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

        #region CreateRequirementDefinition
        [TestMethod]
        public async Task CreateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);
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
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUnknownReqTypeId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUnknownReqDefId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                ReqTypeAIdUnderTest, 
                ReqDefInReqTypeBIdUnderTest,   // known ReqDefId, but under other ReqType
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"Requirement type and/or requirement definition doesn't exist!");

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUpdatingUnknownFieldId()
        {
            var client = LibraryAdminClient(TestFactory.PlantWithAccess);
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = ReqDefIdUnderTest_ForReqDefWithCbField_InReqTypeA;
            var existingReqDefWithField = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                client,
                reqTypeIdUnderTest,
                reqDefIdUnderTest);

            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                client,
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString(),
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });
            var newReqDefWithField = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                client,
                reqTypeIdUnderTest,
                newReqDefId);

            var fieldsToUpdate = new List<FieldDetailsDto>();
            fieldsToUpdate.AddRange(existingReqDefWithField.Fields);

            // try to update a known field in another requirement definition
            fieldsToUpdate.Add(newReqDefWithField.Fields.Single());

            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                client,
                reqTypeIdUnderTest,
                reqDefIdUnderTest,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                fieldsToUpdate,
                HttpStatusCode.BadRequest,
                "Field doesn't exist in requirement!");
        }

        #endregion

        #region VoidRequirementDefinition
        [TestMethod]
        public async Task VoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");
        #endregion

        #region UnvoidRequirementDefinition
        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");
        #endregion

        #region DeleteRequirementDefinition
        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownRequirementTypeOrRequirementDefinitionId()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                ReqTypeAIdUnderTest,
                ReqDefInReqTypeBIdUnderTest, // req def in other RequirementType
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Requirement type and/or requirement definition doesn't exist!");

        #endregion
    }
}
