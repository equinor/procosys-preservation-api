using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.Query.GetDisciplines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetDisciplines
{
    [TestClass]
    public class GetDisciplinesQueryHandlerTests
    {
        private List<ProcosysDiscipline> _resultWithThreeElements;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IDisciplineApiService> _areaCodeApiServiceMock;
        private GetDisciplinesQuery _query;

        [TestInitialize]
        public void Setup()
        {
            _resultWithThreeElements = new List<ProcosysDiscipline>
            {
                new ProcosysDiscipline
                {
                    Code = "CodeA",
                    Description = "DescriptionA",
                    Id = 1,
                },
                new ProcosysDiscipline
                {
                    Code = "CodeB",
                    Description = "DescriptionB",
                    Id = 1,
                },
                new ProcosysDiscipline
                {
                    Code = "CodeC",
                    Description = "DescriptionC",
                    Id = 3,
                },
            };

            _plantProviderMock = new Mock<IPlantProvider>();
            _areaCodeApiServiceMock = new Mock<IDisciplineApiService>();
            _query = new GetDisciplinesQuery();
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectResultType()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetDisciplinesAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetDisciplinesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfElements()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetDisciplinesAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetDisciplinesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

            var result = await dut.Handle(_query, default);

            Assert.AreEqual(3, result.Data.Count);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectData()
        {
            _areaCodeApiServiceMock
                .Setup(x => x.GetDisciplinesAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_resultWithThreeElements));
            var dut = new GetDisciplinesQueryHandler(_areaCodeApiServiceMock.Object, _plantProviderMock.Object);

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
