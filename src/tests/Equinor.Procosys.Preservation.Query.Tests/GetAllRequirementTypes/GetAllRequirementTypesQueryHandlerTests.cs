using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetAllRequirementTypes;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAllRequirementTypes
{
    [TestClass]
    public class GetAllRequirementTypesQueryHandlerTests : ReadOnlyTestsBase
    {
        private readonly string _numberLabel = "TestLabel";
        private readonly string _numberUnit = "TestUnit";
        private RequirementType _reqType1;
        private RequirementDefinition _reqDefForAll;
        private RequirementDefinition _reqDefVoided;
        private RequirementDefinition _reqDefForOther;
        private RequirementDefinition _reqDefForSupplier;
        RequirementTypeIcon _reqIconOther = RequirementTypeIcon.Other;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _reqType1 = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1", _reqIconOther, 999);

                _reqDefForAll = _reqType1.RequirementDefinitions.First();
                _reqDefVoided = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForAll, 2);
                _reqDefVoided.IsVoided = true;
                _reqDefForOther = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForOtherThanSuppliers, 3);
                _reqDefForSupplier = new RequirementDefinition(TestPlant, "D4", 2, RequirementUsage.ForSuppliersOnly, 4);
                _reqType1.AddRequirementDefinition(_reqDefVoided);
                _reqType1.AddRequirementDefinition(_reqDefForOther);
                _reqType1.AddRequirementDefinition(_reqDefForSupplier);
                context.SaveChangesAsync().Wait();

                AddNumberField(context, _reqDefForAll, _numberLabel, _numberUnit, true);
                var f = AddNumberField(context, _reqDefForAll, "NUMBER", "mm", true);
                f.IsVoided = true;
                context.SaveChangesAsync().Wait();

                var reqType2 = AddRequirementTypeWith1DefWithoutField(context, "T2", "D2", _reqIconOther, 7);
                reqType2.IsVoided = true;
                context.SaveChangesAsync().Wait();
                AddRequirementTypeWith1DefWithoutField(context, "T3", "D3", _reqIconOther, 10000);
                AddRequirementTypeWith1DefWithoutField(context, "T4", "D4", _reqIconOther, 1);
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
                var requirementDefinitionDtoForAll = requirementTypeDto.RequirementDefinitions.Single(rd => rd.Id == _reqDefForAll.Id);
                var fields = requirementDefinitionDtoForAll.Fields.ToList();

                Assert.AreEqual(_reqType1.Code, requirementTypeDto.Code);
                Assert.AreEqual(_reqType1.Title, requirementTypeDto.Title);
                Assert.IsFalse(requirementTypeDto.IsVoided);

                AssertReqDef(_reqDefForAll, requirementDefinitionDtoForAll);
                AssertReqDef(_reqDefForOther, requirementTypeDto.RequirementDefinitions.Single(rd => rd.Id == _reqDefForOther.Id));
                AssertReqDef(_reqDefForSupplier, requirementTypeDto.RequirementDefinitions.Single(rd => rd.Id == _reqDefForSupplier.Id));

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
                var fields = requirementDefinitions.First(rd => rd.Id == _reqDefForAll.Id).Fields.ToList();

                Assert.AreEqual(4, requirementTypes.Count);
                Assert.IsTrue(requirementTypes.Any(j => j.IsVoided));
                Assert.AreEqual(4, requirementDefinitions.Count);
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

        private void AssertReqDef(RequirementDefinition reqDefExpected, RequirementDefinitionDto reqDefDto)
        {
            Assert.AreEqual(reqDefExpected.Title, reqDefDto.Title);
            Assert.AreEqual(reqDefExpected.Usage, reqDefDto.Usage);
            Assert.AreEqual(reqDefExpected.DefaultIntervalWeeks, reqDefDto.DefaultIntervalWeeks);
            Assert.AreEqual(reqDefExpected.SortKey, reqDefDto.SortKey);
            Assert.AreEqual(reqDefExpected.IsVoided, reqDefDto.IsVoided);
        }
    }
}
