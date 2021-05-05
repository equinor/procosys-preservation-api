using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Synchronization
{
    [TestClass]
    public class BusReceiverServiceTests
    {
        #region Variables and setup
        private BusReceiverService _dut;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IPlantSetter> _plantSetter;
        private Mock<ITelemetryClient> _telemetryClient;
        private Responsible _responsible;
        private Mock<IResponsibleRepository> _responsibleRepository;
        private Mock<IProjectRepository> _projectRepository;
        private Project _project1, _project2;
        private Mock<ITagFunctionRepository> _tagFunctionRepository;
        private TagFunction _tagFunction;

        private const string plant = "PCS$HEIMDAL";
        private const string code = "Resp_Code";
        private const string description = "789";
        private const string newDescription = "Odfjeld Drilling Instalation";
        private const string project1Name = "Project 1";
        private const string project2Name = "Project 2";
        private const string project1Description = "Description 1";
        private const string project2Description = "Description 2";
        private const string tagFunctionCode = "Code9";
        private const string registerCode = "ElRegisterCode";
        private const string tagFunctionDescription = "Tag function description";

        private Tag _tag1;
        private Tag _tag2;
        private Mock<Project> _project3;
        private const string TagNo1 = "TagNo1";
        private const string TagNo2 = "TagNo2";
        private const string OldTagDescription1 = "OldTagDescription1";
        private const string OldTagDescription2 = "OldTagDescription2";

        private const string CommPkg1 = "Comm1";
        private const string CommPkg2 = "Comm2";
        private const string McPkg1 = "MC1";
        private const string McPkg2 = "MC2";

        [TestInitialize]
        public void Setup()
        {
            _plantSetter = new Mock<IPlantSetter>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _telemetryClient = new Mock<ITelemetryClient>();
            _responsibleRepository = new Mock<IResponsibleRepository>();
            _projectRepository = new Mock<IProjectRepository>();
            _tagFunctionRepository = new Mock<ITagFunctionRepository>();

            // Assert tags in preservation
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(plant);

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(plant);
            _tag1 = new Tag(plant, TagType.Standard, TagNo1, OldTagDescription1, stepMock.Object, new List<TagRequirement>
            {
                new TagRequirement(plant, 4, rdMock.Object)
            });
            _tag1.McPkgNo = McPkg1;
            _tag1.CommPkgNo = CommPkg1;

            _tag2 = new Tag(plant, TagType.Standard, TagNo2, OldTagDescription2, stepMock.Object, new List<TagRequirement>
            {
                new TagRequirement(plant, 4, rdMock.Object)
            });
            _tag2.McPkgNo = McPkg2;
            _tag2.CommPkgNo = CommPkg2;

            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            _responsible = new Responsible(plant, code, description);
            _responsibleRepository.Setup(r => r.GetByCodeAsync(code)).Returns(Task.FromResult(_responsible));

            _project1 = new Project(plant, project1Name, project1Description);
            _project1.AddTag(_tag1);
            _project1.AddTag(_tag2);
            _projectRepository.Setup(p => p.GetStandardTagsInProjectOnlyAsync(project1Name))
                .Returns(Task.FromResult(tags));
            _projectRepository.Setup(p => p.GetProjectOnlyByNameAsync(project1Name))
                .Returns(Task.FromResult(_project1));
            _project2 = new Project(plant, project2Name, project2Description);
            _projectRepository.Setup(p => p.GetProjectOnlyByNameAsync(project2Name))
                .Returns(Task.FromResult(_project2));

            _tagFunction = new TagFunction(plant, tagFunctionCode, tagFunctionDescription, registerCode);
            _tagFunctionRepository.Setup(t => t.GetByCodesAsync(tagFunctionCode, registerCode))
                .Returns(Task.FromResult(_tagFunction));

            _dut = new BusReceiverService(_plantSetter.Object,
                                          _unitOfWork.Object,
                                          _telemetryClient.Object,
                                          _responsibleRepository.Object,
                                          _projectRepository.Object,
                                          _tagFunctionRepository.Object);
        }
        #endregion

        #region Project
        [TestMethod]
        public async Task HandlingProjectTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{\"Plant\" : \"{plant}\", \"ProjectName\" : \"{project1Name}\", \"IsClosed\" : true, \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Project, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(project1Name), Times.Once);
            Assert.AreEqual(newDescription, _project1.Description);
            Assert.AreEqual(true, _project1.IsClosed);
        }

        [TestMethod]
        public async Task HandlingProjectTopic_WhenProjectNotFound_ShouldNotAffectAnyProjects()
        {
            // Arrange
            var unknownProjectDescription = "UnknownProjectDescription";
            var unknownProjectName = "Project";
            var message = $"{{\"Plant\" : \"{plant}\", \"ProjectName\" : \"{unknownProjectName}\", \"IsClosed\" : true, \"Description\" : \"{unknownProjectDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Project, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(project1Name), Times.Never);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(unknownProjectName), Times.Once);
            Assert.AreNotEqual(unknownProjectDescription, _project1.Description);
            Assert.AreNotEqual(unknownProjectDescription, _project2.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfMissingPlan()
        {
            // Arrange
            var messageWithoutPlant = $"{{\"ProjectName\" : \"{project1Name}\", \"IsClosed\" : true, \"Description\" : \"{newDescription}\"}}";
            
            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Project, messageWithoutPlant, new CancellationToken(false));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfMissingPlantOrProjectName()
        {
            // Arrange
            var messageWithoutProjectName = $"{{\"Plant\" : \"{plant}\", \"IsClosed\" : true, \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Project, messageWithoutProjectName, new CancellationToken(false));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Project, message, new CancellationToken(false));
        }
        #endregion

        #region TagFunction
        [TestMethod]
        public async Task HandlingTagFunctionTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{ \"Plant\" : \"{plant}\", \"RegisterCode\" : \"{registerCode}\", \"Code\" : \"{tagFunctionCode}\", \"Description\" : \"{newDescription}\", \"IsVoided\" : true}}";
            Assert.AreNotEqual(newDescription, _tagFunction.Description);
            Assert.AreNotEqual(true, _tagFunction.IsVoided);

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(tagFunctionCode, registerCode), Times.Once);
            Assert.AreEqual(newDescription, _tagFunction.Description);
            Assert.AreEqual(true, _tagFunction.IsVoided);
        }

        [TestMethod]
        public async Task HandlingTagFunctionTopic_WithRenameWithoutFailure()
        {
            // Arrange
            var newRegisterCode = "A new register";
            var newTagFunctionCode = "And a new tag function Code";
            var message = $"{{ \"Plant\" : \"{plant}\", \"Code\" : \"{newTagFunctionCode}\", \"CodeOld\" : \"{tagFunctionCode}\", \"RegisterCode\" : \"{newRegisterCode}\", \"RegisterCodeOld\" : \"{registerCode}\", \"IsVoided\" : true, \"Description\" : \"{newDescription}\"}}";
            Assert.AreEqual(false, _tagFunction.IsVoided);

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(tagFunctionCode, registerCode), Times.Once);
            Assert.AreEqual(newDescription, _tagFunction.Description);
            Assert.AreEqual(true, _tagFunction.IsVoided);
            Assert.AreEqual(newRegisterCode, _tagFunction.RegisterCode);
            Assert.AreEqual(newTagFunctionCode, _tagFunction.Code);
        }

        [TestMethod]
        public async Task HandlingTagFunctionTopic_WhenCodeNotFound_ShouldNotAffectAnyTagFunctions()
        {
            // Arrange
            var unknownCode = "UnknownCode";
            var message = $"{{ \"Plant\" : \"{plant}\", \"RegisterCode\" : \"{registerCode}\", \"Code\" : \"{unknownCode}\", \"Description\" : \"{newDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(tagFunctionCode, registerCode), Times.Never);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(unknownCode, registerCode), Times.Once);
            Assert.AreNotEqual(newDescription, _tagFunction.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var messageWithoutPlant = $"{{ \"RegisterCode\" : \"{registerCode}\", \"Code\" : \"{tagFunctionCode}\", \"Description\" : \"{newDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, messageWithoutPlant, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingTagFunctionCode()
        {
            // Arrange
            var messageWithoutTagFunctionCode = $"{{ \"Plant\" : \"{plant}\", \"RegisterCode\" : \"{registerCode}\", \"Description\" : \"{newDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, messageWithoutTagFunctionCode, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingRegisterCode()
        {
            // Arrange
            var messageWithoutRegisterCode = $"{{ \"Plant\" : \"{plant}\", \"Code\" : \"{tagFunctionCode}\", \"Description\" : \"{newDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, messageWithoutRegisterCode, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.TagFunction, message, new CancellationToken(false));
        }
        #endregion

        #region Responsible
        [TestMethod]
        public async Task HandlingResponsibleTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{code}\", \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Once);
            Assert.AreEqual(newDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopic_WithChangedCode()
        {
            // Arrange
            var codeNew = "Code2";

            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{codeNew}\", \"CodeOld\" : \"{code}\", \"IsVoided\" : false, \"Description\" : \"{newDescription}\"}}";
            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Once);
            Assert.AreEqual(codeNew, _responsible.Code);
            Assert.AreEqual(newDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopic_WhenCodeNotFound_ShouldNotAffectAnyResponsibles()
        {
            // Arrange
            var unknownCode = "UnknownCode";
            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{unknownCode}\", \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Never);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(unknownCode), Times.Once);
            Assert.AreNotEqual(newDescription, _responsible.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var messageWithoutPlant = $"{{ \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{code}\", \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, messageWithoutPlant, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfMissingResponsibleCode()
        {
            // Arrange
            var messageWithoutResponsibleCode = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Description\" : \"{newDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, messageWithoutResponsibleCode, new CancellationToken(false));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));
        }
        #endregion

        #region CommPkg
        [TestMethod]
        public async Task HandlingCommPkgTopic_Move_WithoutFailure()
        {
            // Arrange
            var message = $"{{\"Plant\" : \"{plant}\", \"ProjectName\" : \"{project2Name}\", \"ProjectNameOld\" : \"{project1Name}\", \"CommPkgNo\" :\"{CommPkg1}\", \"Description\" : \"{description}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.CommPkg, message, new CancellationToken(false));

            // Assert
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _projectRepository.Verify(p => p.MoveCommPkgAsync(CommPkg1, project1Name, project2Name), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingCommPkgTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.CommPkg, message, new CancellationToken(false));
        }
        #endregion

        #region McPkg
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingMcPkgTopicMove_ShouldFailIfNotBothOldValuesEitherNullOrNot()
        {
            var message = $"{{\"Plant\" : \"Planten\", \"ProjectName\" : \"P1\", \"CommPkgNo\" :\"C1\", \"McPkgNo\" : \"M2\", \"McPkgNoOld\" : \"M2\", \"Description\" : \"Desc\"}}";

            await _dut.ProcessMessageAsync(PcsTopic.McPkg, message, new CancellationToken(false));
        }

        [TestMethod]
        public async Task HandlingMcPkgTopicMove_ShouldChangeAffectedTags()
        {
            // Arrange
            var toMcPkg = "B";
            var toCommPkg = "D";
            var message = $"{{\"Plant\" : \"{plant}\", \"ProjectName\" : \"{project1Name}\", \"CommPkgNo\" :\"{toCommPkg}\", \"CommPkgNoOld\" :\"{_tag1.CommPkgNo}\", \"McPkgNo\" : \"{toMcPkg}\", \"McPkgNoOld\" : \"{_tag1.McPkgNo}\", \"Description\" : \"Desc\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.McPkg, message, new CancellationToken(false));

            // Assert
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            Assert.AreEqual(toMcPkg, _tag1.McPkgNo);
            Assert.AreEqual(toCommPkg, _tag1.CommPkgNo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingMcPkgTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.McPkg, message, new CancellationToken(false));
        }
        #endregion

        #region Tag
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingTagNo()
        {
            // Arrange
            string message =
                $"{{\"ProjectName\" : \"{project1Name}\",\"Plant\" : \"{plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingProjectName()
        {
            // Arrange
            string message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"Plant\" : \"{plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            string message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"ProjectName\" : \"{project1Name}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldUpdateTag()
        {
            // Arrange
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFuncionCodeNew = "FCS123";
            string message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"Description\" : \"Test 123\",\"ProjectName\" : \"{project1Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFuncionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));


            // Assert
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline,_tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.IsTrue(_tag1.IsVoided);
            Assert.AreEqual(tagFuncionCodeNew, _tag1.TagFunctionCode);
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldUpdateTagAndChageTagNo()
        {
            // Arrange
            var tagNew = "123";
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFuncionCodeNew = "FCS123";
            string message =
                $"{{\"TagNo\" : \"{tagNew}\",\"TagNoOld\" : \"{TagNo1}\",\"Description\" : \"Test 123\",\"ProjectName\" : \"{project1Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFuncionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Tag, message, new CancellationToken(false));

            // Assert
            Assert.AreEqual(tagNew, _tag1.TagNo);
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline, _tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.IsTrue(_tag1.IsVoided);
            Assert.AreEqual(tagFuncionCodeNew, _tag1.TagFunctionCode);
        }
        #endregion
    }
}
