﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Common.Telemetry;
using Equinor.ProCoSys.Preservation.Domain;

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
        private Mock<ICertificateEventProcessorService> _certificationEventProcessorService;
        private Mock<ICurrentUserSetter> _currentUserSetter;

        private const string Plant = "PCS$HEIMDAL";
        private const string Code = "Resp_Code";
        private const string Description = "789";
        private const string NewDescription = "Odfjeld Drilling Instalation";
        private const string Project1Name = "Project 1";
        private const string Project2Name = "Project 2";
        private readonly Guid ProjectProCoSysGuid1 = new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8");
        private readonly Guid ProjectProCoSysGuid2 = new Guid("6afabbbf-cf21-4533-93ff-73fe6fdfd27a");
        private const string Project1Description = "Description 1";
        private const string Project2Description = "Description 2";
        private const string TagFunctionCode = "Code9";
        private const string RegisterCode = "ElRegisterCode";
        private const string TagFunctionDescription = "Tag function description";

        private Tag _tag1;
        private Tag _tag2;
        private Project _newProjectCreated;
        private string _projectNotInPreservation ="ProjectNotInPres";
        
        private const string TagNo1 = "TagNo1";
        private const string TagNo2 = "TagNo2";
        private const string OldTagDescription1 = "OldTagDescription1";
        private const string OldTagDescription2 = "OldTagDescription2";

        private const string ProCoSysTagGuid = "EB38CCCAAE98D926E0532810000AC5B2";

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
            _certificationEventProcessorService = new Mock<ICertificateEventProcessorService>();

            // Assert tags in preservation
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(Plant);

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(Plant);
            _tag1 = new Tag(Plant, TagType.Standard, Guid.NewGuid(), TagNo1, OldTagDescription1, stepMock.Object, new List<TagRequirement>
            {
                new TagRequirement(Plant, 4, rdMock.Object)
            });
            _tag1.McPkgNo = McPkg1;
            _tag1.CommPkgNo = CommPkg1;

            _tag2 = new Tag(Plant, TagType.Standard, Guid.NewGuid(), TagNo2, OldTagDescription2, stepMock.Object, new List<TagRequirement>
            {
                new TagRequirement(Plant, 4, rdMock.Object)
            });
            _tag2.McPkgNo = McPkg2;
            _tag2.CommPkgNo = CommPkg2;

            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            _responsible = new Responsible(Plant, Code, Description);
            _responsibleRepository.Setup(r => r.GetByCodeAsync(Code)).Returns(Task.FromResult(_responsible));

            _project1 = new Project(Plant, Project1Name, Project1Description, ProjectProCoSysGuid1);
            _project1.AddTag(_tag1);
            _project1.AddTag(_tag2);
            _projectRepository.Setup(p => p.GetStandardTagsInProjectOnlyAsync(Project1Name))
                .Returns(Task.FromResult(tags));
            _projectRepository.Setup(p => p.GetProjectWithTagsByNameAsync(Project1Name))
                .Returns(Task.FromResult(_project1));
            _projectRepository.Setup(p => p.GetProjectOnlyByNameAsync(Project1Name))
                .Returns(Task.FromResult(_project1));

            _projectRepository.Setup(p => p.GetProjectOnlyByTagGuidAsync(Guid.Parse(ProCoSysTagGuid)))
                .Returns(Task.FromResult(_project1));
            _project2 = new Project(Plant, Project2Name, Project2Description, ProjectProCoSysGuid2);
            _projectRepository.Setup(p => p.GetProjectWithTagsByNameAsync(Project2Name))
                .Returns(Task.FromResult(_project2));
            _projectRepository.Setup(p => p.GetProjectOnlyByNameAsync(Project2Name))
                .Returns(Task.FromResult(_project2));
            _projectRepository.Setup(p => p.Add(It.IsAny<Project>())).Callback((Project p) => _newProjectCreated = p);
            _projectRepository.Setup(p => p.GetTagOnlyByGuidAsync(Guid.Parse(ProCoSysTagGuid))).Returns(Task.FromResult(_tag1));


            _projectRepository.Setup(p => p.GetTagOnlyByGuidAsync(Guid.ParseExact("EB38CCCAAE98D926E0532810000AC5B2", "N"))).Returns(Task.FromResult(_tag1));


            _tagFunction = new TagFunction(Plant, TagFunctionCode, TagFunctionDescription, RegisterCode);
            _tagFunctionRepository.Setup(t => t.GetByCodesAsync(TagFunctionCode, RegisterCode))
                .Returns(Task.FromResult(_tagFunction));
            var options = new Mock<IOptionsSnapshot<ApplicationOptions>>();
            options.Setup(s => s.Value).Returns(new ApplicationOptions{ObjectId = Guid.NewGuid()});
            _currentUserSetter = new Mock<ICurrentUserSetter>();
            var projectApiService = new Mock<IProjectApiService>();
            projectApiService.Setup(p => p.TryGetProjectAsync(Plant, _projectNotInPreservation, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new ProCoSysProject{Description = "Project Description", IsClosed = false, Name = _projectNotInPreservation}));

            _dut = new BusReceiverService(_plantSetter.Object,
                                          _unitOfWork.Object,
                                          _telemetryClient.Object,
                                          _responsibleRepository.Object,
                                          _projectRepository.Object,
                                          _tagFunctionRepository.Object,
                                          _currentUserSetter.Object,
                                          options.Object,
                                          projectApiService.Object,
                                          _certificationEventProcessorService.Object);
        }
        #endregion

        #region Project
        [TestMethod]
        public async Task HandlingProjectTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project1Name}\", \"IsClosed\" : true, \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Project, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(Project1Name), Times.Once);
            Assert.AreEqual(NewDescription, _project1.Description);
            Assert.AreEqual(true, _project1.IsClosed);
        }

        [TestMethod]
        public async Task HandlingProjectTopic_WhenProjectNotFound_ShouldNotAffectAnyProjects()
        {
            // Arrange
            var unknownProjectDescription = "UnknownProjectDescription";
            var unknownProjectName = "Project";
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{unknownProjectName}\", \"IsClosed\" : true, \"Description\" : \"{unknownProjectDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Project, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(Project1Name), Times.Never);
            _projectRepository.Verify(i => i.GetProjectOnlyByNameAsync(unknownProjectName), Times.Once);
            Assert.AreNotEqual(unknownProjectDescription, _project1.Description);
            Assert.AreNotEqual(unknownProjectDescription, _project2.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var messageWithoutPlant = $"{{\"ProjectName\" : \"{Project1Name}\", \"IsClosed\" : true, \"Description\" : \"{NewDescription}\"}}";
            
            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Project, messageWithoutPlant, default);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfMissingPlantOrProjectName()
        {
            // Arrange
            var messageWithoutProjectName = $"{{\"Plant\" : \"{Plant}\", \"IsClosed\" : true, \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Project, messageWithoutProjectName, default);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingProjectTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Project, message, default);
        }
        #endregion

        #region TagFunction
        [TestMethod]
        public async Task HandlingTagFunctionTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{ \"Plant\" : \"{Plant}\", \"RegisterCode\" : \"{RegisterCode}\", \"Code\" : \"{TagFunctionCode}\", \"Description\" : \"{NewDescription}\", \"IsVoided\" : true}}";
            Assert.AreNotEqual(NewDescription, _tagFunction.Description);
            Assert.AreNotEqual(true, _tagFunction.IsVoided);

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(TagFunctionCode, RegisterCode), Times.Once);
            Assert.AreEqual(NewDescription, _tagFunction.Description);
            Assert.AreEqual(true, _tagFunction.IsVoided);
        }

        [TestMethod]
        public async Task HandlingTagFunctionTopic_WithRenameWithoutFailure()
        {
            // Arrange
            var newRegisterCode = "A new register";
            var newTagFunctionCode = "And a new tag function Code";
            var message = $"{{ \"Plant\" : \"{Plant}\", \"Code\" : \"{newTagFunctionCode}\", \"CodeOld\" : \"{TagFunctionCode}\", \"RegisterCode\" : \"{newRegisterCode}\", \"RegisterCodeOld\" : \"{RegisterCode}\", \"IsVoided\" : true, \"Description\" : \"{NewDescription}\"}}";
            Assert.AreEqual(false, _tagFunction.IsVoided);

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(TagFunctionCode, RegisterCode), Times.Once);
            Assert.AreEqual(NewDescription, _tagFunction.Description);
            Assert.AreEqual(true, _tagFunction.IsVoided);
            Assert.AreEqual(newRegisterCode, _tagFunction.RegisterCode);
            Assert.AreEqual(newTagFunctionCode, _tagFunction.Code);
        }

        [TestMethod]
        public async Task HandlingTagFunctionTopic_WhenCodeNotFound_ShouldNotAffectAnyTagFunctions()
        {
            // Arrange
            var unknownCode = "UnknownCode";
            var message = $"{{ \"Plant\" : \"{Plant}\", \"RegisterCode\" : \"{RegisterCode}\", \"Code\" : \"{unknownCode}\", \"Description\" : \"{NewDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(TagFunctionCode, RegisterCode), Times.Never);
            _tagFunctionRepository.Verify(i => i.GetByCodesAsync(unknownCode, RegisterCode), Times.Once);
            Assert.AreNotEqual(NewDescription, _tagFunction.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var messageWithoutPlant = $"{{ \"RegisterCode\" : \"{RegisterCode}\", \"Code\" : \"{TagFunctionCode}\", \"Description\" : \"{NewDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, messageWithoutPlant, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingTagFunctionCode()
        {
            // Arrange
            var messageWithoutTagFunctionCode = $"{{ \"Plant\" : \"{Plant}\", \"RegisterCode\" : \"{RegisterCode}\", \"Description\" : \"{NewDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, messageWithoutTagFunctionCode, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfMissingRegisterCode()
        {
            // Arrange
            var messageWithoutRegisterCode = $"{{ \"Plant\" : \"{Plant}\", \"Code\" : \"{TagFunctionCode}\", \"Description\" : \"{NewDescription}\", \"IsVoided\" : false}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, messageWithoutRegisterCode, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagFunctionTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.TagFunction, message, default);
        }
        #endregion

        #region Responsible
        [TestMethod]
        public async Task HandlingResponsibleTopicWithoutFailure()
        {
            // Arrange
            var message = $"{{ \"Plant\" : \"{Plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{Code}\", \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(Code), Times.Once);
            Assert.AreEqual(NewDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopic_WithChangedCode()
        {
            // Arrange
            var codeNew = "Code2";

            var message = $"{{ \"Plant\" : \"{Plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{codeNew}\", \"CodeOld\" : \"{Code}\", \"IsVoided\" : false, \"Description\" : \"{NewDescription}\"}}";
            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(Code), Times.Once);
            Assert.AreEqual(codeNew, _responsible.Code);
            Assert.AreEqual(NewDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopic_WhenCodeNotFound_ShouldNotAffectAnyResponsibles()
        {
            // Arrange
            var unknownCode = "UnknownCode";
            var message = $"{{ \"Plant\" : \"{Plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{unknownCode}\", \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, message, default);

            // Assert
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(Code), Times.Never);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(unknownCode), Times.Once);
            Assert.AreNotEqual(NewDescription, _responsible.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var messageWithoutPlant = $"{{ \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{Code}\", \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, messageWithoutPlant, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfMissingResponsibleCode()
        {
            // Arrange
            var messageWithoutResponsibleCode = $"{{ \"Plant\" : \"{Plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Description\" : \"{NewDescription}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, messageWithoutResponsibleCode, default);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingResponsibleTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Responsible, message, default);
        }
        #endregion

        #region CommPkg
        [TestMethod]
        public async Task HandlingCommPkgTopic_Move_WithoutFailure()
        {
            // Arrange & Assert
            Assert.IsFalse(_project2.Tags.Contains(_tag1));
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project2Name}\", \"ProjectNameOld\" : \"{Project1Name}\", \"CommPkgNo\" :\"{CommPkg1}\", \"Description\" : \"{Description}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.CommPkg, message, default);

            // Assert
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            Assert.IsTrue(_project2.Tags.Contains(_tag1));
        }

        [TestMethod]
        public async Task HandlingCommPkgTopic_Move_ShouldCreateWhenProjectIsNotFoundLocallyButInMain()
        {
            // Arrange & Assert
            Assert.IsTrue(_project1.Tags.Contains(_tag1));
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{_projectNotInPreservation}\", \"ProjectNameOld\" : \"{Project1Name}\", \"CommPkgNo\" :\"{CommPkg1}\", \"Description\" : \"{Description}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.CommPkg, message, default);

            // Assert
            _projectRepository.Verify(p => p.Add(It.IsAny<Project>()));
            Assert.IsTrue(_newProjectCreated.Tags.Contains(_tag1));
            Assert.IsFalse(_project1.Tags.Contains(_tag1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task HandlingCommPkgTopic_Move_ShouldFailWhenProjectIsNotFoundInMain()
        {
            // Arrange & Assert
            var unknownProject = "NotKnown";
            Assert.IsFalse(_project2.Tags.Contains(_tag1));
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{unknownProject}\", \"ProjectNameOld\" : \"{Project1Name}\", \"CommPkgNo\" :\"{CommPkg1}\", \"Description\" : \"{Description}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.CommPkg, message, default);
        }

        [TestMethod]
        public async Task HandlingCommPkgTopic_Move_ShouldDoNothingIfFromProjectIsUnknown()
        {
            // Arrange & Assert
            var unknownProject = "NotKnown";
            Assert.IsFalse(_project2.Tags.Contains(_tag1));
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project1Name}\", \"ProjectNameOld\" : \"{unknownProject}\", \"CommPkgNo\" :\"{CommPkg1}\", \"Description\" : \"{Description}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.CommPkg, message, default);

            // Assert
            _projectRepository.Verify(p => p.GetProjectWithTagsByNameAsync(unknownProject), Times.Once);
            _projectRepository.Verify(p => p.GetProjectWithTagsByNameAsync(Project1Name), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingCommPkgTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.CommPkg, message, default);
        }
        #endregion

        #region McPkg
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingMcPkgTopicMove_ShouldFailIfNotBothOldValuesEitherNullOrNot()
        {
            var message = "{\"Plant\" : \"OnePlant\", \"ProjectName\" : \"P1\", \"CommPkgNo\" :\"C1\", \"McPkgNo\" : \"M2\", \"McPkgNoOld\" : \"M2\", \"Description\" : \"Desc\"}";

            await _dut.ProcessMessageAsync(PcsTopicConstants.McPkg, message, default);
        }

        [TestMethod]
        public async Task HandlingMcPkgTopicMove_ShouldChangeAffectedTags()
        {
            // Arrange
            var toMcPkg = "B";
            var toCommPkg = "D";
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project1Name}\", \"CommPkgNo\" :\"{toCommPkg}\", \"CommPkgNoOld\" :\"{_tag1.CommPkgNo}\", \"McPkgNo\" : \"{toMcPkg}\", \"McPkgNoOld\" : \"{_tag1.McPkgNo}\", \"Description\" : \"Desc\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.McPkg, message, default);

            // Assert
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            Assert.AreEqual(toMcPkg, _tag1.McPkgNo);
            Assert.AreEqual(toCommPkg, _tag1.CommPkgNo);
        }

        [TestMethod]
        public async Task HandlingMcPkgTopicMove_ShouldNotFailIfProjectIsUnknowToPreservation()
        {
            // Arrange
            var toMcPkg = "B";
            var toCommPkg = "D";
            var unknowProject = "ProjectUnknown";
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{unknowProject}\", \"CommPkgNo\" :\"{toCommPkg}\", \"CommPkgNoOld\" :\"{_tag1.CommPkgNo}\", \"McPkgNo\" : \"{toMcPkg}\", \"McPkgNoOld\" : \"{_tag1.McPkgNo}\", \"Description\" : \"Desc\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.McPkg, message, default);

            // Assert
            _plantSetter.Verify(p => p.SetPlant(Plant), Times.Once);
            Assert.AreNotEqual(toMcPkg, _tag1.McPkgNo);
            Assert.AreNotEqual(toCommPkg, _tag1.CommPkgNo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingMcPkgTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.McPkg, message, default);
        }
        #endregion

        #region Tag
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandlingTagTopic_ShouldFailIfEmpty()
        {
            // Arrange
            var message = "{}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingTagNo()
        {
            // Arrange
            var message =
                $"{{\"ProjectName\" : \"{Project1Name}\",\"Plant\" : \"{Plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingProjectName()
        {
            // Arrange
            var message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"Plant\" : \"{Plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleTagTopic_ShouldFailIfMissingPlant()
        {
            // Arrange
            var message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"ProjectName\" : \"{Project1Name}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);
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
            var tagFunctionCodeNew = "FCS123";
            var message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"ProCoSysGuid\" : \"{ProCoSysTagGuid}\",\"Description\" : \"Test 123\",\"ProjectName\" : \"{Project1Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFunctionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{Plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline,_tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.AreEqual(tagFunctionCodeNew, _tag1.TagFunctionCode);
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldUpdateTagAndChangeTagNo()
        {
            // Arrange
            var tagNew = "123";
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFunctionCodeNew = "FCS123";
            var message =
                $"{{\"TagNo\" : \"{tagNew}\",\"TagNoOld\" : \"{TagNo1}\",\"ProCoSysGuid\" : \"{ProCoSysTagGuid}\",\"Description\" : \"Test 123\",\"ProjectName\" : \"{Project1Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFunctionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{Plant}\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.AreEqual(tagNew, _tag1.TagNo);
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline, _tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.AreEqual(tagFunctionCodeNew, _tag1.TagFunctionCode);
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldUpdateBothIsVoidedAndIsVoidedInSourceProperty()
        {
            // Arrange
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFunctionCodeNew = "FCS123";
            var message =
                $"{{\"TagNo\" : \"{TagNo1}\",\"ProCoSysGuid\" : \"{ProCoSysTagGuid}\",\"Description\" : \"Test 123\",\"ProjectName\" : \"{Project1Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFunctionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{Plant}\"}}";

            Assert.IsFalse(_tag1.IsVoided);
            Assert.IsFalse(_tag1.IsVoidedInSource);

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.IsTrue(_tag1.IsVoided);
            Assert.IsTrue(_tag1.IsVoidedInSource);
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldMoveTagAndChangeTagNo()
        {
            // Arrange
            var tagNew = "123";
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFunctionCodeNew = "FCS123";
            var message =
                $"{{\"TagNo\" : \"{tagNew}\",\"TagNoOld\" : \"{TagNo1}\",\"ProCoSysGuid\" : \"{ProCoSysTagGuid}\",\"Description\" : \"Test 123\",\"ProjectNameOld\" : \"{Project1Name}\",\"ProjectName\" : \"{Project2Name}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFunctionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{Plant}\"}}";

            // Assert
            Assert.IsTrue(_project1.Tags.Contains(_tag1));
            Assert.IsFalse(_project2.Tags.Contains(_tag1));

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.AreEqual(tagNew, _tag1.TagNo);
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline, _tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.AreEqual(tagFunctionCodeNew, _tag1.TagFunctionCode);
            Assert.IsTrue( _project2.Tags.Contains(_tag1));
        }

        [TestMethod]
        public async Task HandleTagTopic_ShouldCreateProjectMoveTagAndChangeTagNo()
        {
            // Arrange
            var tagNew = "123";
            var area = "Area69";
            var areaDescription = "Area69 Description";
            var discipline = "ABC";
            var disciplineDescription = "ABC Desc";
            var callOffNo = "123";
            var poNo = "321";
            var tagFunctionCodeNew = "FCS123";

            var message =
                $"{{\"TagNo\" : \"{tagNew}\",\"TagNoOld\" : \"{TagNo1}\",\"ProCoSysGuid\" : \"{ProCoSysTagGuid}\",\"Description\" : \"Test 123\",\"ProjectNameOld\" : \"{Project1Name}\",\"ProjectName\" : \"{_projectNotInPreservation}\",\"McPkgNo\" : \"{McPkg1}\",\"CommPkgNo\" : \"{CommPkg1}\",\"AreaCode\" : \"{area}\",\"AreaDescription\" : \"{areaDescription}\",\"DisciplineCode\" : \"{discipline}\",\"DisciplineDescription\" : \"{disciplineDescription}\",\"CallOffNo\" : \"{callOffNo}\",\"PurchaseOrderNo\" : \"{poNo}\",\"TagFunctionCode\" : \"{tagFunctionCodeNew}\",\"IsVoided\" : true,\"Plant\" : \"{Plant}\"}}";

            // Assert
            Assert.IsTrue(_project1.Tags.Contains(_tag1));
            Assert.IsNull(_newProjectCreated);

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.AreEqual(tagNew, _tag1.TagNo);
            Assert.AreEqual(area, _tag1.AreaCode);
            Assert.AreEqual(areaDescription, _tag1.AreaDescription);
            Assert.AreEqual(discipline, _tag1.DisciplineCode);
            Assert.AreEqual(disciplineDescription, _tag1.DisciplineDescription);
            Assert.AreEqual(callOffNo, _tag1.Calloff);
            Assert.AreEqual(poNo, _tag1.PurchaseOrderNo);
            Assert.AreEqual(tagFunctionCodeNew, _tag1.TagFunctionCode);
            Assert.IsNotNull(_newProjectCreated);
            Assert.IsTrue(_newProjectCreated.Tags.Contains(_tag1));
        }

        #endregion

        #region Certificate

        [TestMethod]
        public async Task HandleCertificateTopic_ShouldCall_CertificateEventProcessorService_WithoutBehavior()
        {
            // Arrange
            var message = 
                $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project1Name}\", \"CertificateNo\" :\"XX\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Certificate, message, default);

            // Assert
            _certificationEventProcessorService.Verify(u => u.ProcessCertificateEventAsync(message), Times.Once);

        }

        [TestMethod]
        public async Task HandleCertificateTopic_ShouldCall_CertificateEventProcessorService_WithBehavior()
        {
            // Arrange
            var message =
                $"{{\"Plant\" : \"{Plant}\", \"ProjectName\" : \"{Project1Name}\", \"CertificateNo\" :\"XX\", \"Behavior\" :\"delete\"}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Certificate, message, default);

            // Assert
            _certificationEventProcessorService.Verify(u => u.ProcessCertificateEventAsync(message), Times.Once);

        }
        #endregion

        #region Delete

        [TestMethod]
        public async Task HandleDeleteTopic_ForTag_ShouldDeleteTag()
        {
            // Arrange
            var guid = _tag1.Guid;
            var message = $"{{\"Plant\" : \"{Plant}\", \"ProCoSysGuid\" : \"{guid}\",\"Behavior\" : \"delete\"}}";
            _projectRepository.Setup(p => p.GetTagOnlyByGuidAsync(guid))
                .Returns(Task.FromResult(_tag1));
            Assert.IsFalse(_tag1.IsDeletedInSource);
            // Act
            await _dut.ProcessMessageAsync(PcsTopicConstants.Tag, message, default);

            // Assert
            Assert.IsTrue(_tag1.IsDeletedInSource);
        }

        [TestMethod]
        public async Task HandleDeleteTopic_ForProject_ShouldIgnoreMessage()
            => await HandleDeleteTopic_ShouldIgnoreMessage(Guid.NewGuid(), PcsTopicConstants.Project);

        [TestMethod]
        public async Task HandleDeleteTopic_ForResponsible_ShouldIgnoreMessage()
            => await HandleDeleteTopic_ShouldIgnoreMessage(Guid.NewGuid(), PcsTopicConstants.Responsible);

        [TestMethod]
        public async Task HandleDeleteTopic_ForTagFunction_ShouldIgnoreMessage()
            => await HandleDeleteTopic_ShouldIgnoreMessage(Guid.NewGuid(), PcsTopicConstants.TagFunction);

        [TestMethod]
        public async Task HandleDeleteTopic_ForCommPkg_ShouldIgnoreMessage()
            => await HandleDeleteTopic_ShouldIgnoreMessage(Guid.NewGuid(), PcsTopicConstants.CommPkg);

        [TestMethod]
        public async Task HandleDeleteTopic_ForMcPkg_ShouldIgnoreMessage()
            => await HandleDeleteTopic_ShouldIgnoreMessage(Guid.NewGuid(), PcsTopicConstants.McPkg);

        private async Task HandleDeleteTopic_ShouldIgnoreMessage(Guid guid, string topic)
        {
            // Arrange
            var message = $"{{\"ProCoSysGuid\" : \"{guid}\",\"Behavior\" : \"delete\"}}";

            // Act
            await _dut.ProcessMessageAsync(topic, message, default);

            // Assert
            _telemetryClient.Verify(tc => tc.TrackEvent("Preservation Bus Receiver",
                new Dictionary<string, string>
                {
                    {"Event Delete", topic},
                    {"ProCoSysGuid", guid.ToString()},
                    {"Supported", "False"}
                }, null),Times.Once());

            // processing message should return before setting plant
            _plantSetter.VerifyNoOtherCalls();
        }
        #endregion
    }
}
