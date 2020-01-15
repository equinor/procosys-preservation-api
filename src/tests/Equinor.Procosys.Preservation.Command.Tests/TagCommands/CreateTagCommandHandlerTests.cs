using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Requirement = Equinor.Procosys.Preservation.Command.TagCommands.CreateTag.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands
{
    [TestClass]
    public class CreateTagCommandHandlerTests
    {
        [TestMethod]
        public async Task HandlingCreateTagCommand_ShouldAddTagToRepository()
        {
            // Arrange
            var mode = new Mock<Mode>("TestPlant", "ModeTitle");
            var responsible = new Mock<Responsible>("TestPlant", "ResponsibleName");
            var stepId = 11;
            var step = new Mock<Step>("TestPlant", mode.Object, responsible.Object);
            step.SetupGet(x => x.Id).Returns(stepId);
            var journeyRepository = new Mock<IJourneyRepository>();
            journeyRepository
                .Setup(x => x.GetStepByStepIdAsync(stepId))
                .Returns(Task.FromResult(step.Object));

            Tag tagAddedToRepository = null;
            var tagRepository = new Mock<ITagRepository>();
            tagRepository
                .Setup(x => x.Add(It.IsAny<Tag>()))
                .Callback<Tag>(x =>
                {
                    tagAddedToRepository = x;
                });

            var requirementTypeRepository = new Mock<IRequirementTypeRepository>();
            var requirementDefinition = new Mock<RequirementDefinition>("TestPlant", "Title", 4, 1);
            var requirementDefinitionId = 99;
            requirementDefinition.SetupGet(x => x.Id).Returns(requirementDefinitionId);
            requirementTypeRepository
                .Setup(r => r.GetRequirementDefinitionByIdAsync(requirementDefinitionId))
                .Returns(Task.FromResult(requirementDefinition.Object));

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

            var command = new CreateTagCommand(
                "TagNo",
                "ProjectNumber",
                stepId,
                new List<Requirement>
                {
                    new Requirement(requirementDefinitionId, 1)
                });
            var createTagCommandHandler = new CreateTagCommandHandler(
                tagRepository.Object,
                journeyRepository.Object,
                requirementTypeRepository.Object,
                unitOfWork.Object,
                plantProvider.Object,
                tagApiService.Object);

            // Act
            var result = await createTagCommandHandler.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual("AreaCode", tagAddedToRepository.AreaCode);
            Assert.AreEqual("CalloffNo", tagAddedToRepository.CalloffNumber);
            Assert.AreEqual("CommPkgNo", tagAddedToRepository.CommPkgNumber);
            Assert.AreEqual("DisciplineCode", tagAddedToRepository.DisciplineCode);
            Assert.AreEqual(0, tagAddedToRepository.Id);
            Assert.AreEqual(false, tagAddedToRepository.IsAreaTag);
            Assert.AreEqual("McPkgNo", tagAddedToRepository.McPkcNumber);
            Assert.AreEqual("ProjectNumber", tagAddedToRepository.ProjectNumber);
            Assert.AreEqual("PurchaseOrderNo", tagAddedToRepository.PurchaseOrderNumber);
            Assert.AreEqual("TestPlant", tagAddedToRepository.Schema);
            Assert.AreEqual(stepId, tagAddedToRepository.StepId);
            Assert.AreEqual("TagFunctionCode", tagAddedToRepository.TagFunctionCode);
            Assert.AreEqual("TagNo", tagAddedToRepository.TagNo);
            Assert.AreEqual(1, tagAddedToRepository.Requirements.Count);
            Assert.AreEqual(requirementDefinitionId, tagAddedToRepository.Requirements.First().RequirementDefinitionId);
        }
    }
}
