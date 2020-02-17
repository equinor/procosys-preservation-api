using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.Query.GetAreas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAreaCodes
{
    [TestClass]
    public class GetAreaCodesQueryHandlerTests
    {
        private List<ProcosysArea> _resultWithThreeElements;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IAreaApiService> _areaApiServiceMock;
        private GetAreasQuery _query;

        [TestInitialize]
        public void Setup()
        {
            _resultWithThreeElements = new List<ProcosysArea>
            {
                new ProcosysArea
                {
                    Code = "CodeA",
                    Description = "DescriptionA",
                    Id = 1,
                },
                new ProcosysArea
                {
                    Code = "CodeB",
                    Description = "DescriptionB",
                    Id = 1,
                },
                new ProcosysArea
                {
                    Code = "CodeC",
                    Description = "DescriptionC",
                    Id = 3,
                },
            };

            _plantProviderMock = new Mock<IPlantProvider>();
            _areaApiServiceMock = new Mock<IAreaApiService>();
            _query = new GetAreasQuery();
        }

        [TestMethod]
        public async Task Handle_ShouldReturnCorrectResultType()
        {
            _areaApiServiceMock
                .Setup(x => x.GetAreas(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreasQueryHandler(_areaApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnCorrectNumberOfElements()
        {
            _areaApiServiceMock
                .Setup(x => x.GetAreas(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreasQueryHandler(_areaApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(3, result.Data.Count);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnCorrectData()
        {
            _areaApiServiceMock
                .Setup(x => x.GetAreas(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetAreasQueryHandler(_areaApiServiceMock.Object, _plantProviderMock.Object);

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
