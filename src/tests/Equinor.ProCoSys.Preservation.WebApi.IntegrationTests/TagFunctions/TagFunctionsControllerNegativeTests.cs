using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.TagFunctions
{
    [TestClass]
    public class TagFunctionsControllerNegativeTests : TagFunctionsControllerTestsBase
    {
        #region Get
        [TestMethod]
        public async Task GetTagFunction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.Anonymous,
                TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTagFunction_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagFunction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTagFunction_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagFunction_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagFunction_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagFunction_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Update
        [TestMethod]
        public async Task UpdateTagFunction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateTagFunction_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagFunction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateTagFunction_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagFunction_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagFunction_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagFunction_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                null,
                HttpStatusCode.Forbidden);

        #endregion

        #region Void
        [TestMethod]
        public async Task VoidTagFunction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidTagFunction_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidTagFunction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidTagFunction_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidTagFunction_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidTagFunction_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidTagFunction_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidTagFunction_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            await EnsureTagFunctionIsUnvoidedAsync(
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode);

            // Act
            await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                           UserType.LibraryAdmin,
                           TestFactory.PlantWithAccess,
                           TagFunctionUnderVoidingTest.Code,
                           TagFunctionUnderVoidingTest.RegisterCode,
                           TestFactory.WrongButValidRowVersion,
                           HttpStatusCode.Conflict);
        }
        #endregion

        #region Unvoid
        [TestMethod]
        public async Task UnvoidTagFunction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidTagFunction_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidTagFunction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidTagFunction_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidTagFunction_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithoutAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidTagFunction_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.Planner, 
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidTagFunction_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidTagFunction_AsAdmin_ShouldReturnConflict_WhenWrongRowVersion()
        {
            // Arrange
            await EnsureTagFunctionIsVoidedAsync(
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode);

            // Act
            await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                           UserType.LibraryAdmin,
                           TestFactory.PlantWithAccess,
                           TagFunctionUnderUnvoidingTest.Code,
                           TagFunctionUnderUnvoidingTest.RegisterCode,
                           TestFactory.WrongButValidRowVersion,
                           HttpStatusCode.Conflict);
        }
        #endregion
    }
}
