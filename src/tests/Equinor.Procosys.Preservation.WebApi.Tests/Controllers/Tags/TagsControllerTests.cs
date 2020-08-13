using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Equinor.Procosys.Preservation.WebApi.Excel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class TagsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private Mock<IExcelConverter> _excelConverterMock;
        private readonly CreateTagsDto _createTagsDto = new CreateTagsDto();
        private TagsController _dut;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>));
            _excelConverterMock = new Mock<IExcelConverter>();
            _dut = new TagsController(_mediatorMock.Object, _excelConverterMock.Object);
        }

        [TestMethod]
        public async Task CreateTags_ShouldSendCommand()
        {
            await _dut.CreateTags("", _createTagsDto);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTags_ShouldCreateCorrectCommand()
        {
            CreateTagsCommand CreateTagsCommandCreated = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>))
                .Callback<IRequest<Result<List<int>>>, CancellationToken>((request, cancellationToken) =>
                {
                    CreateTagsCommandCreated = request as CreateTagsCommand;
                });

            _createTagsDto.ProjectName = "ProjectName";
            _createTagsDto.StepId = 2;
            _createTagsDto.TagNos = new List<string> {"TagNo1", "TagNo2"};

            await _dut.CreateTags("", _createTagsDto);

            Assert.AreEqual(_createTagsDto.ProjectName, CreateTagsCommandCreated.ProjectName);
            Assert.AreEqual(_createTagsDto.StepId, CreateTagsCommandCreated.StepId);
            Assert.AreEqual(2, CreateTagsCommandCreated.TagNos.Count());
            Assert.AreEqual(_createTagsDto.TagNos.First(), CreateTagsCommandCreated.TagNos.First());
            Assert.AreEqual(_createTagsDto.TagNos.Last(), CreateTagsCommandCreated.TagNos.Last());
        }

        [TestMethod]
        public async Task CreateTags_ShouldReturnsOk_WhenResultIsSuccessful()
        {
            var result = await _dut.CreateTags("", _createTagsDto);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<int>));
            var list = (List<int>)okResult.Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(5, list.First());
        }
    }
}
