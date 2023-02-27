using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.PcsServiceBus.Enums;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Synchronization
{
    [TestClass]
    public class CertificateEventProcessorServiceTests
    {
        private CertificateTopic _certificateTopic = new CertificateTopic
        {
            Plant = "PCS$HEIMDAL",
            CertificateNo = "CO-OP-0065",
            CertificateStatus = CertificateStatus.Accepted,
            CertificateType = "RFOC",
            ProCoSysGuid = new Guid("e92bb49b-196e-e9ba-e053-2910000a8f36")
        };

        private CertificateEventProcessorService _dut;
        private Mock<ILogger<CertificateEventProcessorService>> _logger;
        private Mock<ITelemetryClient> _telemetryClient;
        private Mock<IMediator> _mediator;
        private Mock<IPlantSetter> _plantSetter;

        [TestInitialize]
        public void Setup()
        {
            _plantSetter = new Mock<IPlantSetter>();
            _logger = new Mock<ILogger<CertificateEventProcessorService>>();
            _telemetryClient = new Mock<ITelemetryClient>();
            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(x => x.Send(It.IsAny<AutoTransferCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SuccessResult<Unit>(Unit.Value) as Result<Unit>));
            _dut = new CertificateEventProcessorService(
                _logger.Object,
                _telemetryClient.Object,
                _mediator.Object, 
                _plantSetter.Object);
        }

        [TestMethod]
        public async Task ProcessCertificateEventAsync_Should_SetPlant()
        {
            // Arrange
            var message = GetValidMessage();

            // Act
            await _dut.ProcessCertificateEventAsync(message);

            // Assert
            _plantSetter.Verify(p => p.SetPlant(_certificateTopic.Plant), Times.Once);
        }

        [TestMethod]
        public async Task ProcessCertificateEventAsync_Should_Send_AutoTransferCommand_ToMediator()
        {
            // Arrange
            var message = GetValidMessage();

            // Act
            await _dut.ProcessCertificateEventAsync(message);

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<AutoTransferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ProcessCertificateEventAsync_WithoutPlant_ShouldThrowException()
        {
            // Arrange
            _certificateTopic.Plant = null;
            var message = GetValidMessage();

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _dut.ProcessCertificateEventAsync(message));
        }

        [TestMethod]
        public async Task ProcessCertificateEventAsync_WithoutCertificateNo_ShouldThrowException()
        {
            // Arrange
            _certificateTopic.CertificateNo = null;
            var message = GetValidMessage();

            // Act
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _dut.ProcessCertificateEventAsync(message));
        }

        [TestMethod]
        public async Task ProcessCertificateEventAsync_WithCertificateType_OtherThan_RFOC_RFCC_Should_NotSend_AutoTransferCommand_ToMediator()
        {
            // Arrange
            _certificateTopic.CertificateType = "LUN";
            var message = GetValidMessage();

            // Act
            await _dut.ProcessCertificateEventAsync(message);

            // Assert
            _mediator.Verify(x => x.Send(It.IsAny<AutoTransferCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private string GetValidMessage() => JsonSerializer.Serialize(_certificateTopic);
    }
}
