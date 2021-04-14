using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.UpdateRequirements
{
    [TestClass]
    public class UpdateRequirementsCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string TagFunctionCode = "TFC";
        private readonly string RegisterCode = "RC";
        private readonly string PCSDescription = "PCSDescription";
        private RequirementDefinition _reqDef1;
        private RequirementDefinition _reqDef2;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int Interval1 = 2;
        private const int Interval2 = 3;
        private const int Interval3 = 4;

        private TagFunction _tfAddedToRepository;
        private Mock<ITagFunctionRepository> _tfRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<ITagFunctionApiService> _tagFunctionApiServiceMock;
        
        private UpdateRequirementsCommand _commandWithTwoRequirements;
        private UpdateRequirementsCommand _commandWithoutRequirements;
        private UpdateRequirementsCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(x => x.Id).Returns(ReqDefId1);
            rdMock1.SetupGet(x => x.Plant).Returns(TestPlant);
            _reqDef1 = rdMock1.Object;
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(x => x.Id).Returns(ReqDefId2);
            rdMock2.SetupGet(x => x.Plant).Returns(TestPlant);
            _reqDef2 = rdMock2.Object;
            
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionsByIdsAsync(new List<int> {ReqDefId1, ReqDefId2}))
                .Returns(Task.FromResult(new List<RequirementDefinition> {rdMock1.Object, rdMock2.Object}));

            _tagFunctionApiServiceMock = new Mock<ITagFunctionApiService>();
            _tagFunctionApiServiceMock.Setup(t => t.TryGetTagFunctionAsync(TestPlant, TagFunctionCode, RegisterCode))
                .Returns(Task.FromResult(new PCSTagFunction
                {
                    Code = TagFunctionCode,
                    Description = PCSDescription,
                    RegisterCode = RegisterCode
                }));
            _commandWithTwoRequirements = new UpdateRequirementsCommand(TagFunctionCode, RegisterCode,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId1, Interval1),
                    new RequirementForCommand(ReqDefId2, Interval2),
                });
            _commandWithoutRequirements = new UpdateRequirementsCommand(TagFunctionCode, RegisterCode, null);
            
            _tfRepositoryMock = new Mock<ITagFunctionRepository>();
            _tfRepositoryMock
                .Setup(x => x.Add(It.IsAny<TagFunction>()))
                .Callback<TagFunction>(tf =>
                {
                    _tfAddedToRepository = tf;
                });

            _dut = new UpdateRequirementsCommandHandler(
                _tfRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _tagFunctionApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldAddTagFunctionWithRequirementsToRepository_WhenTagFunctionNotExists()
        {
            // Arrange
            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult((TagFunction)null));

            // Act
            await _dut.Handle(_commandWithTwoRequirements, default);

            // Assert
            Assert.AreEqual(TagFunctionCode, _tfAddedToRepository.Code);
            Assert.AreEqual(PCSDescription, _tfAddedToRepository.Description);
            Assert.AreEqual(RegisterCode, _tfAddedToRepository.RegisterCode);
            Assert.AreEqual(2, _tfAddedToRepository.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldAddTagFunctionWithoutRequirementsToRepository_WhenTagFunctionNotExists()
        {
            // Arrange
            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult((TagFunction)null));

            // Act
            await _dut.Handle(_commandWithoutRequirements, default);

            // Assert
            Assert.AreEqual(TagFunctionCode, _tfAddedToRepository.Code);
            Assert.AreEqual(RegisterCode, _tfAddedToRepository.RegisterCode);
            Assert.AreEqual(0, _tfAddedToRepository.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldNotAddAnyTagFunctionToRepository_WhenTagFunctionAlreadyExists()
        {
            // Arrange
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode, "", RegisterCode);
            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult(tagFunction));

            // Act
            var result = await _dut.Handle(_commandWithTwoRequirements, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(_tfAddedToRepository);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldAddNewRequirementsToExistingTagFunction()
        {
            // Arrange
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode, "", RegisterCode);
            Assert.AreEqual(0, tagFunction.Requirements.Count);
            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult(tagFunction));

            // Act
            await _dut.Handle(_commandWithTwoRequirements, default);

            // Assert
            Assert.AreEqual(2, tagFunction.Requirements.Count);
            AssertRequirement(tagFunction.Requirements, ReqDefId1, Interval1);
            AssertRequirement(tagFunction.Requirements, ReqDefId2, Interval2);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldUpdateRequirementsInExistingTagFunction_WhenNewIntervals()
        {
            // Arrange
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode, "", RegisterCode);
            tagFunction.AddRequirement(new TagFunctionRequirement(TestPlant, Interval3, _reqDef1));
            tagFunction.AddRequirement(new TagFunctionRequirement(TestPlant, Interval3, _reqDef2));
            Assert.AreEqual(2, tagFunction.Requirements.Count);
            AssertRequirement(tagFunction.Requirements, ReqDefId1, Interval3);
            AssertRequirement(tagFunction.Requirements, ReqDefId2, Interval3);

            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult(tagFunction));

            // Act
            await _dut.Handle(_commandWithTwoRequirements, default);

            // Assert
            Assert.AreEqual(2, tagFunction.Requirements.Count);
            AssertRequirement(tagFunction.Requirements, ReqDefId1, Interval1);
            AssertRequirement(tagFunction.Requirements, ReqDefId2, Interval2);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldRemoveRequirementsFromExistingTagFunction()
        {
            // Arrange
            var tagFunction = new TagFunction(TestPlant, TagFunctionCode, "", RegisterCode);
            tagFunction.AddRequirement(new TagFunctionRequirement(TestPlant, Interval3, _reqDef1));
            tagFunction.AddRequirement(new TagFunctionRequirement(TestPlant, Interval3, _reqDef2));
            Assert.AreEqual(2, tagFunction.Requirements.Count);
            AssertRequirement(tagFunction.Requirements, ReqDefId1, Interval3);
            AssertRequirement(tagFunction.Requirements, ReqDefId2, Interval3);

            _tfRepositoryMock
                .Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode)).Returns(Task.FromResult(tagFunction));

            // Act
            await _dut.Handle(_commandWithoutRequirements, default);

            // Assert
            Assert.AreEqual(0, tagFunction.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_commandWithTwoRequirements, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertRequirement(IReadOnlyCollection<TagFunctionRequirement> requirements, int reqDefId, int interval)
        {
            var req = requirements.SingleOrDefault(r => r.RequirementDefinitionId == reqDefId);
            Assert.IsNotNull(req);
            Assert.AreEqual(interval, req.IntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementsCommand_ShouldNotSetRowVersion_WhenTagFunctionAdded()
        {
            // Act
            var result = await _dut.Handle(_commandWithoutRequirements, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            var defaultRowVersion = "AAAAAAAAAAA=";
            Assert.AreEqual(defaultRowVersion, result.Data);
            Assert.AreEqual(defaultRowVersion, _tfAddedToRepository.RowVersion.ConvertToString());
        }
    }
}
