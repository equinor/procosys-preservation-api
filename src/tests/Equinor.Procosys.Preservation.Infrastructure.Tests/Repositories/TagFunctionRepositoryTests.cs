using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class TagFunctionRepositoryTests : RepositoryTestBase
    {
        private TagFunctionRepository _dut;
        private readonly string TagFunctionCode1 = "TFC1";
        private readonly string TagFunctionDesc1 = "TFDesc1";
        private readonly string TagFunctionCode2 = "TFC2";
        private readonly string TagFunctionDesc2 = "TFDesc2";
        private readonly string RegisterCode = "RC";

        [TestInitialize]
        public void Setup()
        {
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode1, TagFunctionDesc1, RegisterCode);
            var tagFunctionWithRequirement = new TagFunction(TestPlant, TagFunctionCode2, TagFunctionDesc2, RegisterCode);
            tagFunctionWithRequirement.AddRequirement(new TagFunctionRequirement(TestPlant, 1, new RequirementDefinition(TestPlant, "D", 4, 0)));
            var tagFunctions = new List<TagFunction> {tagFunction, tagFunctionWithRequirement};
            
            var dbSetMock = tagFunctions.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.TagFunctions)
                .Returns(dbSetMock.Object);

            _dut = new TagFunctionRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetTagFunctionByCodes_KnownCodes_ReturnTagFunction()
        {
            var result = await _dut.GetByCodesAsync(TagFunctionCode1, RegisterCode);

            Assert.AreEqual(TagFunctionCode1, result.Code);
            Assert.AreEqual(TagFunctionDesc1, result.Description);
            Assert.AreEqual(RegisterCode, result.RegisterCode);
        }

        [TestMethod]
        public async Task GetTagFunctionByCodes_UnKnownCode_ReturnNull()
        {
            var result = await _dut.GetByCodesAsync(TagFunctionCode1, "X");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllWithRequirements_ReturnTagFunctionsWithRequirements()
        {
            var result = await _dut.GetAllWithRequirementsAsync();

            Assert.AreEqual(1, result.Count);
            var tagFunction = result.Single();
            Assert.AreEqual(TagFunctionCode2, tagFunction.Code);
            Assert.AreEqual(TagFunctionDesc2, tagFunction.Description);
            Assert.AreEqual(RegisterCode, tagFunction.RegisterCode);
        }
    }
}
