using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands
{
    [TestClass]
    public class CreateTagCommandHandlerTests
    {
        [TestMethod]
        public async Task TagIsAddedToRepositoryTestAsync()
        {
            // Arrange
            var mode = new Mock<Mode>("TestPlant", "ModeTitle");
            var responsible = new Mock<Responsible>("TestPlant", "ResponsibleName");
            var step = new Mock<Step>("TestPlant", mode.Object, responsible.Object);
            var journey = new Mock<Journey>("TestPlant", "JourneyTitle");
            journey.Object.AddStep(step.Object);
            var journeyRepository = new Mock<IJourneyRepository>();
            journeyRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(journey.Object));

            Tag tagAddedToRepository = null;
            var tagRepository = new Mock<ITagRepository>();
            tagRepository
                .Setup(x => x.Add(It.IsAny<Tag>()))
                .Callback<Tag>(x =>
                {
                    tagAddedToRepository = x;
                });

            var unitOfWork = new Mock<IUnitOfWork>();

            var plantProvider = new Mock<IPlantProvider>();
            plantProvider
                .Setup(x => x.Plant)
                .Returns("TestPlant");

            var tagDetails = new ProcosysTagDetails();
            tagDetails.AreaCode = "AreaCode";
            tagDetails.CallOffNo = "CalloffNo";
            tagDetails.CommPkgNo = "CommPkgNo";
            tagDetails.Description = "Description";
            tagDetails.DisciplineCode = "DisciplineCode";
            tagDetails.McPkgNo = "McPkgNo";
            tagDetails.PurchaseOrderNo = "PurchaseOrderNo";
            tagDetails.TagFunctionCode = "TagFunctionCode";
            tagDetails.TagNo = "TagNo";
            var tagApiService = new Mock<ITagApiService>();
            tagApiService
                .Setup(x => x.GetTagDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(tagDetails));

            var command = new CreateTagCommand("TagNumber", "ProjectNumber", 0, 0, "Description");
            var dut = new CreateTagCommandHandler(tagRepository.Object, journeyRepository.Object, unitOfWork.Object, plantProvider.Object, tagApiService.Object);

            // Act
            var result = await dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual("AreaCode", tagAddedToRepository.AreaCode);
            Assert.AreEqual("CalloffNo", tagAddedToRepository.CalloffNumber);
            Assert.AreEqual("CommPkgNo", tagAddedToRepository.CommPkgNumber);
            Assert.AreEqual("Description", tagAddedToRepository.Description);
            Assert.AreEqual("DisciplineCode", tagAddedToRepository.DisciplineCode);
            Assert.AreEqual(0, tagAddedToRepository.Id);
            Assert.AreEqual(false, tagAddedToRepository.IsAreaTag);
            Assert.AreEqual("McPkgNo", tagAddedToRepository.McPkcNumber);
            Assert.AreEqual("ProjectNumber", tagAddedToRepository.ProjectNumber);
            Assert.AreEqual("PurchaseOrderNo", tagAddedToRepository.PurchaseOrderNumber);
            Assert.AreEqual("TestPlant", tagAddedToRepository.Schema);
            Assert.AreEqual(0, tagAddedToRepository.StepId);
            Assert.AreEqual("TagFunctionCode", tagAddedToRepository.TagFunctionCode);
            Assert.AreEqual("TagNumber", tagAddedToRepository.TagNumber);
        }
    }
}
