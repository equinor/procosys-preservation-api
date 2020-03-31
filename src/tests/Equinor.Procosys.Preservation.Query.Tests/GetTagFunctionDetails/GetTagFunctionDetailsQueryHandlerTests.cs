using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunctionDetails
{
    [TestClass]
    public class GetTagFunctionDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private TagFunction _tfWithRequirement;
        private TagFunction _tfWithoutRequirement;
        private RequirementDefinition _requirementDefinition;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _tfWithRequirement = AddTagFunction(context, "TFC2", "RC1");
                var rt = AddRequirementTypeWith1DefWithoutField(context, "ROT", "R");
                _requirementDefinition = rt.RequirementDefinitions.First();
                _tfWithRequirement.AddRequirement(new TagFunctionRequirement(TestPlant, 4, _requirementDefinition));

                _tfWithoutRequirement = AddTagFunction(context, "TFC1", "RC1");
                
                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionDetailsQuery_ShouldReturnTagFunctionWithRequirement_WhenTagFunctionHaveRequirement()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionDetailsQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionDetailsQuery(
                    TestPlant,
                    _tfWithRequirement.Code,
                    _tfWithRequirement.RegisterCode),
                    default);

                var tagFunction = result.Data;

                Assert.IsNotNull(tagFunction);
                Assert.AreEqual(_tfWithRequirement.Code, tagFunction.Code);
                Assert.AreEqual(_tfWithRequirement.Description, tagFunction.Description);
                Assert.AreEqual(_tfWithRequirement.RegisterCode, tagFunction.RegisterCode);
                var tagFunctionRequirements = tagFunction.Requirements.ToList();
                Assert.AreEqual(1, tagFunctionRequirements.Count);
                Assert.AreEqual(_requirementDefinition.Id, tagFunctionRequirements.Single().RequirementDefinitionId);
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionDetailsQuery_ShouldReturnTagFunctionWithoutRequirement_WhenTagFunctionHaveNoRequirements()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionDetailsQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionDetailsQuery(TestPlant, _tfWithoutRequirement.Code, _tfWithoutRequirement.RegisterCode), default);

                var tagFunction = result.Data;

                Assert.IsNotNull(tagFunction);
                Assert.AreEqual(_tfWithoutRequirement.Code, tagFunction.Code);
                Assert.AreEqual(_tfWithoutRequirement.Description, tagFunction.Description);
                Assert.AreEqual(_tfWithoutRequirement.RegisterCode, tagFunction.RegisterCode);
                Assert.AreEqual(0, tagFunction.Requirements.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionDetailsQuery_UnknownTagFunction_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionDetailsQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionDetailsQuery(TestPlant, "XX", _tfWithRequirement.RegisterCode), default);
                Assert.IsNull(result.Data);
            }
        }
    }
}
