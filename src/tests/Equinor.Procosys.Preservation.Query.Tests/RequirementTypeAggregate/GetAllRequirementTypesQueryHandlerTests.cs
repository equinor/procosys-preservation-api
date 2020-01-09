using System.Collections.Generic;
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
    public class GetAllRequirementTypesQueryHandlerTests
    {
        private Mock<IRequirementTypeRepository> _repoMock;
        private RequirementType _requirementType;
        private RequirementType _requirementTypeVoided;
        private RequirementDefinition _requirementDefinition;
        private RequirementDefinition _requirementDefinitionVoided;
        private Field _field;
        private Field _fieldVoided;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IRequirementTypeRepository>();

            _field = new Field("SchemaA", "LabelA", "UnitA", true, FieldType.Attachment, 10);
            _fieldVoided = new Field("SchemaB", "LabelB", "UnitB", false, FieldType.Number, 20);
            _fieldVoided.Void();

            _requirementDefinition = new RequirementDefinition("SchemaA", "TitleA", 4, 10);
            _requirementDefinitionVoided = new RequirementDefinition("SchemaB", "TitleB", 8, 20);
            _requirementDefinitionVoided.Void();

            _requirementType = new RequirementType("SchemaA", "CodeA", "TitleA", 10);
            _requirementTypeVoided = new RequirementType("SchemaB", "CodeB", "TitleB", 20);
            _requirementTypeVoided.Void();

            _requirementDefinition.AddField(_field);
            _requirementDefinition.AddField(_fieldVoided);
            _requirementDefinitionVoided.AddField(_field);
            _requirementDefinitionVoided.AddField(_fieldVoided);
            _requirementType.AddRequirementDefinition(_requirementDefinition);
            _requirementType.AddRequirementDefinition(_requirementDefinitionVoided);
            _requirementTypeVoided.AddRequirementDefinition(_requirementDefinition);
            _requirementTypeVoided.AddRequirementDefinition(_requirementDefinitionVoided);

            var requirementTypes = new List<RequirementType>
            {
                _requirementType,
                _requirementTypeVoided
            };
            _repoMock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(requirementTypes));
        }

        [TestMethod]
        public void HandleGetAllNonVoidedRequirementTypesQueryTest()
        {
            var handler = new GetAllRequirementTypesQueryHandler(_repoMock.Object);

            var result = handler.Handle(new GetAllRequirementTypesQuery(false), new CancellationToken()).Result;

            var requirementTypes = result.Data.ToList();
            var requirementDefinitions = requirementTypes.First().RequirementDefinitions.ToList();
            var fields = requirementDefinitions.First().Fields.ToList();

            Assert.AreEqual(1, requirementTypes.Count);
            Assert.AreEqual(1, requirementDefinitions.Count);
            Assert.AreEqual(1, fields.Count);

            Assert.AreEqual(_requirementType.Code, requirementTypes[0].Code);
            Assert.AreEqual(_requirementType.Title, requirementTypes[0].Title);
            Assert.AreEqual(_requirementType.SortKey, requirementTypes[0].SortKey);
            Assert.IsFalse(requirementTypes[0].IsVoided);

            Assert.AreEqual(_requirementDefinition.Title, requirementDefinitions[0].Title);
            Assert.AreEqual(_requirementDefinition.DefaultInterval, requirementDefinitions[0].DefaultInterval);
            Assert.AreEqual(_requirementDefinition.SortKey, requirementDefinitions[0].SortKey);
            Assert.IsFalse(requirementDefinitions[0].IsVoided);

            Assert.AreEqual(_field.Label, fields[0].Label);
            Assert.AreEqual(_field.Unit, fields[0].Unit);
            Assert.AreEqual(_field.FieldType, fields[0].FieldType);
            Assert.AreEqual(_field.ShowPrevious, fields[0].ShowPrevious);
            Assert.AreEqual(_field.SortKey, fields[0].SortKey);
            Assert.IsFalse(fields[0].IsVoided);
        }

        [TestMethod]
        public void HandleGetAllInclVoidedRequirementTypesQueryTest()
        {
            var handler = new GetAllRequirementTypesQueryHandler(_repoMock.Object);

            var result = handler.Handle(new GetAllRequirementTypesQuery(true), new CancellationToken()).Result;

            var requirementTypes = result.Data.ToList();
            var requirementDefinitions = requirementTypes.First().RequirementDefinitions.ToList();
            var fields = requirementDefinitions.First().Fields.ToList();

            Assert.IsTrue(requirementTypes.Count > 1);
            Assert.IsTrue(requirementDefinitions.Count > 1);
            Assert.IsTrue(fields.Count > 1);
        }
    }
}
