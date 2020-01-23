using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class GetRequirementTypeByIdQueryHandlerTests
    {
        private Mock<IRequirementTypeRepository> _repoMock;
        private RequirementType _requirementType;
        private RequirementDefinition _requirementDefWithInfo;
        private RequirementDefinition _requirementDefWithNumber;
        private RequirementDefinition _requirementDefWithCheckbox;
        private RequirementDefinition _requirementDefWithAttachment;
        private Field _infoField;
        private Field _numberField;
        private Field _checkboxField;
        private Field _attachmentField;
        private int _id = 1;

        private GetRequirementTypeByIdQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IRequirementTypeRepository>();

            _infoField = new Field("SchemaA", "LabelA", FieldType.Info, 10);
            _numberField = new Field("SchemaB", "LabelB", FieldType.Number, 20, "UnitA", true);
            _checkboxField = new Field("SchemaC", "LabelC", FieldType.CheckBox, 30);
            _attachmentField = new Field("SchemaD", "LabelD", FieldType.Attachment, 40);

            _requirementDefWithInfo = new RequirementDefinition("SchemaA", "DefWithInfo", 8, 140);
            _requirementDefWithNumber = new RequirementDefinition("SchemaB", "DefWithNumber", 8, 130);
            _requirementDefWithCheckbox = new RequirementDefinition("SchemaC", "DefWithCheckbox", 8, 120);
            _requirementDefWithAttachment = new RequirementDefinition("SchemaD", "DefWithAttachment", 8, 110);

            _requirementType = new RequirementType("SchemaA", "CodeA", "TitleA", 10);

            _requirementDefWithInfo.AddField(_infoField);
            _requirementDefWithNumber.AddField(_numberField);
            _requirementDefWithCheckbox.AddField(_checkboxField);
            _requirementDefWithAttachment.AddField(_attachmentField);
            _requirementType.AddRequirementDefinition(_requirementDefWithInfo);
            _requirementType.AddRequirementDefinition(_requirementDefWithNumber);
            _requirementType.AddRequirementDefinition(_requirementDefWithCheckbox);
            _requirementType.AddRequirementDefinition(_requirementDefWithAttachment);

            _repoMock.Setup(r => r.GetByIdAsync(_id)).Returns(Task.FromResult(_requirementType));

            _dut = new GetRequirementTypeByIdQueryHandler(_repoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetRequirementTypeByIdQuery_ShouldGetRequirementTypeWithAllPropertiesSet()
        {
            var result = await _dut.Handle(new GetRequirementTypeByIdQuery(_id), new CancellationToken());

            var requirementType = result.Data;

            Assert.AreEqual(_requirementType.Code, requirementType.Code);
            Assert.AreEqual(_requirementType.Title, requirementType.Title);
            Assert.AreEqual(_requirementType.SortKey, requirementType.SortKey);
            Assert.IsFalse(requirementType.IsVoided);

            var requirementDefinitions = requirementType.RequirementDefinitions.ToList();
            Assert.AreEqual(4, requirementDefinitions.Count);

            var requirementDefWithInfo = requirementDefinitions.Single(rd => rd.Title == "DefWithInfo");
            AssertRequirementDefinition(requirementDefWithInfo, _requirementDefWithInfo, _infoField, false);

            var requirementDefWithNumber = requirementDefinitions.Single(rd => rd.Title == "DefWithNumber");
            AssertRequirementDefinition(requirementDefWithNumber, _requirementDefWithNumber, _numberField, true);

            var requirementDefWithCheckbox = requirementDefinitions.Single(rd => rd.Title == "DefWithCheckbox");
            AssertRequirementDefinition(requirementDefWithCheckbox, _requirementDefWithCheckbox, _checkboxField, true);

            var requirementDefWithAttachment = requirementDefinitions.Single(rd => rd.Title == "DefWithAttachment");
            AssertRequirementDefinition(requirementDefWithAttachment, _requirementDefWithAttachment, _attachmentField, true);
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
