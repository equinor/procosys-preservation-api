using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class RequirementTypeRepositoryTests : RepositoryTestBase
    {
        private const int ReqDefId = 8;
        private List<RequirementType> _requirementTypes;
        private Mock<DbSet<RequirementType>> _dbSetMock;

        private RequirementTypeRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            
            var requirementType = new RequirementType(TestPlant, "C1", "T1", 0);
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(r => r.Id).Returns(ReqDefId);
            rdMock1.SetupGet(r => r.Plant).Returns(TestPlant);
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(r => r.Plant).Returns(TestPlant);
            var rdMock3 = new Mock<RequirementDefinition>();
            rdMock3.SetupGet(r => r.Plant).Returns(TestPlant);
            requirementType.AddRequirementDefinition(rdMock1.Object);
            requirementType.AddRequirementDefinition(rdMock2.Object);
            requirementType.AddRequirementDefinition(rdMock3.Object);
            
            _requirementTypes = new List<RequirementType>
            {
                requirementType,
                new RequirementType(TestPlant, "C2", "T2", 0),
                new RequirementType(TestPlant, "C3", "T3", 0)
            };
            
            _dbSetMock = _requirementTypes.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.RequirementTypes)
                .Returns(_dbSetMock.Object);

            _dut = new RequirementTypeRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionById_KnownReqDefId_ReturnReqDef()
        {
            var result = await _dut.GetRequirementDefinitionByIdAsync(ReqDefId);

            Assert.AreEqual(ReqDefId, result.Id);
        }

        [TestMethod]
        public async Task GetRequirementDefinitionsByIds_KnownReqDefId_ReturnReqDef()
        {
            var result = await _dut.GetRequirementDefinitionsByIdsAsync(new List<int> {ReqDefId});

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(ReqDefId, result.First().Id);
        }
    }
}
