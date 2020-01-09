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
        [TestMethod]
        public async Task Create_tag_sends_command_test_async()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<int>(5) as Result<int>));
            var dut = new TagsController(mediator.Object);
            var dto = new Mock<CreateTagDto>();

            await dut.CreateTag(dto.Object);

            mediator.Verify(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_tag_creates_correct_command_test_async()
        {
            var mediator = new Mock<IMediator>();
            CreateTagCommand createTagCommandCreated = null;
            mediator
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<int>(5) as Result<int>))
                .Callback<IRequest<Result<int>>, CancellationToken>((request, cancellationToken) =>
                {
                    createTagCommandCreated = request as CreateTagCommand;
                });
            var dut = new TagsController(mediator.Object);
            var dto = new CreateTagDto
            {
                Description = "Description",
                JourneyId = 1,
                ProjectNo = "ProjectNumber",
                StepId = 2,
                TagNo = "TagNumber"
            };

            await dut.CreateTag(dto);

            Assert.AreEqual(dto.Description, createTagCommandCreated.Description);
            Assert.AreEqual(dto.JourneyId, createTagCommandCreated.JourneyId);
            Assert.AreEqual(dto.ProjectNo, createTagCommandCreated.ProjectNumber);
            Assert.AreEqual(dto.StepId, createTagCommandCreated.StepId);
            Assert.AreEqual(dto.TagNo, createTagCommandCreated.TagNumber);
        }

        [TestMethod]
        public async Task Create_tag_returns_Ok_test_async()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<int>(5) as Result<int>));
            var dut = new TagsController(mediator.Object);
            var dto = new Mock<CreateTagDto>();

            var result = await dut.CreateTag(dto.Object);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(5, ((OkObjectResult)result).Value);
        }

        [TestMethod]
        public async Task Create_tag_returns_NotFound_test_async()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(It.IsAny<CreateTagCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new NotFoundResult<int>("") as Result<int>));
            var dut = new TagsController(mediator.Object);
            var dto = new Mock<CreateTagDto>();

            var result = await dut.CreateTag(dto.Object);
        }
    }
}
