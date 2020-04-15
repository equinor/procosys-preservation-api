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
        private readonly string TagFunctionCode3 = "TFC3";
        private readonly string TagFunctionDesc3 = "TFDesc3";
        private readonly string RegisterCode = "RC";

        [TestInitialize]
        public void Setup()
        {
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(r => r.Id).Returns(1);
            rdMock1.SetupGet(r => r.Plant).Returns(TestPlant);

            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(r => r.Id).Returns(2);
            rdMock2.SetupGet(r => r.Plant).Returns(TestPlant);

            var rdMock3 = new Mock<RequirementDefinition>();
            rdMock3.SetupGet(r => r.Id).Returns(3);
            rdMock3.SetupGet(r => r.Plant).Returns(TestPlant);
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode1, TagFunctionDesc1, RegisterCode);
            var tagFunctionWithRequirements = new TagFunction(TestPlant, TagFunctionCode2, TagFunctionDesc2, RegisterCode);
            var voidedRagFunctionWithRequirement = new TagFunction(TestPlant, TagFunctionCode3, TagFunctionDesc3, RegisterCode);
            voidedRagFunctionWithRequirement.Void();

            var tagFunctionRequirement1 = new TagFunctionRequirement(TestPlant, 1, rdMock1.Object);
            var tagFunctionRequirement2 = new TagFunctionRequirement(TestPlant, 1, rdMock2.Object);
            var tagFunctionRequirement3 = new TagFunctionRequirement(TestPlant, 1, rdMock3.Object);
            tagFunctionRequirement2.Void();
            tagFunctionWithRequirements.AddRequirement(tagFunctionRequirement1);
            tagFunctionWithRequirements.AddRequirement(tagFunctionRequirement2);
            voidedRagFunctionWithRequirement.AddRequirement(tagFunctionRequirement3);
            
            var tagFunctions = new List<TagFunction> {tagFunction, tagFunctionWithRequirements, voidedRagFunctionWithRequirement};
            
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
        public async Task GetAllNonVoidedWithNonVoidedRequirements_ReturnTagFunctionsWithRequirements()
        {
            var result = await _dut.GetAllNonVoidedWithRequirementsAsync();

            Assert.AreEqual(1, result.Count);
            var tagFunction = result.Single();
            Assert.AreEqual(TagFunctionCode2, tagFunction.Code);
            Assert.AreEqual(TagFunctionDesc2, tagFunction.Description);
            Assert.AreEqual(RegisterCode, tagFunction.RegisterCode);
            Assert.AreEqual(2, tagFunction.Requirements.Count);
            Assert.IsTrue(tagFunction.Requirements.Any(r => r.IsVoided));
            Assert.IsTrue(tagFunction.Requirements.Any(r => !r.IsVoided));
        }
    }
}
