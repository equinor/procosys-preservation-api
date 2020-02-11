using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.AreaCode;
using Equinor.Procosys.Preservation.Query.GetAreaCodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAreaCodes
{
    [TestClass]
    public class GetAreaCodesQueryHandlerTests
    {
        private List<ProcosysAreaCode> _resultWithThreeElements;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IAreaCodeApiService> _areaCodeApiServiceMock;
        private GetAreaCodesQuery _query;

        [TestInitialize]
        public void Setup()
        {
            _resultWithThreeElements = new List<ProcosysAreaCode>
            {
                new ProcosysAreaCode
                {
                    Code = "CodeA",
                    Description = "DescriptionA",
                    Id = 1,
                },
                new ProcosysAreaCode
                {
                    Code = "CodeB",
                    Description = "DescriptionB",
                    Id = 1,
                },
                new ProcosysAreaCode
                {
                    Code = "CodeC",
                    Description = "DescriptionC",
                    Id = 3,
                },
            };

            _plantProviderMock = new Mock<IPlantProvider>();
            _areaCodeApiServiceMock = new Mock<IAreaCodeApiService>();
            _query = new GetAreaCodesQuery();
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrecResultType()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetAreaCodes(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreaCodesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetAreaCodes(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreaCodesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(3, result.Data.Count);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectData()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetAreaCodes(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreaCodesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual("CodeA", result.Data[0].Code);
            Assert.AreEqual("DescriptionA", result.Data[0].Description);
            Assert.AreEqual("CodeB", result.Data[1].Code);
            Assert.AreEqual("DescriptionB", result.Data[1].Description);
            Assert.AreEqual("CodeC", result.Data[2].Code);
            Assert.AreEqual("DescriptionC", result.Data[2].Description);
        }
    }
}
