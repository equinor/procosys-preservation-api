using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetAreas;
using Equinor.Procosys.Preservation.WebApi.Controllers.Areas;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Areas
{
    [TestClass]
    public class AreasControllerTests
    {
        Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Setup() => _mediatorMock = new Mock<IMediator>();

        [TestMethod]
        public async Task GetDisciplines_ReturnsCorrectNumberOfElements()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAreasQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<AreaDto>>(new List<AreaDto>
                {
                    new AreaDto("CodeA", "DescriptionA"),
                    new AreaDto("CodeB", "DescriptionB"),
                    new AreaDto("CodeC", "DescriptionC"),
                }) as Result<List<AreaDto>>));

            var dut = new AreasController(_mediatorMock.Object);

            var result = await dut.GetAreas("");

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<AreaDto>));
            var list = (List<AreaDto>)okResult.Value;
            Assert.AreEqual(3, list.Count);
        }
    }
}
