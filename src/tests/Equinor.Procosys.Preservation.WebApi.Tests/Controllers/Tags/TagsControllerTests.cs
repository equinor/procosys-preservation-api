using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
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
        private Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private CreateTagDto _createTagDto = new CreateTagDto();
        private TagsController _dut;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>));
            _dut = new TagsController(_mediatorMock.Object);
        }

        [TestMethod]
        public async Task CreateTag_ShouldSendCommand()
        {
            await _dut.CreateTag(_createTagDto);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTag_ShouldCreateCorrectCommand()
        {
            CreateTagCommand createTagCommandCreated = null;
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<List<int>>(new List<int>{5}) as Result<List<int>>))
                .Callback<IRequest<Result<List<int>>>, CancellationToken>((request, cancellationToken) =>
                {
                    createTagCommandCreated = request as CreateTagCommand;
                });

            _createTagDto.ProjectName = "ProjectName";
            _createTagDto.StepId = 2;
            _createTagDto.TagNos = new List<string> {"TagNo1", "TagNo2"};

            await _dut.CreateTag(_createTagDto);

            Assert.AreEqual(_createTagDto.ProjectName, createTagCommandCreated.ProjectName);
            Assert.AreEqual(_createTagDto.StepId, createTagCommandCreated.StepId);
            Assert.AreEqual(2, createTagCommandCreated.TagNos.Count());
            Assert.AreEqual(_createTagDto.TagNos.First(), createTagCommandCreated.TagNos.First());
            Assert.AreEqual(_createTagDto.TagNos.Last(), createTagCommandCreated.TagNos.Last());
        }

        [TestMethod]
        public async Task CreateTag_ShouldReturnsOk_WhenResultIsSuccessful()
        {
            var result = await _dut.CreateTag(_createTagDto);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<int>));
            var list = (List<int>)okResult.Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(5, list.First());
        }
    }
}
