using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Certificate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.AutoTransfer
{
    [TestClass]
    public class AutoTransferCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _testProjectName = "ProjectA";
        private new readonly Guid _projectProCoSysGuid = new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8");
        private readonly string _testTagNo = "TagA";
        private readonly string _certificateNo = "CertificateA";
        private readonly Guid _rfccGuid = new Guid("{4270C978-D0A7-4485-82E0-146B6084FB20}");
        private readonly Guid _rfocGuid = new Guid("{4270C978-D0A7-4485-82E0-146B6084FB21}");
        private readonly Guid _tacGuid = new Guid("{4270C978-D0A7-4485-82E0-146B6084FB22}");
        private readonly int _step1OnJourneyId = 1;
        private readonly int _step2OnJourneyId = 2;
        private readonly string _rfcc = "RFCC";
        private readonly string _rfoc = "RFOC";
        private readonly string _tac = "TAC";

        private AutoTransferCommand _commandForRfcc;
        private AutoTransferCommand _commandForRfoc;
        private AutoTransferCommand _commandForOther;

        private AutoTransferCommandHandler _dut;
        private Tag _tag;

        private Mock<ILogger<AutoTransferCommandHandler>> _loggerMock;
        private Mock<ICertificateApiService> _certificateApiServiceMock;
        private Mock<IProjectRepository> _projectRepoMock;
        private PCSCertificateTagsModel _procosysCertificateTagsModel;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            var mode = new Mode(TestPlant, "M", false);
            var responsible = new Responsible(TestPlant, "RC", "RD");

            var step1OnJourney = new Step(TestPlant, "Step1", mode, responsible)
            {
                AutoTransferMethod = AutoTransferMethod.OnRfccSign
            };
            step1OnJourney.SetProtectedIdForTesting(_step1OnJourneyId);
            var step2OnJourney = new Step(TestPlant, "Step2", mode, responsible)
            {
                AutoTransferMethod = AutoTransferMethod.OnRfocSign
            };
            step2OnJourney.SetProtectedIdForTesting(_step2OnJourneyId);

            _journey = new Journey(TestPlant, "J1");
            _journey.AddStep(step1OnJourney);
            _journey.AddStep(step2OnJourney);

            var journeyRepoMock = new Mock<IJourneyRepository>();
            journeyRepoMock
                .Setup(r => r.GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod.OnRfccSign))
                .Returns(Task.FromResult(new List<Journey> {_journey}));
            journeyRepoMock
                .Setup(r => r.GetJourneysWithAutoTransferStepsAsync(AutoTransferMethod.OnRfocSign))
                .Returns(Task.FromResult(new List<Journey> {_journey}));

            var reqMock = new Mock<TagRequirement>();
            reqMock.SetupGet(r => r.Plant).Returns(TestPlant);
            
            _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), _testTagNo, "", step1OnJourney, new List<TagRequirement> {reqMock.Object});

            _tag.StartPreservation();

            _commandForRfcc = new AutoTransferCommand(_testProjectName, _certificateNo, _rfcc, _rfccGuid);
            _commandForRfoc = new AutoTransferCommand(_testProjectName, _certificateNo, _rfoc, _rfocGuid);
            _commandForOther = new AutoTransferCommand(_testProjectName, _certificateNo, _tac, _tacGuid);

            _procosysCertificateTagsModel = new PCSCertificateTagsModel()
            {
                CertificateIsAccepted = true,
                Tags = new List<PCSCertificateTag>
                {
                    new PCSCertificateTag
                    {
                        TagNo = _testTagNo
                    }
                }
            };
            _certificateApiServiceMock = new Mock<ICertificateApiService>();
            _certificateApiServiceMock.Setup(c => c.TryGetCertificateTagsAsync(TestPlant, _rfccGuid))
                .Returns(Task.FromResult(_procosysCertificateTagsModel));
            _certificateApiServiceMock.Setup(c => c.TryGetCertificateTagsAsync(TestPlant, _rfocGuid))
                .Returns(Task.FromResult(_procosysCertificateTagsModel));
            _certificateApiServiceMock.Setup(c => c.TryGetCertificateTagsAsync(TestPlant, _tacGuid))
                .Returns(Task.FromResult(_procosysCertificateTagsModel));

            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock
                .Setup(r => r.GetProjectOnlyByNameAsync(_testProjectName))
                .Returns(Task.FromResult(new Project(TestPlant, _testProjectName, "Desc", _projectProCoSysGuid)));
            _projectRepoMock
                .Setup(r => r.GetStandardTagsInProjectInStepsAsync(_testProjectName, new List<string> {_testTagNo},
                    new List<int> {_step1OnJourneyId}))
                .Returns(Task.FromResult(new List<Tag> {_tag}));
            _projectRepoMock
                .Setup(r => r.GetStandardTagsInProjectInStepsAsync(_testProjectName, new List<string> {_testTagNo},
                    new List<int> {_step2OnJourneyId}))
                .Returns(Task.FromResult(new List<Tag> {_tag}));
            
            _loggerMock = new Mock<ILogger<AutoTransferCommandHandler>>();
            
            _dut = new AutoTransferCommandHandler(
                _projectRepoMock.Object,
                journeyRepoMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _certificateApiServiceMock.Object,
                _loggerMock.Object);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ForRfccCertificate_ShouldTransferToNextStep_WhenNextStepExists()
        {
            // Act
            var result = await _dut.Handle(_commandForRfcc, default);

            // Assert
            Assert.AreEqual(_step2OnJourneyId, _tag.StepId);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ForRfocCertificate_ShouldTransferToNextStep_WhenNextStepExists()
        {
            // Arrange
            var step3OnJourneyId = 3;
            var step3OnJourney = new Step(
                TestPlant,
                "Step3", 
                new Mode(TestPlant, "M3", false),
                new Responsible(TestPlant, "RC3", "RD3"));
            step3OnJourney.SetProtectedIdForTesting(step3OnJourneyId);
            _journey.AddStep(step3OnJourney);
            await _dut.Handle(_commandForRfcc, default);
            _projectRepoMock
                .Setup(r => r.GetStandardTagsInProjectInStepsAsync(_testProjectName, new List<string> {_testTagNo},
                    new List<int> {_tag.StepId}))
                .Returns(Task.FromResult(new List<Tag> {_tag}));

            // Act
            var result = await _dut.Handle(_commandForRfoc, default);

            // Assert
            Assert.AreEqual(step3OnJourneyId, _tag.StepId);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ForOtherCertificate_ShouldDoNothing()
        {
            // Act
            await _dut.Handle(_commandForOther, default);

            // Assert
            Assert.AreEqual(_step1OnJourneyId, _tag.StepId);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ForNoAcceptedCertificate_ShouldDoNothing()
        {
            // Arrange
            _procosysCertificateTagsModel.CertificateIsAccepted = false;

            // Act
            await _dut.Handle(_commandForRfcc, default);

            // Assert
            Assert.AreEqual(_step1OnJourneyId, _tag.StepId);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ForUnknownCertificate_ShouldDoNothing_AndReturnNotFound()
        {
            // Arrange
            _certificateApiServiceMock.Setup(c => c.TryGetCertificateTagsAsync(TestPlant, _commandForRfcc.ProCoSysGuid))
                .Returns(Task.FromResult<PCSCertificateTagsModel>(null));

            // Act
            var result = await _dut.Handle(_commandForRfcc, default);

            // Assert
            Assert.AreEqual(_step1OnJourneyId, _tag.StepId);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Certificate {_commandForRfcc.ProCoSysGuid} not found", result.Errors[0]);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_OnUnknownProject_ShouldDoNothing()
        {
            // Arrange
            _projectRepoMock
                .Setup(r => r.GetProjectOnlyByNameAsync(_testProjectName))
                .Returns(Task.FromResult<Project>(null));

            // Act
            await _dut.Handle(_commandForRfcc, default);

            // Assert
            Assert.AreEqual(_step1OnJourneyId, _tag.StepId);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_OnClosedProject_ShouldDoNothing()
        {
            // Arrange
            _projectRepoMock
                .Setup(r => r.GetProjectOnlyByNameAsync(_testProjectName))
                .Returns(Task.FromResult(new Project(TestPlant, _testProjectName, "Desc",_projectProCoSysGuid){IsClosed = true}));

            // Act
            await _dut.Handle(_commandForRfcc, default);

            // Assert
            Assert.AreEqual(_step1OnJourneyId, _tag.StepId);
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ShouldThrowException_WhenTransferFromLastStep()
        {
            await _dut.Handle(_commandForRfcc, default);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_commandForRfoc, default)
            );
        }

        [TestMethod]
        public async Task HandlingAutoTransferCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_commandForRfcc, default);

            // Assert
            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}
