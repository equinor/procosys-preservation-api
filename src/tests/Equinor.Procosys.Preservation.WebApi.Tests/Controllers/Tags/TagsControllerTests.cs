using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
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
        private readonly CreateTagDto _createTagDto = new CreateTagDto();
        private TagsController _dut;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>));
            _dut = new TagsController(_mediatorMock.Object);
        }

        [TestMethod]
        public async Task CreateTag_ShouldSendCommand()
        {
            await _dut.CreateTags("", _createTagDto);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTag_ShouldCreateCorrectCommand()
        {
            CreateTagsCommand CreateTagsCommandCreated = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>))
                .Callback<IRequest<Result<List<int>>>, CancellationToken>((request, cancellationToken) =>
                {
                    CreateTagsCommandCreated = request as CreateTagsCommand;
                });

            _createTagDto.ProjectName = "ProjectName";
            _createTagDto.StepId = 2;
            _createTagDto.TagNos = new List<string> {"TagNo1", "TagNo2"};

            await _dut.CreateTags("", _createTagDto);

            Assert.AreEqual(_createTagDto.ProjectName, CreateTagsCommandCreated.ProjectName);
            Assert.AreEqual(_createTagDto.StepId, CreateTagsCommandCreated.StepId);
            Assert.AreEqual(2, CreateTagsCommandCreated.TagNos.Count());
            Assert.AreEqual(_createTagDto.TagNos.First(), CreateTagsCommandCreated.TagNos.First());
            Assert.AreEqual(_createTagDto.TagNos.Last(), CreateTagsCommandCreated.TagNos.Last());
        }

        [TestMethod]
        public async Task CreateTag_ShouldReturnsOk_WhenResultIsSuccessful()
        {
            var result = await _dut.CreateTags("", _createTagDto);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<int>));
            var list = (List<int>)okResult.Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(5, list.First());
        }
    }
}
