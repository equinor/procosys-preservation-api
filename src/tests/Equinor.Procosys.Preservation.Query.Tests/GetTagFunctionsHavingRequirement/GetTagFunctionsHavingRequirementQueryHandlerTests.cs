using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunctionsHavingRequirement
{
    [TestClass]
    public class GetTagFunctionsHavingRequirementQueryHandlerTests : ReadOnlyTestsBase
    {
        private TagFunction _tfWithRequirement;
        private RequirementDefinition _requirementDefinition;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _tfWithRequirement = AddTagFunction(context, "TFC2", "RC1");
                var rt = AddRequirementTypeWith1DefWithoutField(context, "ROT", "R");
                _requirementDefinition = rt.RequirementDefinitions.First();
                _tfWithRequirement.AddRequirement(new TagFunctionRequirement(TestPlant, 4, _requirementDefinition));

                AddTagFunction(context, "TFC1", "RC1");
                
                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_KnownTagFunction_ShouldReturnTagFunctionWithAllPropertiesSet()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionsHavingRequirementQuery(), default);

                var tagFunctions = result.Data.ToList();

                Assert.AreEqual(1, tagFunctions.Count);
                Assert.AreEqual(_tfWithRequirement.Code, tagFunctions.Single().Code);
                Assert.AreEqual(_tfWithRequirement.RegisterCode, tagFunctions.Single().RegisterCode);
            }
        }
    }
}
