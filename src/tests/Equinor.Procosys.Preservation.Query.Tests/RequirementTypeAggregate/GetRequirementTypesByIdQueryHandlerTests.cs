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
    public class GetRequirementTypesByIdQueryHandlerTests
    {
        private Mock<IRequirementTypeRepository> _repoMock;
        private RequirementType _requirementTypeVoided;
        private RequirementDefinition _requirementDefinitionVoided;
        private Field _fieldVoided;
        private int _id = 1;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IRequirementTypeRepository>();

            _fieldVoided = new Field("SchemaB", "LabelB", FieldType.Number, 20, "UnitB", false);
            _fieldVoided.Void();

            _requirementDefinitionVoided = new RequirementDefinition("SchemaB", "TitleB", 8, 20);
            _requirementDefinitionVoided.Void();

            _requirementTypeVoided = new RequirementType("SchemaB", "CodeB", "TitleB", 20);
            _requirementTypeVoided.Void();

            _requirementDefinitionVoided.AddField(_fieldVoided);
            _requirementTypeVoided.AddRequirementDefinition(_requirementDefinitionVoided);

            _repoMock.Setup(r => r.GetByIdAsync(_id)).Returns(Task.FromResult(_requirementTypeVoided));
        }

        [TestMethod]
        public void HandleGetRequirementTypeByIdQueryTest()
        {
            var handler = new GetRequirementTypeByIdQueryHandler(_repoMock.Object);

            var result = handler.Handle(new GetRequirementTypeByIdQuery(_id), new CancellationToken()).Result;

            var requirementType = result.Data;
            var requirementDefinitions = requirementType.RequirementDefinitions.ToList();
            var fields = requirementDefinitions.First().Fields.ToList();

            Assert.AreEqual(1, requirementDefinitions.Count);
            Assert.AreEqual(1, fields.Count);

            Assert.AreEqual(_requirementTypeVoided.Code, requirementType.Code);
            Assert.AreEqual(_requirementTypeVoided.Title, requirementType.Title);
            Assert.AreEqual(_requirementTypeVoided.SortKey, requirementType.SortKey);
            Assert.IsTrue(requirementType.IsVoided);

            Assert.AreEqual(_requirementDefinitionVoided.Title, requirementDefinitions[0].Title);
            Assert.AreEqual(_requirementDefinitionVoided.DefaultInterval, requirementDefinitions[0].DefaultInterval);
            Assert.AreEqual(_requirementDefinitionVoided.SortKey, requirementDefinitions[0].SortKey);
            Assert.IsTrue(requirementDefinitions[0].IsVoided);

            Assert.AreEqual(_fieldVoided.Label, fields[0].Label);
            Assert.AreEqual(_fieldVoided.Unit, fields[0].Unit);
            Assert.AreEqual(_fieldVoided.FieldType, fields[0].FieldType);
            Assert.AreEqual(_fieldVoided.ShowPrevious, fields[0].ShowPrevious);
            Assert.AreEqual(_fieldVoided.SortKey, fields[0].SortKey);
            Assert.IsTrue(fields[0].IsVoided);
        }
    }
}
