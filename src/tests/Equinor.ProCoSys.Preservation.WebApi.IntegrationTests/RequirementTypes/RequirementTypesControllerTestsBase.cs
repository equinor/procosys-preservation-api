using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public class RequirementTypesControllerTestsBase : TestBase
    {
        protected int ReqTypeAIdUnderTest;
        protected int ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA;
        protected int ReqDefIdUnderTest_ForReqDefWithCbField_InReqTypeA;
        protected int ReqTypeBIdUnderTest;
        protected int ReqDefInReqTypeBIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var requirementTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            
            var reqTypeA = requirementTypes.Single(j => j.Code == KnownTestData.ReqTypeA);
            ReqTypeAIdUnderTest = reqTypeA.Id;
            ReqDefIdUnderTest_ForReqDefWithNoFields_InReqTypeA
                = reqTypeA.RequirementDefinitions.Single(rd => rd.Title == KnownTestData.ReqDefInReqTypeANoField).Id;
            ReqDefIdUnderTest_ForReqDefWithCbField_InReqTypeA
                = reqTypeA.RequirementDefinitions.Single(rd => rd.Title == KnownTestData.ReqDefInReqTypeAWithCbField).Id;
            
            var reqTypeB = requirementTypes.Single(j => j.Code == KnownTestData.ReqTypeB);
            ReqTypeBIdUnderTest = reqTypeB.Id;
            ReqDefInReqTypeBIdUnderTest = reqTypeB.RequirementDefinitions.First().Id;        }
    }
}
