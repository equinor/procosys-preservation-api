using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class TagFunctionRepositoryTests : RepositoryTestBase
    {
        private TagFunctionRepository _dut;
        private readonly string _tfWithoutReqCode = "TFC1";
        private readonly string _tfWithoutReqDesc = "TFDesc1";
        private readonly string _tf1WithReqCode = "TFC2";
        private readonly string _tf1WithReqDesc = "TFDesc2";
        private readonly string _tf2WithReqCode = "TFC3";
        private readonly string _tf2WithReqDesc = "TFDesc3";
        private readonly string _registerCode = "RC";
        private TagFunction _tfWithoutReq;
        private TagFunction _tf1WithReq;
        private TagFunction _tf2WithReq;
        private readonly int _tfWithoutReqId = 1;
        private readonly int _tf1WithReqId = 2;
        private readonly int _tf2WithReqId = 3;
        private TagFunctionRequirement _tfReq1_1;
        private TagFunctionRequirement _tfReq1_2;
        private readonly int _tfReq1_1Id = 11;
        private readonly int _tfReq1_2Id = 12;

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
            _tfWithoutReq = new TagFunction(TestPlant, _tfWithoutReqCode, _tfWithoutReqDesc, _registerCode);
            _tfWithoutReq.SetProtectedIdForTesting(_tfWithoutReqId);
            _tf1WithReq = new TagFunction(TestPlant, _tf1WithReqCode, _tf1WithReqDesc, _registerCode);
            _tf1WithReq.SetProtectedIdForTesting(_tf1WithReqId);
            _tf2WithReq = new TagFunction(TestPlant, _tf2WithReqCode, _tf2WithReqDesc, _registerCode);
            _tf2WithReq.SetProtectedIdForTesting(_tf2WithReqId);
            _tfReq1_1 = new TagFunctionRequirement(TestPlant, 1, rdMock1.Object);
            _tfReq1_1.SetProtectedIdForTesting(_tfReq1_1Id);
            _tfReq1_2 = new TagFunctionRequirement(TestPlant, 1, rdMock2.Object);
            _tfReq1_2.SetProtectedIdForTesting(_tfReq1_2Id);

            _tf1WithReq.AddRequirement(_tfReq1_1);
            _tf1WithReq.AddRequirement(_tfReq1_2);

            var tfReq2_1 = new TagFunctionRequirement(TestPlant, 1, rdMock3.Object);
            _tf2WithReq.AddRequirement(tfReq2_1);
            
            var tagFunctions = new List<TagFunction> {_tfWithoutReq, _tf1WithReq, _tf2WithReq};
            
            var dbSetMock = tagFunctions.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.TagFunctions)
                .Returns(dbSetMock.Object);

            _dut = new TagFunctionRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetByCodesAsync_KnownCode_ReturnTagFunction()
        {
            var result = await _dut.GetByCodesAsync(_tfWithoutReqCode, _registerCode);

            Assert.AreEqual(_tfWithoutReqCode, result.Code);
            Assert.AreEqual(_tfWithoutReqDesc, result.Description);
            Assert.AreEqual(_registerCode, result.RegisterCode);
        }

        [TestMethod]
        public async Task GetByCodesAsync_UnKnownCode_ReturnNull()
        {
            var result = await _dut.GetByCodesAsync(_tfWithoutReqCode, "X");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllNonVoidedWithRequirementsAsync_ReturnTagFunctionsWithRequirementsOnly()
        {
            var result = await _dut.GetAllNonVoidedWithRequirementsAsync();

            Assert.AreEqual(2, result.Count);
            Assert.IsFalse(result.Any(tf => tf.Id == _tfWithoutReqId));

            var tagFunction1 = result.Single(tf => tf.Id == _tf1WithReqId);
            Assert.AreEqual(_tf1WithReqCode, tagFunction1.Code);
            Assert.AreEqual(_tf1WithReqDesc, tagFunction1.Description);
            Assert.AreEqual(_registerCode, tagFunction1.RegisterCode);
            Assert.AreEqual(2, tagFunction1.Requirements.Count);

            var tagFunction2 = result.Single(tf => tf.Id == _tf2WithReqId);
            Assert.AreEqual(_tf2WithReqCode, tagFunction2.Code);
            Assert.AreEqual(_tf2WithReqDesc, tagFunction2.Description);
            Assert.AreEqual(_registerCode, tagFunction2.RegisterCode);
            Assert.AreEqual(1, tagFunction2.Requirements.Count);
        }

        [TestMethod]
        public async Task GetAllNonVoidedWithRequirementsAsync_DoNotReturnVoidedTagFunctions()
        {
            _tf1WithReq.IsVoided = true;
            var result = await _dut.GetAllNonVoidedWithRequirementsAsync();

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result.Any(tf => tf.Id == _tfWithoutReqId));
            Assert.IsNotNull(result.Single(tf => tf.Id == _tf2WithReqId));
        }

        [TestMethod]
        public async Task GetAllNonVoidedWithRequirementsAsync_ReturnBothVoidedAndNonVoidedTagFunctionRequirements()
        {
            _tfReq1_1.IsVoided = true;
            var result = await _dut.GetAllNonVoidedWithRequirementsAsync();

            Assert.AreEqual(2, result.Count);
            var tagFunction1 = result.Single(tf => tf.Id == _tf1WithReqId);
            Assert.AreEqual(2, tagFunction1.Requirements.Count);
            var req1 = tagFunction1.Requirements.Single(r => r.Id == _tfReq1_1Id);
            Assert.IsTrue(req1.IsVoided);
            var req2 = tagFunction1.Requirements.Single(r => r.Id == _tfReq1_2Id);
            Assert.IsFalse(req2.IsVoided);
        }
    }
}
