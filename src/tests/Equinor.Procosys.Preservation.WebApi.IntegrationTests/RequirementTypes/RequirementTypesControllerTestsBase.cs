using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    [TestClass]
    public class RequirementTypesControllerTestsBase : TestBase
    {
        protected int ReqTypeAIdUnderTest;
        protected int ReqDefInReqTypeAIdUnderTest;
        protected int ReqTypeBIdUnderTest;
        protected int ReqDefInReqTypeBIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var requirementTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(LibraryAdminClient(TestFactory.PlantWithAccess));
            
            var reqTypeA = requirementTypes.Single(j => j.Code == KnownTestData.ReqTypeA);
            ReqTypeAIdUnderTest = reqTypeA.Id;
            ReqDefInReqTypeAIdUnderTest = reqTypeA.RequirementDefinitions.First().Id;
            
            var reqTypeB = requirementTypes.Single(j => j.Code == KnownTestData.ReqTypeB);
            ReqTypeBIdUnderTest = reqTypeB.Id;
            ReqDefInReqTypeBIdUnderTest = reqTypeB.RequirementDefinitions.First().Id;        }
    }
}
