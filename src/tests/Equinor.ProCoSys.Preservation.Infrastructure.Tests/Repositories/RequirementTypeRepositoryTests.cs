using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class RequirementTypeRepositoryTests : RepositoryTestBase
    {
        private readonly int _reqDefId = 8;
        private List<RequirementType> _requirementTypes;
        private Mock<DbSet<RequirementType>> _rtSetMock;
        private Mock<DbSet<RequirementDefinition>> _rdSetMock;
        private Mock<DbSet<Field>> _fieldSetMock;

        private RequirementTypeRepository _dut;
        RequirementTypeIcon _reqIconOther = RequirementTypeIcon.Other;
        private RequirementDefinition _rd1;
        private Field _field;

        [TestInitialize]
        public void Setup()
        {
            _field = new Field(TestPlant, "L", FieldType.Info, 1);
            _rd1 = new RequirementDefinition(TestPlant, "RD1", 1, RequirementUsage.ForAll, 1);
            _rd1.SetProtectedIdForTesting(_reqDefId);
            _rd1.AddField(_field);
            var rd2 = new RequirementDefinition(TestPlant, "RD2", 1, RequirementUsage.ForAll, 2);
            var rd3 = new RequirementDefinition(TestPlant, "RD3", 1, RequirementUsage.ForAll, 3);

            var requirementType = new RequirementType(TestPlant, "C1", "T1", _reqIconOther, 0);
            requirementType.AddRequirementDefinition(_rd1);
            requirementType.AddRequirementDefinition(rd2);
            requirementType.AddRequirementDefinition(rd3);

            _requirementTypes = new List<RequirementType>
            {
                requirementType,
                new RequirementType(TestPlant, "C2", "T2", _reqIconOther,0),
                new RequirementType(TestPlant, "C3", "T3", _reqIconOther, 0)
            };

            _rtSetMock = _requirementTypes.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.RequirementTypes)
                .Returns(_rtSetMock.Object);

            var requirementDefinitions = new List<RequirementDefinition>
            {
                _rd1, rd2, rd3
            };

            _rdSetMock = requirementDefinitions.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.RequirementDefinitions)
                .Returns(_rdSetMock.Object);

            var fields = new List<Field>
            {
                _field
            };

            _fieldSetMock = fields.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Fields)
                .Returns(_fieldSetMock.Object);

            _dut = new RequirementTypeRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionByIdAsync_ShouldReturnReqDef_WhenKnownReqDefId()
        {
            var result = await _dut.GetRequirementDefinitionByIdAsync(_reqDefId);

            Assert.AreEqual(_reqDefId, result.Id);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionByIdAsync_ShouldReturnNull_WhenUnknownReqDefId()
        {
            var result = await _dut.GetRequirementDefinitionByIdAsync(2325);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionsByIdsAsync_ShouldReturnReqDef_WhenKnownReqDefId()
        {
            var result = await _dut.GetRequirementDefinitionsByIdsAsync(new List<int> { _reqDefId });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_reqDefId, result.First().Id);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionsByIdsAsync_ShouldReturnEmptyList_WhenUnKnownReqDefId()
        {
            var result = await _dut.GetRequirementDefinitionsByIdsAsync(new List<int> { 2325 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void RemoveRequirementDefinition_ShouldRemoveRequirementDefinitionFromContext()
        {
            _dut.RemoveRequirementDefinition(_rd1);

            _rdSetMock.Verify(s => s.Remove(_rd1), Times.Once);
        }

        [TestMethod]
        public void RemoveField_ShouldRemoveFieldFromContext()
        {
            _dut.RemoveField(_field);

            _fieldSetMock.Verify(s => s.Remove(_field), Times.Once);
        }
    }
}
