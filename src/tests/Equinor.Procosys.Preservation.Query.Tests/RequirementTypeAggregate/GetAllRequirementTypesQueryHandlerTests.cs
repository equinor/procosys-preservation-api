using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class GetAllRequirementTypesQueryHandlerTests : ReadOnlyTestsBase
    {
        private readonly string _numberLabel = "TestLabel";
        private readonly string _numberUnit = "TestUnit";
        private RequirementType _reqType1;
        private RequirementDefinition _reqDef1;
        private RequirementDefinition _reqDef2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _reqType1 = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1", 999);

                _reqDef1 = _reqType1.RequirementDefinitions.First();
                _reqDef2 = new RequirementDefinition(TestPlant, "D2", 2, 1);
                _reqDef2.Void();
                _reqType1.AddRequirementDefinition(_reqDef2);
                context.SaveChangesAsync().Wait();

                AddNumberField(context, _reqDef1, _numberLabel, _numberUnit, true);
                var f = AddNumberField(context, _reqDef1, "NUMBER", "mm", true);
                f.Void();
                context.SaveChangesAsync().Wait();

                var reqType2 = AddRequirementTypeWith1DefWithoutField(context, "T2", "D2", 7);
                reqType2.Void();
                context.SaveChangesAsync().Wait();
                AddRequirementTypeWith1DefWithoutField(context, "T3", "D3", 10000);
                AddRequirementTypeWith1DefWithoutField(context, "T4", "D4", 1);
            }
        }

        [TestMethod]
        public async Task HandleGetAllRequirementTypesQuery_ShouldReturnNonVoidedRequirementTypesOnly_WhenNotGettingVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllRequirementTypesQueryHandler(context);
                var result = await dut.Handle(new GetAllRequirementTypesQuery(false), default);

                var requirementTypes = result.Data.ToList();
                var requirementTypeDto = requirementTypes.Single(rt => rt.Id == _reqType1.Id);
                var requirementDefinitionDto = requirementTypeDto.RequirementDefinitions.Single();
                var fields = requirementDefinitionDto.Fields.ToList();

                Assert.AreEqual(_reqType1.Code, requirementTypeDto.Code);
                Assert.AreEqual(_reqType1.Title, requirementTypeDto.Title);
                Assert.IsFalse(requirementTypeDto.IsVoided);

                Assert.AreEqual(_reqDef1.Title, requirementDefinitionDto.Title);
                Assert.IsFalse(requirementDefinitionDto.IsVoided);

                var fieldDto = fields[0];
                Assert.AreEqual(_numberLabel, fieldDto.Label);
                Assert.AreEqual(_numberUnit, fieldDto.Unit);
                Assert.AreEqual(FieldType.Number, fieldDto.FieldType);
                Assert.IsTrue(fieldDto.ShowPrevious.HasValue);
                Assert.IsTrue(fieldDto.ShowPrevious.Value);
                Assert.IsFalse(fieldDto.IsVoided);
            }
        }

        [TestMethod]
        public async Task HandleGetAllRequirementTypesQuery_ShouldlncludeVoidedRequirementTypes_WhenGettingVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllRequirementTypesQueryHandler(context);
                var result = await dut.Handle(new GetAllRequirementTypesQuery(true), default);

                var requirementTypes = result.Data.ToList();
                var requirementDefinitions = requirementTypes.First(rt => rt.Id == _reqType1.Id).RequirementDefinitions.ToList();
                var fields = requirementDefinitions.First(rd => !rd.IsVoided).Fields.ToList();

                Assert.AreEqual(4, requirementTypes.Count);
                Assert.IsTrue(requirementTypes.Any(j => j.IsVoided));
                Assert.AreEqual(2, requirementDefinitions.Count);
                Assert.IsTrue(requirementDefinitions.Any(j => j.IsVoided));
                Assert.AreEqual(2, fields.Count);
                Assert.IsTrue(fields.Any(j => j.IsVoided));
            }
        }

        [TestMethod]
        public async Task HandleGetAllRequirementTypesQuery_ShouldReturnRequirementTypesSortedBySortKey()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllRequirementTypesQueryHandler(context);
                var result = await dut.Handle(new GetAllRequirementTypesQuery(true), default);

                var dtos = result.Data.ToList();
                Assert.AreEqual(4, dtos.Count);
                Assert.AreEqual(1, dtos[0].SortKey);
                Assert.AreEqual(7, dtos[1].SortKey);
                Assert.AreEqual(999, dtos[2].SortKey);
                Assert.AreEqual(10000, dtos[3].SortKey);
            }
        }
    }
}
