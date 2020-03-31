using System.Linq;
using System.Threading.Tasks;
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
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_NoTagFunctiona_ShouldReturnEmptyList()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionsHavingRequirementQuery(TestPlant), default);

                Assert.AreEqual(0, result.Data.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_NoTagFunctionsWithRequirement_ShouldReturnEmptyList()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddTagFunction(context, "TF2", "RC");
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionsHavingRequirementQuery(TestPlant), default);

                Assert.AreEqual(0, result.Data.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_KnownTagFunction_ShouldReturnTagFunctionWithAllPropertiesSet()
        {
            TagFunction tf;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                tf = AddTagFunction(context, "TFC", "RC");
                var rt = AddRequirementTypeWith1DefWithoutField(context, "ROT", "R");
                tf.AddRequirement(new TagFunctionRequirement(TestPlant, 4, rt.RequirementDefinitions.First()));
                
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionsHavingRequirementQuery(TestPlant), default);

                var tagFunctions = result.Data.ToList();

                Assert.AreEqual(1, tagFunctions.Count);
                Assert.AreEqual(tf.Code, tagFunctions.Single().Code);
                Assert.AreEqual(tf.RegisterCode, tagFunctions.Single().RegisterCode);
            }
        }
    }
}
