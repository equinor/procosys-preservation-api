using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.RecordValues
{
    [TestClass]
    public class RecordValuesCommandHandlerTests : CommandHandlerTestsBase
    {
        private TagRequirement _requirement;
        private RecordValuesCommand _recordValuesCommandWithCheckedCheckBoxAndNumber;
        private RecordValuesCommand _recordValuesCommandWithNullAsNumber;
        private RecordValuesCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var _tagId = 1;
            var _checkBoxFieldId = 11;
            var _numberFieldId = 12;
            var _reqId = 21;

            _recordValuesCommandWithCheckedCheckBoxAndNumber = new RecordValuesCommand(
                _tagId, 
                _reqId, 
                new Dictionary<int, string>
                {
                    {_checkBoxFieldId, "true"},
                    {_numberFieldId, "21"}
                }, 
                null);

            _recordValuesCommandWithNullAsNumber = new RecordValuesCommand(
                _tagId, 
                _reqId, 
                new Dictionary<int, string>
                {
                    {_numberFieldId, null}
                }, 
                null);

            var requirementDefinitionWith2FieldsMock = new Mock<RequirementDefinition>();
            requirementDefinitionWith2FieldsMock.SetupGet(r => r.Id).Returns(_reqId);
            requirementDefinitionWith2FieldsMock.SetupGet(r => r.Plant).Returns(TestPlant);
            
            var checkBoxFieldMock = new Mock<Field>(TestPlant, "", FieldType.CheckBox, 0, null, null);
            checkBoxFieldMock.SetupGet(f => f.Id).Returns(_checkBoxFieldId);
            checkBoxFieldMock.SetupGet(f => f.Plant).Returns(TestPlant);
            requirementDefinitionWith2FieldsMock.Object.AddField(checkBoxFieldMock.Object);

            var numberFieldMock = new Mock<Field>(TestPlant, "", FieldType.Number, 0, "mm", false);
            numberFieldMock.SetupGet(f => f.Id).Returns(_numberFieldId);
            numberFieldMock.SetupGet(f => f.Plant).Returns(TestPlant);
            requirementDefinitionWith2FieldsMock.Object.AddField(numberFieldMock.Object);

            var requirementMock = new Mock<TagRequirement>(TestPlant, 2, requirementDefinitionWith2FieldsMock.Object);
            requirementMock.SetupGet(r => r.Id).Returns(_reqId);
            requirementMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _requirement = requirementMock.Object;

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement>
            {
                _requirement
            });

            tag.StartPreservation();
            Assert.AreEqual(PreservationStatus.Active, tag.Status);
            Assert.IsTrue(_requirement.HasActivePeriod);

            var _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(tag));

            var _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(_reqId))
                .Returns(Task.FromResult(requirementDefinitionWith2FieldsMock.Object));
            
            _dut = new RecordValuesCommandHandler(
                _projectRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldMakeActivePeriodReadyToBePreserved_WhenRecordingAllRequiredFields()
        {
            // Assert setup
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            // Act
            await _dut.Handle(_recordValuesCommandWithCheckedCheckBoxAndNumber, default);
            
            // Assert
            Assert.AreEqual(2, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldDoNothing_WhenRecordingNullAsNumber()
        {
            // Assert setup
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);

            // Act
            await _dut.Handle(_recordValuesCommandWithNullAsNumber, default);
            
            // Assert
            Assert.AreEqual(0, _requirement.ActivePeriod.FieldValues.Count);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, _requirement.ActivePeriod.Status);
        }

        [TestMethod]
        public async Task HandlingRecordValuesCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_recordValuesCommandWithCheckedCheckBoxAndNumber, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
