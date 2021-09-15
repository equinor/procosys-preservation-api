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
    public class RequirementTypesControllerNegativeTests :  RequirementTypesControllerTestsBase
    {
        #region GetRequirementTypes
        [TestMethod]
        public async Task GetRequirementTypes_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetRequirementTypes_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                HttpStatusCode.Forbidden);

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

        #region CreateRequirementDefinition
        [TestMethod]
        public async Task CreateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task CreateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                "RequirementDefinition1",
                expectedStatusCode:HttpStatusCode.Forbidden);
        #endregion
       
        #region UpdateRequirementDefinition
        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
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
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess, 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999,
                8888,
                Guid.NewGuid().ToString(),
                4,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsPreserver_ShouldReturnBadRequest_WhenUnknownReqTypeId()
            => await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
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
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
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
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = ReqDefIdUnderTest_ForReqDefWithCbField_InReqTypeA;
            var existingReqDefWithField = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDefIdUnderTest);

            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
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
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                newReqDefId);

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

        #endregion

        #region VoidRequirementDefinition
        [TestMethod]
        public async Task VoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
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
        #endregion

        #region UnvoidRequirementDefinition
        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
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
        #endregion

        #region DeleteRequirementDefinition
        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAnonymous_ShouldReturnUnauthorized()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                8888,
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

        #endregion
    }
}
