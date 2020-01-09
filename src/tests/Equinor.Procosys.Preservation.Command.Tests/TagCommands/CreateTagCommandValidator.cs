using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands
{
    [TestClass]
    public class CreateTagCommandValidatorTests
    {
        [TestMethod]
        public void Should_have_error_when_journey_does_not_exist_test()
        {
            var journeyRepository = new Mock<IJourneyRepository>();
            var command = new Mock<CreateTagCommand>("TagNumber", "ProjectNumber", 1, 2, "Description");
            var dut = new CreateTagCommandValidator(journeyRepository.Object);
            dut.ShouldHaveValidationErrorFor(command => command.JourneyId, command.Object);
        }

        [TestMethod]
        public void Should_not_have_error_when_journey_exists_test()
        {
            var journeyRepository = new Mock<IJourneyRepository>();
            journeyRepository.Setup(repo => repo.GetByIdAsync(1)).Returns(Task.FromResult(new Mock<Journey>("TestPlant", "JourneyTitle").Object));
            var command = new Mock<CreateTagCommand>("TagNumber", "ProjectNumber", 1, 2, "Description");
            var dut = new CreateTagCommandValidator(journeyRepository.Object);
            dut.ShouldNotHaveValidationErrorFor(command => command.JourneyId, command.Object);
        }

        [TestMethod]
        public void Should_have_error_when_step_does_not_exist_test()
        {
            var journeyRepository = new Mock<IJourneyRepository>();
            var command = new Mock<CreateTagCommand>("TagNumber", "ProjectNumber", 1, 2, "Description");
            var dut = new CreateTagCommandValidator(journeyRepository.Object);
            dut.ShouldHaveValidationErrorFor(command => command.StepId, command.Object);
        }

        [TestMethod]
        public void Should_not_have_error_when_step_exists_test()
        {
            var step = new Mock<Step>();
            step
                .SetupGet(step => step.Id)
                .Returns(2);
            var journey = new Mock<Journey>("TestPlant", "JourneyTitle");
            journey
                .SetupGet(x => x.Id)
                .Returns(1);
            journey.Object.AddStep(step.Object);
            var journeyRepository = new Mock<IJourneyRepository>();
            journeyRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .Returns(Task.FromResult(journey.Object));
            var command = new Mock<CreateTagCommand>("TagNumber", "ProjectNumber", 1, 2, "Description");
            var dut = new CreateTagCommandValidator(journeyRepository.Object);

            dut.ShouldNotHaveValidationErrorFor(command => command.JourneyId, command.Object);
        }
    }
}
