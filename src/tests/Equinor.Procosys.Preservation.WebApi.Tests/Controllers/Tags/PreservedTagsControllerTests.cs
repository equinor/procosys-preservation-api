using System.Collections.Generic;
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
    public class PreservedTagsControllerTests
    {
        private Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private CreateTagDto _createTagDto = new CreateTagDto {Requirements = new List<TagRequirementDto>()};
        private PreservedTagsController _dut;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<int>(5) as Result<int>));
            _dut = new PreservedTagsController(_mediatorMock.Object);
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
                .Returns(Task.FromResult(new SuccessResult<int>(5) as Result<int>))
                .Callback<IRequest<Result<int>>, CancellationToken>((request, cancellationToken) =>
                {
                    createTagCommandCreated = request as CreateTagCommand;
                });

            _createTagDto.ProjectName = "ProjectName";
            _createTagDto.StepId = 2;
            _createTagDto.TagNo = "TagNo";

            await _dut.CreateTag(_createTagDto);

            Assert.AreEqual(_createTagDto.ProjectName, createTagCommandCreated.ProjectName);
            Assert.AreEqual(_createTagDto.StepId, createTagCommandCreated.StepId);
            Assert.AreEqual(_createTagDto.TagNo, createTagCommandCreated.TagNo);
        }

        [TestMethod]
        public async Task CreateTag_ShouldReturnsOk_WhenResultIsSuccessful()
        {
            var result = await _dut.CreateTag(_createTagDto);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(5, ((OkObjectResult)result.Result).Value);
        }

        [TestMethod]
        public async Task CreateTag_ShouldReturnsNotFound_IfResultIsNotFound()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new NotFoundResult<int>("") as Result<int>));

            var result = await _dut.CreateTag(_createTagDto);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }
    }
}
