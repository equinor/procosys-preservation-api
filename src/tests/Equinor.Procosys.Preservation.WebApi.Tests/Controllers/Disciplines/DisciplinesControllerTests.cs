using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.GetDisciplines;
using Equinor.Procosys.Preservation.WebApi.Controllers.Disciplines;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Disciplines
{
    [TestClass]
    public class DisciplinesControllerTests
    {
        Mock<IMediator> _mediatorMock;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetDisciplinesQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<DisciplineDto>>(new List<DisciplineDto>
                {
                    new DisciplineDto("CodeA", "DescriptionA"),
                    new DisciplineDto("CodeB", "DescriptionB"),
                    new DisciplineDto("CodeC", "DescriptionC"),
                }) as Result<List<DisciplineDto>>));
        }

        [TestMethod]
        public async Task GetDisciplines_ReturnsCorrectNumberOfElements()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<GetDisciplinesQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<DisciplineDto>>(new List<DisciplineDto>
                {
                    new DisciplineDto("CodeA", "DescriptionA"),
                    new DisciplineDto("CodeB", "DescriptionB"),
                    new DisciplineDto("CodeC", "DescriptionC"),
                }) as Result<List<DisciplineDto>>));

            var dut = new DisciplinesController(_mediatorMock.Object);

            var result = await dut.GetDisciplines();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<DisciplineDto>));
            var list = (List<DisciplineDto>)okResult.Value;
            Assert.AreEqual(3, list.Count);
        }
    }
}
