using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class AvailableTagsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private AvailableTagsController _dut;
        private readonly List<ProcosysTagDto> _listWithTwoItems = new List<ProcosysTagDto>
        {
            new ProcosysTagDto("TagNo1", "Desc1", "PO1", "CommPkg1", "McPkg1", true),
            new ProcosysTagDto("TagNo2", "Desc2", "PO2", "CommPkg2", "McPkg2", false)
        };

        [TestInitialize]
        public void Setup() => _dut = new AvailableTagsController(_mediatorMock.Object);

        [TestMethod]
        public async Task GetAllAvailableTags_ShouldSendCommand()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(null) as Result<List<ProcosysTagDto>>));

            await _dut.GetAllAvailableTags("", "");
            _mediatorMock.Verify(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAllAvailableTags_ShouldCreateCorrectCommand()
        {
            SearchTagsQuery query = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(null) as Result<List<ProcosysTagDto>>))
                .Callback<IRequest<Result<List<ProcosysTagDto>>>, CancellationToken>((request, cancellationToken) =>
                {
                    query = request as SearchTagsQuery;
                });

            await _dut.GetAllAvailableTags("ProjectName", "TagNo");

            Assert.AreEqual("ProjectName", query.ProjectName);
            Assert.AreEqual("TagNo", query.StartsWithTagNo);
        }

        [TestMethod]
        public async Task GetAllAvailableTags_ShouldReturnOk_WhenResultIsSuccessful()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(_listWithTwoItems) as Result<List<ProcosysTagDto>>));

            var result = await _dut.GetAllAvailableTags("ProjectName", "TagNo");

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            Assert.AreEqual(2, (((OkObjectResult)result.Result).Value as List<ProcosysTagDto>).Count);
        }

        [TestMethod]
        public async Task GetAllAvailableTags_ReturnsCorrectNumberOfElements()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(_listWithTwoItems) as Result<List<ProcosysTagDto>>));

            var result = await _dut.GetAllAvailableTags("ProjectName", "TagNo");
            
            Assert.AreEqual(2, (((OkObjectResult)result.Result).Value as List<ProcosysTagDto>).Count);
        }

        [TestMethod]
        public async Task GetAllAvailableTags_ShouldReturnsNotFound_IfResultIsNotFound()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new NotFoundResult<List<ProcosysTagDto>>(string.Empty) as Result<List<ProcosysTagDto>>));

            var result = await _dut.GetAllAvailableTags("ProjectName", "TagNo");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }
    }
}
