using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MiscCommands.Clone;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    [TestClass]
    public class CloneCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _sourcePlant = "SOURCE";
        private CloneCommand _command;
        private CloneCommandHandler _dut;
        private readonly PlantProvider _plantProvider = new PlantProvider(TestPlant);
        private ModeRepository _modeRepository;
        private readonly List<Mode> _sourceModes = new List<Mode>();
        private ResponsibleRepository _responsibleRepository;
        private readonly List<Responsible> _sourceResponsibles = new List<Responsible>();
        private RequirementTypeRepository _requirementTypeRepository;
        private readonly List<RequirementType> _sourceRequirementTypes = new List<RequirementType>();
        private TagFunctionRepository _tagFunctionRepository;
        private readonly List<TagFunction> _sourceTagFunctions = new List<TagFunction>();

        [TestInitialize]
        public void Setup()
        {
            var reqDefId = 0;
            _modeRepository = new ModeRepository(_plantProvider, _sourceModes);
            _sourceModes.Add(new Mode(TestPlant, "ModeA"));
            _sourceModes.Add(new Mode(TestPlant, "ModeB"));

            _responsibleRepository = new ResponsibleRepository(_plantProvider, _sourceResponsibles);
            _sourceResponsibles.Add(new Responsible(TestPlant, "ResponsibleCodeA", "ResponsibleTitleA"));
            _sourceResponsibles.Add(new Responsible(TestPlant, "ResponsibleCodeB", "ResponsibleTitleB"));
            
            _requirementTypeRepository = new RequirementTypeRepository(_plantProvider, _sourceRequirementTypes);
            var requirementTypeA = new RequirementType(TestPlant, "RequirementTypeCodeA", "RequirementTypeTitleA", 1);
            var reqDefA1 = new RequirementDefinition(TestPlant, "RequirementDefCodeA1", 1, 2);
            reqDefA1.SetProtectedIdForTesting(++reqDefId);
            requirementTypeA.AddRequirementDefinition(reqDefA1);
            var reqDefA2 = new RequirementDefinition(TestPlant, "RequirementDefCodeA2", 3, 4);
            reqDefA2.SetProtectedIdForTesting(++reqDefId);
            requirementTypeA.AddRequirementDefinition(reqDefA2);
            var numberField = new Field(TestPlant, "LabelA", FieldType.Number, 1, "UnitA", true);
            reqDefA1.AddField(numberField);
            var requirementTypeB = new RequirementType(TestPlant, "RequirementTypeCodeB", "RequirementTypeTitleB", 2);
            _sourceRequirementTypes.Add(requirementTypeA);
            _sourceRequirementTypes.Add(requirementTypeB);

            _tagFunctionRepository = new TagFunctionRepository(_plantProvider, _sourceTagFunctions);
            var tagFunctionA = new TagFunction(TestPlant, "TagFunctionCodeA", "TagFunctionDescA", "RegisterCodeA");
            var tagFunctionB = new TagFunction(TestPlant, "TagFunctionCodeB", "TagFunctionDescB", "RegisterCodeB");
            _sourceTagFunctions.Add(tagFunctionA);
            _sourceTagFunctions.Add(tagFunctionB);

            _command = new CloneCommand(_sourcePlant, TestPlant);
            _dut = new CloneCommandHandler(
                _plantProvider,
                UnitOfWorkMock.Object,
                _modeRepository,
                _responsibleRepository,
                _requirementTypeRepository,
                _tagFunctionRepository);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneModes()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedModes(_sourceModes, _modeRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneResponsibles()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedResponsibles(_sourceResponsibles, _responsibleRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneRequirementTypes()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedRequirementTypes(_sourceRequirementTypes, _requirementTypeRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneTagFunctions()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedTagFunctions(_sourceTagFunctions, _tagFunctionRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertClonedModes(List<Mode> sourceModes, List<Mode> result)
        {
            Assert.AreEqual(sourceModes.Count, result.Count);
            for (var i = 0; i < sourceModes.Count; i++)
            {
                var source = sourceModes.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Title, clone.Title);
            }
        }

        private void AssertClonedResponsibles(List<Responsible> sourceResponsibles, List<Responsible> result)
        {
            Assert.AreEqual(sourceResponsibles.Count, result.Count);
            for (var i = 0; i < sourceResponsibles.Count; i++)
            {
                var source = sourceResponsibles.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Code, clone.Code);
                Assert.AreEqual(source.Title, clone.Title);
            }
        }

        private void AssertClonedTagFunctions(List<TagFunction> sourceTagFunctions, List<TagFunction> result)
        {
            Assert.AreEqual(sourceTagFunctions.Count, result.Count);
            for (var i = 0; i < sourceTagFunctions.Count; i++)
            {
                var source = sourceTagFunctions.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Code, clone.Code);
                Assert.AreEqual(source.Description, clone.Description);
                Assert.AreEqual(source.RegisterCode, clone.RegisterCode);
                AssertClonedRequirements(source.Requirements, clone.Requirements);
            }
        }

        private void AssertClonedRequirements(IReadOnlyCollection<TagFunctionRequirement> sourceRequirements, IReadOnlyCollection<TagFunctionRequirement> result)
        {
            Assert.AreEqual(sourceRequirements.Count, result.Count);
        }

        private void AssertClonedRequirementTypes(List<RequirementType> sourceRequirementTypes, List<RequirementType> result)
        {
            Assert.AreEqual(sourceRequirementTypes.Count, result.Count);
            for (var i = 0; i < sourceRequirementTypes.Count; i++)
            {
                var source = sourceRequirementTypes.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Code, clone.Code);
                Assert.AreEqual(source.Title, clone.Title);
                Assert.AreEqual(source.SortKey, clone.SortKey);
                AssertClonedRequirementDefs(source.RequirementDefinitions, clone.RequirementDefinitions);
            }
        }

        private void AssertClonedRequirementDefs(
            IReadOnlyCollection<RequirementDefinition> sourceRequirementDefinitions,
            IReadOnlyCollection<RequirementDefinition> result)
        {
            Assert.AreEqual(sourceRequirementDefinitions.Count, result.Count);

            for (var i = 0; i < sourceRequirementDefinitions.Count; i++)
            {
                var source = sourceRequirementDefinitions.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Title, clone.Title);
                Assert.AreEqual(source.DefaultIntervalWeeks, clone.DefaultIntervalWeeks);
                Assert.AreEqual(source.SortKey, clone.SortKey);
                AssertClonedRequirementFields(source.Fields, clone.Fields);
            }
        }

        private void AssertClonedRequirementFields(IReadOnlyCollection<Field> sourceFields, IReadOnlyCollection<Field> result)
        {
            Assert.AreEqual(sourceFields.Count, result.Count);
            for (var i = 0; i < sourceFields.Count; i++)
            {
                var source = sourceFields.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Label, clone.Label);
                Assert.AreEqual(source.FieldType, clone.FieldType);
                Assert.AreEqual(source.SortKey, clone.SortKey);
                Assert.AreEqual(source.Unit, clone.Unit);
                Assert.AreEqual(source.ShowPrevious, clone.ShowPrevious);
            }
        }
    }
}
