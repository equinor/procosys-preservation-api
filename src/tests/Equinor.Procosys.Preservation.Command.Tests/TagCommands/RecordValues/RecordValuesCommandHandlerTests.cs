using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.RecordValues;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FieldValue = Equinor.Procosys.Preservation.Command.TagCommands.RecordValues.FieldValue;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordValues
{
    [TestClass]
    public class RecordValuesCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string Comment = "Comment";
        private const int TagId = 1;
        private const int CheckBoxFieldId = 11;
        private const int NumberFieldId = 12;
        private const double Number = 1282.91;
        private const int ReqId = 21;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;

        private Requirement _requirement;
        private RecordValuesCommand _recordValuesCommandWithCheckedCheckBox;
        private RecordValuesCommand _recordValuesCommandWithUncheckedCheckBox;
        private RecordValuesCommand _recordValuesCommandWithNaNumber;
        private RecordValuesCommand _recordValuesCommandWithNullNumber;
        private RecordValuesCommand _recordValuesCommandWithNormalNumber;
        private RecordValuesCommand _recordValuesCommandWithCheckedCheckBoxAndNaAsNumber;
        private RecordValuesCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _recordValuesCommandWithCheckedCheckBox = new RecordValuesCommand(
                TagId,
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(CheckBoxFieldId, "true")
                }, 
                Comment);
            
            _recordValuesCommandWithUncheckedCheckBox = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(CheckBoxFieldId, "false")
                }, 
                Comment);
            
            _recordValuesCommandWithNaNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(NumberFieldId, "n/a")
                }, 
                Comment);
            
            _recordValuesCommandWithNullNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(NumberFieldId, null)
                }, 
                Comment);

            _recordValuesCommandWithNormalNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(NumberFieldId, Number.ToString("F2"))
                }, 
                Comment);
            
            _recordValuesCommandWithCheckedCheckBoxAndNaAsNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(CheckBoxFieldId, "true"),
                    new FieldValue(NumberFieldId, "n/a")
                }, 
                Comment);

            var requirementDefinitionWith2FieldsMock = new Mock<RequirementDefinition>();
            requirementDefinitionWith2FieldsMock.SetupGet(r => r.Id).Returns(ReqId);
            
            var checkBoxFieldMock = new Mock<Field>("", "", FieldType.CheckBox, 0, null, null);
            checkBoxFieldMock.SetupGet(f => f.Id).Returns(CheckBoxFieldId);
            requirementDefinitionWith2FieldsMock.Object.AddField(checkBoxFieldMock.Object);

            var numberFieldMock = new Mock<Field>("", "", FieldType.Number, 0, "mm", false);
            numberFieldMock.SetupGet(f => f.Id).Returns(NumberFieldId);
            requirementDefinitionWith2FieldsMock.Object.AddField(numberFieldMock.Object);

            var requirementMock = new Mock<Requirement>("", 2, requirementDefinitionWith2FieldsMock.Object);
            requirementMock.SetupGet(r => r.Id).Returns(ReqId);
            _requirement = requirementMock.Object;

            var tag = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", new Mock<Step>().Object, new List<Requirement>
            {
                _requirement
            });
            tag.StartPreservation(new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(TagId))
                .Returns(Task.FromResult(tag));

            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(ReqId))
                .Returns(Task.FromResult(requirementDefinitionWith2FieldsMock.Object));
            
            _dut = new RecordValuesCommandHandler(
                _projectRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object);

            // Assert setup
            Assert.AreEqual(PreservationStatus.Active, tag.Status);
            Assert.IsTrue(_requirement.HasActivePeriod);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithCheckBoxChecked_ShouldCreateNewCheckBoxChecked_WhenValueIsTrue()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithCheckedCheckBox, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var fieldValues = _requirement.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(CheckBoxChecked));
            Assert.AreEqual(CheckBoxFieldId, fv.FieldId);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithNaAsNumber_ShouldCreateNumberValueWithNullValue()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithNaNumber, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var fieldValues = _requirement.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberFieldId, fv.FieldId);
            Assert.IsNull(((NumberValue)fv).Value);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithNumber_ShouldCreateNumberValueWithCorrectValue()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithNormalNumber, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var fieldValues = _requirement.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberFieldId, fv.FieldId);
            var numberValue = (NumberValue)fv;
            Assert.IsTrue( numberValue.Value.HasValue);
            Assert.AreEqual(Number, numberValue.Value.Value);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithComment_ShouldUpdateCommentOnActivePeriod()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithCheckedCheckBox, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Comment, _requirement.ActivePeriod.Comment);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithCheckBoxUnchecked_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithUncheckedCheckBox, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_WithNoNumber_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            // Act
            var result = await _dut.Handle(_recordValuesCommandWithNullNumber, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldDeleteExistingCheckBoxValueAndNotCreateNew_WhenCheckBoxIsUncheckedAndValueExistsInAdvance()
        {
            // Arrange
            await _dut.Handle(_recordValuesCommandWithCheckedCheckBox, default);
            Assert.AreEqual(1, _requirement.ActivePeriod.FieldValues.Count);

            // Act
            await _dut.Handle(_recordValuesCommandWithUncheckedCheckBox, default);

            // Assert
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldDeleteExistingNumberValueAndNotCreateNew_WhenNumberIsNullAndValueExistsInAdvance()
        {
            // Arrange
            await _dut.Handle(_recordValuesCommandWithNormalNumber, default);
            Assert.AreEqual(1, _requirement.ActivePeriod.FieldValues.Count);

            // Act
            await _dut.Handle(_recordValuesCommandWithNullNumber, default);

            // Assert
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldComeToReadyToBePreserved_WhenRecordRealValues_OneByOne()
        {
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            await _dut.Handle(_recordValuesCommandWithCheckedCheckBox, default);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            await _dut.Handle(_recordValuesCommandWithNaNumber, default);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldComeToReadyToBePreserved_WhenRecordRealValues_AllRequiredAtOnce()
        {
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            await _dut.Handle(_recordValuesCommandWithCheckedCheckBoxAndNaAsNumber, default);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldReverseToNeedsUserInput_WhenBlankingCheckBox()
        {
            await _dut.Handle(_recordValuesCommandWithCheckedCheckBoxAndNaAsNumber, default);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);

            await _dut.Handle(_recordValuesCommandWithUncheckedCheckBox, default);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }
    }
}
