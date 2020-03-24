using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string TagFunctionCode = "TFC";
        private readonly string TagFunctionDesc = "TFDesc";
        private readonly string RegisterCode = "RC";

        [TestInitialize]
        public void Setup()
        {
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode, TagFunctionDesc, RegisterCode);
            var tagFunctions = new List<TagFunction> {tagFunction};
            
            var dbSetMock = tagFunctions.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.TagFunctions)
                .Returns(dbSetMock.Object);

            _dut = new TagFunctionRepository(ContextHelper.ContextMock.Object);
        }


        [TestMethod]
        public async Task GetTagFunctionByCode_KnownCode_ReturnTagFunction()
        {
            var result = await _dut.GetByCodeAsync(TagFunctionCode, RegisterCode);

            Assert.AreEqual(TagFunctionCode, result.Code);
            Assert.AreEqual(TagFunctionDesc, result.Description);
            Assert.AreEqual(RegisterCode, result.RegisterCode);
        }
    }
}
