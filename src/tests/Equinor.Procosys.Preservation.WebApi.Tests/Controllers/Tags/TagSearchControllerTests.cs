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
    public class TagSearchControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private TagSearchController _dut;
        private readonly List<ProcosysTagDto> _listWithTwoItems = new List<ProcosysTagDto>
        {
            new ProcosysTagDto("TagNo1", "Desc1", "PO1", "CommPkg1", "McPkg1", "TagFuncCode1", "RegCode1", "R1", true),
            new ProcosysTagDto("TagNo2", "Desc2", "PO2", "CommPkg2", "McPkg2", "TagFuncCode2", "RegCode2", "R2", false)
        };

        [TestInitialize]
        public void Setup() => _dut = new TagSearchController(_mediatorMock.Object);

        [TestMethod]
        public async Task SearchTags_ShouldSendCommand()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(null) as Result<List<ProcosysTagDto>>));

            await _dut.SearchTagsByTagNo("", "", "");
            _mediatorMock.Verify(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task SearchTags_ShouldCreateCorrectCommand()
        {
            SearchTagsByTagNoQuery query = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(null) as Result<List<ProcosysTagDto>>))
                .Callback<IRequest<Result<List<ProcosysTagDto>>>, CancellationToken>((request, cancellationToken) =>
                {
                    query = request as SearchTagsByTagNoQuery;
                });

            await _dut.SearchTagsByTagNo("", "ProjectName", "TagNo");

            Assert.AreEqual("ProjectName", query.ProjectName);
            Assert.AreEqual("TagNo", query.StartsWithTagNo);
        }

        [TestMethod]
        public async Task SearchTags_ShouldReturnOk_WhenResultIsSuccessful()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(_listWithTwoItems) as Result<List<ProcosysTagDto>>));

            var result = await _dut.SearchTagsByTagNo("", "ProjectName", "TagNo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<ProcosysTagDto>>));

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task SearchTags_ShouldReturnCorrectNumberOfElements()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<ProcosysTagDto>>(_listWithTwoItems) as Result<List<ProcosysTagDto>>));

            var result = await _dut.SearchTagsByTagNo("", "ProjectName", "TagNo");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsNotNull(((OkObjectResult)result.Result).Value);
            Assert.IsInstanceOfType(((OkObjectResult)result.Result).Value, typeof(List<ProcosysTagDto>));
            Assert.AreEqual(2, (((OkObjectResult)result.Result).Value as List<ProcosysTagDto>).Count);
        }

        [TestMethod]
        public async Task SearchTags_ShouldReturnNotFound_IfResultIsNotFound()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<SearchTagsByTagNoQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new NotFoundResult<List<ProcosysTagDto>>(string.Empty) as Result<List<ProcosysTagDto>>));

            var result = await _dut.SearchTagsByTagNo("", "ProjectName", "TagNo");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }
    }
}
