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
    public class GetRequirementTypeByIdQueryHandlerTests : ReadOnlyTestsBase
    {
        private RequirementType _requirementType;
        private RequirementDefinition _requirementDefWithInfo;
        private RequirementDefinition _requirementDefWithNumber;
        private RequirementDefinition _requirementDefWithCheckbox;
        private RequirementDefinition _requirementDefWithAttachment;
        private Field _infoField;
        private Field _numberField;
        private Field _checkboxField;
        private Field _attachmentField;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1", 999);
                _requirementDefWithInfo = _requirementType.RequirementDefinitions.Single();
                _infoField = AddInfoField(context, _requirementDefWithInfo, "LabelA");

                _requirementDefWithNumber = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithNumber);
                
                _requirementDefWithCheckbox = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithCheckbox);
                
                _requirementDefWithAttachment = new RequirementDefinition(TestPlant, "D4", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithAttachment);

                context.SaveChangesAsync().Wait();

                _numberField = AddNumberField(context, _requirementDefWithNumber, "LabelB", "UnitA", true);
                _checkboxField = AddCheckBoxField(context, _requirementDefWithCheckbox, "LabelC");
                _attachmentField = AddAttachmentField(context, _requirementDefWithAttachment, "LabelD");
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_KnownId_ShouldReturnRequirementTypeWithAllPropertiesSet()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var requirementType = result.Data;

                Assert.AreEqual(_requirementType.Code, requirementType.Code);
                Assert.AreEqual(_requirementType.Title, requirementType.Title);
                Assert.AreEqual(_requirementType.SortKey, requirementType.SortKey);
                Assert.IsFalse(requirementType.IsVoided);

                var requirementDefinitions = requirementType.RequirementDefinitions.ToList();
                Assert.AreEqual(4, requirementDefinitions.Count);

                var requirementDefWithInfoDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithInfo.Id);
                AssertRequirementDefinition(requirementDefWithInfoDto, _requirementDefWithInfo, _infoField, false);

                var requirementDefWithNumberDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithNumber.Id);
                AssertRequirementDefinition(requirementDefWithNumberDto, _requirementDefWithNumber, _numberField, true);

                var requirementDefWithCheckboxDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithCheckbox.Id);
                AssertRequirementDefinition(requirementDefWithCheckboxDto, _requirementDefWithCheckbox, _checkboxField,
                    true);

                var requirementDefWithAttachment = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithAttachment.Id);
                AssertRequirementDefinition(requirementDefWithAttachment, _requirementDefWithAttachment, _attachmentField, true);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(246), default);

                Assert.IsNull(result.Data);
            }
        }

        private void AssertRequirementDefinition(
            RequirementDefinitionDto requirementDefinitionDto,
            RequirementDefinition requirementDefinition,
            Field field,
            bool needsUserInput)
        {
            Assert.AreEqual(requirementDefinition.Title, requirementDefinitionDto.Title);
            Assert.AreEqual(requirementDefinition.DefaultIntervalWeeks, requirementDefinitionDto.DefaultIntervalWeeks);
            Assert.AreEqual(requirementDefinition.SortKey, requirementDefinitionDto.SortKey);
            Assert.AreEqual(needsUserInput, requirementDefinitionDto.NeedsUserInput);
            Assert.IsFalse(requirementDefinitionDto.IsVoided);

            var fields = requirementDefinitionDto.Fields.ToList();
            Assert.AreEqual(1, fields.Count);
            Assert.AreEqual(field.Label, fields[0].Label);
            Assert.AreEqual(field.Unit, fields[0].Unit);
            Assert.AreEqual(field.FieldType, fields[0].FieldType);
            Assert.AreEqual(field.ShowPrevious, fields[0].ShowPrevious);
            Assert.AreEqual(field.SortKey, fields[0].SortKey);
            Assert.IsFalse(fields[0].IsVoided);
        }
    }
}
