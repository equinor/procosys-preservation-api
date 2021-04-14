using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetRequirementTypeById;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetRequirementTypeById
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
        private RequirementType _requirementTypeWithoutDefinition;
        private RequirementDefinition _requirementDefWithoutField;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _requirementTypeWithoutDefinition = new RequirementType(TestPlant, "T", "Title", RequirementTypeIcon.Area, 1);
                context.RequirementTypes.Add(_requirementTypeWithoutDefinition);
                context.SaveChangesAsync().Wait();

                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "T0", "D0", RequirementTypeIcon.Other, 999);
                _requirementDefWithoutField = _requirementType.RequirementDefinitions.Single();
                
                _requirementDefWithInfo = new RequirementDefinition(TestPlant, "D1", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithInfo);

                _requirementDefWithNumber = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithNumber);
                
                _requirementDefWithCheckbox = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithCheckbox);
                
                _requirementDefWithAttachment = new RequirementDefinition(TestPlant, "D4", 2, RequirementUsage.ForAll, 1);
                _requirementType.AddRequirementDefinition(_requirementDefWithAttachment);

                context.SaveChangesAsync().Wait();

                _infoField = AddInfoField(context, _requirementDefWithInfo, "LabelA");
                _numberField = AddNumberField(context, _requirementDefWithNumber, "LabelB", "UnitA", true);
                _checkboxField = AddCheckBoxField(context, _requirementDefWithCheckbox, "LabelC");
                _attachmentField = AddAttachmentField(context, _requirementDefWithAttachment, "LabelD");
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_KnownId_ShouldReturnRequirementTypeWithAllPropertiesSet()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var requirementType = result.Data;

                Assert.AreEqual(_requirementType.Code, requirementType.Code);
                Assert.AreEqual(_requirementType.Title, requirementType.Title);
                Assert.AreEqual(_requirementType.SortKey, requirementType.SortKey);
                Assert.IsFalse(requirementType.IsVoided);

                var requirementDefinitions = requirementType.RequirementDefinitions.ToList();
                Assert.AreEqual(5, requirementDefinitions.Count);

                var requirementDefWithoutFieldDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithoutField.Id);
                AssertRequirementDefinition(requirementDefWithoutFieldDto, _requirementDefWithoutField, null, false, false);

                var requirementDefWithInfoDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithInfo.Id);
                AssertRequirementDefinition(requirementDefWithInfoDto, _requirementDefWithInfo, _infoField, false, true);

                var requirementDefWithNumberDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithNumber.Id);
                AssertRequirementDefinition(requirementDefWithNumberDto, _requirementDefWithNumber, _numberField, true, true);

                var requirementDefWithCheckboxDto = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithCheckbox.Id);
                AssertRequirementDefinition(requirementDefWithCheckboxDto, _requirementDefWithCheckbox, _checkboxField, true, true);

                var requirementDefWithAttachment = requirementDefinitions.Single(rd => rd.Id == _requirementDefWithAttachment.Id);
                AssertRequirementDefinition(requirementDefWithAttachment, _requirementDefWithAttachment, _attachmentField, true, true);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(246), default);

                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_WithTagUsage_ShouldSetDefinitionInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithoutField.Id);
                Assert.IsFalse(reqDef.IsInUse);

                var project = AddProject(context, "P", "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                AddTag(context, project, TagType.Standard, "TagNo", "Tag description", journey.Steps.First(),
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, _requirementDefWithoutField)});
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithoutField.Id);
                Assert.IsTrue(reqDef.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_WithTagFunctionUsage_ShouldSetDefinitionInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithoutField.Id);
                Assert.IsFalse(reqDef.IsInUse);

                var tfWithRequirement = AddTagFunction(context, "TFC2", "RC1");
                tfWithRequirement.AddRequirement(new TagFunctionRequirement(TestPlant, 4, _requirementDefWithoutField));

                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithoutField.Id);
                Assert.IsTrue(reqDef.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_AfterPreservationRecorded_ShouldSetFieldInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithCheckbox.Id);
                Assert.IsFalse(reqDef.Fields.Single().IsInUse);

                var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var tagRequirement = new TagRequirement(TestPlant, 2, _requirementDefWithCheckbox);
                var tag = new Tag(TestPlant,
                    TagType.Standard, 
                    "TagNo",
                    "Description",
                    journey.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        tagRequirement
                    });
                tag.StartPreservation();

                context.Tags.Add(tag);
                context.SaveChangesAsync().Wait();

                tagRequirement.RecordCheckBoxValues(
                    new Dictionary<int, bool>
                    {
                        {_checkboxField.Id, true}
                    }, 
                    _requirementDefWithCheckbox);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                var reqDef = result.Data.RequirementDefinitions.Single(rd => rd.Id == _requirementDefWithCheckbox.Id);
                Assert.IsTrue(reqDef.Fields.Single().IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_WithAnyDefinition_ShouldSetRequirementTypeInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementType.Id), default);

                Assert.IsTrue(result.Data.IsInUse);
            }
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_WithoutDefinition_ShouldNotSetRequirementTypeInUse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetRequirementTypeByIdQueryHandler(context);
                var result = await dut.Handle(new GetRequirementTypeByIdQuery(_requirementTypeWithoutDefinition.Id), default);

                Assert.IsFalse(result.Data.IsInUse);
            }
        }

        private void AssertRequirementDefinition(
            RequirementDefinitionDetailDto requirementDefinitionDto,
            RequirementDefinition requirementDefinition,
            Field field,
            bool needsUserInput,
            bool inUse)
        {
            Assert.AreEqual(requirementDefinition.Title, requirementDefinitionDto.Title);
            Assert.AreEqual(requirementDefinition.DefaultIntervalWeeks, requirementDefinitionDto.DefaultIntervalWeeks);
            Assert.AreEqual(requirementDefinition.SortKey, requirementDefinitionDto.SortKey);
            Assert.AreEqual(needsUserInput, requirementDefinitionDto.NeedsUserInput);
            Assert.AreEqual(inUse, requirementDefinitionDto.IsInUse);
            Assert.IsFalse(requirementDefinitionDto.IsVoided);

            var fields = requirementDefinitionDto.Fields.ToList();
            if (field == null)
            {
                Assert.AreEqual(0, fields.Count);
            }
            else
            {
                Assert.AreEqual(1, fields.Count);
                Assert.AreEqual(field.Label, fields[0].Label);
                Assert.AreEqual(field.Unit, fields[0].Unit);
                Assert.AreEqual(field.FieldType, fields[0].FieldType);
                Assert.AreEqual(field.ShowPrevious, fields[0].ShowPrevious);
                Assert.AreEqual(field.SortKey, fields[0].SortKey);
                Assert.IsFalse(fields[0].IsVoided);
                Assert.IsFalse(fields[0].IsInUse);
            }
        }
    }
}
