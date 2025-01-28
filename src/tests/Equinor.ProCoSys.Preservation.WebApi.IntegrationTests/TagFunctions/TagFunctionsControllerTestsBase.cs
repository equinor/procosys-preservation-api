using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.TagFunctions
{
    public class TagFunctionsControllerTestsBase : TestBase
    {
        protected TagFunctionDetailsDto TagFunctionUnderTest;
        protected TagFunctionDetailsDto TagFunctionUnderVoidingTest;
        protected TagFunctionDetailsDto TagFunctionUnderUnvoidingTest;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var tagFunctionCodeUnderTest = "TestTF";
            var registerCodeUnderTest = "RegA";
            var registerCodeForVoidingTest = "RegB";
            var registerCodeForUnvoidingTest = "RegC";
            var newReqDefId = await CreateRequirementDefinitionAsync(TestFactory.PlantWithAccess);

            TagFunctionUnderTest = await UpdateAndGetTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                tagFunctionCodeUnderTest,
                registerCodeUnderTest,
                newReqDefId,
                4);

            TagFunctionUnderVoidingTest = await UpdateAndGetTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                tagFunctionCodeUnderTest,
                registerCodeForVoidingTest,
                newReqDefId,
                4);

            TagFunctionUnderUnvoidingTest = await UpdateAndGetTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                tagFunctionCodeUnderTest,
                registerCodeForUnvoidingTest,
                newReqDefId,
                4);
        }

        protected async Task<TagFunctionDetailsDto> UpdateAndGetTagFunctionAsync(
            UserType userType,
            string plant,
            string tagFunctionCode,
            string registerCode,
            int reqDefId,
            int intervalWeeks)
        {
            MockMainApiDataForTagFunction(plant, tagFunctionCode, registerCode);

            await TagFunctionsControllerTestsHelper.UpdateTagFunctionAsync(
                UserType.LibraryAdmin,
                plant,
                tagFunctionCode,
                registerCode,
                new List<TagFunctionRequirementDto>
                {
                    new TagFunctionRequirementDto
                    {
                        RequirementDefinitionId = reqDefId,
                        IntervalWeeks = intervalWeeks
                    }
                });

            return await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                plant,
                tagFunctionCode,
                registerCode);
        }

        private void MockMainApiDataForTagFunction(string plant, string tagFunctionCode, string registerCode)
        {
            var mainTagFunctionDetails = new PCSTagFunction
            {
                Code = tagFunctionCode,
                Description = $"{tagFunctionCode} {registerCode} Description",
                RegisterCode = registerCode
            };

            TestFactory.Instance
                .TagFunctionApiServiceMock
                .Setup(service => service.TryGetTagFunctionAsync(
                    plant,
                    tagFunctionCode,
                    registerCode,
                    CancellationToken.None))
                .Returns(Task.FromResult(mainTagFunctionDetails));
        }

        protected async Task<string> EnsureTagFunctionIsUnvoidedAsync(
            string plant,
            string code,
            string registerCode)
        {
            var tagFunction = await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                plant,
                code,
                registerCode);

            if (tagFunction.IsVoided)
            {
                return await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                    UserType.LibraryAdmin,
                    plant,
                    code,
                    registerCode,
                    tagFunction.RowVersion);
            }
            
            return tagFunction.RowVersion;
        }

        protected async Task<string> EnsureTagFunctionIsVoidedAsync(
            string plant,
            string code,
            string registerCode)
        {
            var tagFunction = await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                plant,
                code,
                registerCode);

            if (!tagFunction.IsVoided)
            {
                return await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                    UserType.LibraryAdmin,
                    plant,
                    code,
                    registerCode,
                    tagFunction.RowVersion);
            }

            return tagFunction.RowVersion;
        }
    }
}
