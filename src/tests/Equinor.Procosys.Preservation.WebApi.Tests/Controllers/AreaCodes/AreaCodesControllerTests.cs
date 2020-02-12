using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetAreaCodes;
using Equinor.Procosys.Preservation.WebApi.Controllers.AreaCodes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.AreaCodes
{
    [TestClass]
    public class AreaCodesControllerTests
    {
        Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [TestMethod]
        public async Task GetDisciplines_ReturnsCorrectNumberOfElements()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetAreaCodesQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<AreaCodeDto>>(new List<AreaCodeDto>
                {
                    new AreaCodeDto("CodeA", "DescriptionA"),
                    new AreaCodeDto("CodeB", "DescriptionB"),
                    new AreaCodeDto("CodeC", "DescriptionC"),
                }) as Result<List<AreaCodeDto>>));

            var dut = new AreaCodesController(_mediatorMock.Object);

            var result = await dut.GetAreaCodes();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<AreaCodeDto>));
            var list = (List<AreaCodeDto>)okResult.Value;
            Assert.AreEqual(3, list.Count);
        }
    }
}
