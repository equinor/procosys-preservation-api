using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    [TestClass]
    public class RecordCheckBoxCheckedCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 1;
        private const int FieldId = 11;
        private const int RdId = 21;

        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;

        private Requirement _requirement;
        private RecordCheckBoxCheckedCommand _recordCheckedCommand;
        private RecordCheckBoxCheckedCommand _recordUncheckedCommand;
        private RecordCheckBoxCheckedCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _recordCheckedCommand = new RecordCheckBoxCheckedCommand(TagId, FieldId, true);
            _recordUncheckedCommand = new RecordCheckBoxCheckedCommand(TagId, FieldId, false);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(f => f.Id).Returns(RdId);
            var fieldMock = new Mock<Field>("", "", FieldType.CheckBox, 0, null, null);
            fieldMock.SetupGet(f => f.Id).Returns(FieldId);
            rdMock.Object.AddField(fieldMock.Object);

            _requirement = new Requirement("", 2, rdMock.Object);
            var tag = new Tag("", "", "", "", "", "", "", "", "", "", "", new Mock<Step>().Object, new List<Requirement>
            {
                _requirement
            });
            _requirement.StartPreservation(new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_recordCheckedCommand.TagId))
                .Returns(Task.FromResult(tag));

            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByFieldIdAsync(_recordCheckedCommand.FieldId))
                .Returns(Task.FromResult(rdMock.Object));
            
            _dut = new RecordCheckBoxCheckedCommandHandler(
                _projectRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object);

            // Assert setup
            Assert.IsTrue(_requirement.HasActivePeriod);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordCheckBoxCheckedCommand_ShouldCreateNewCheckBoxChecked_WhenValueIsTrue()
        {
            // Act
            var result = await _dut.Handle(_recordCheckedCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var fieldValues = _requirement.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(CheckBoxChecked));
            Assert.AreEqual(FieldId, fv.FieldId);
        }

        [TestMethod]
        public async Task HandlingRecordCheckBoxCheckedCommand_ShouldUpdateStatusToReadyToBePreserved_WhenValueIsTrue()
        {
            // Act
            var result = await _dut.Handle(_recordCheckedCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordCheckBoxCheckedCommand_ShouldDoNothing_WhenValueIsFalseAndNoValueExistsInAdvance()
        {
            // Act
            var result = await _dut.Handle(_recordUncheckedCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordCheckBoxCheckedCommand_ShouldDeleteExistingValueAndNotCreateNew_WhenValueIsFalseAndValueExistsInAdvance()
        {
            // Arrange
            await _dut.Handle(_recordCheckedCommand, default);
            Assert.AreEqual(1, _requirement.ActivePeriod.FieldValues.Count);

            // Act
            var result = await _dut.Handle(_recordUncheckedCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public async Task HandlingRecordCheckBoxCheckedCommand_ShouldUpdateStatusBackToNeedsUserInput_WhenValueIsFalse()
        {
            // Arrange
            await _dut.Handle(_recordCheckedCommand, default);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);

            // Act
            var result = await _dut.Handle(_recordUncheckedCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }
    }
}
