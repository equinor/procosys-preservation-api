using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.IPO.WebApi.Synchronization;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.WebApi.Authentication;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Equinor.Procosys.Preservation.WebApi.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Synchronization
{
    [TestClass]
    public class BusReceiverServiceTests
    {
        private BusReceiverService _dut;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IPlantSetter> _plantSetter;
        private Mock<ITelemetryClient> _telemetryClient;
        private Mock<IReadOnlyContext> _readOnlyContext;
        private Mock<IApplicationAuthenticator> _applicationAuthenticator;
        private Mock<IBearerTokenSetter> _bearerTokenSetter;
        private Responsible _responsible;
        private Mock<IResponsibleRepository> _responsibleRepository;

        private const string plant = "PCS$HEIMDAL";
        private const string code = "Resp_Code";
        private const string description = "789";
        private const string newDescription = "Odfjeld Drilling Instalation";

        [TestInitialize]
        public void Setup()
        {
            _plantSetter = new Mock<IPlantSetter>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _telemetryClient = new Mock<ITelemetryClient>();
            _readOnlyContext = new Mock<IReadOnlyContext>();
            _applicationAuthenticator = new Mock<IApplicationAuthenticator>();
            _bearerTokenSetter = new Mock<IBearerTokenSetter>();
            _responsibleRepository = new Mock<IResponsibleRepository>();
            
            _responsible = new Responsible(plant, code, description);
            _responsibleRepository.Setup(r => r.GetByCodeAsync(code)).Returns(Task.FromResult(_responsible));

            _dut = new BusReceiverService(_plantSetter.Object,
                                          _unitOfWork.Object,
                                          _telemetryClient.Object,
                                          _readOnlyContext.Object,
                                          _applicationAuthenticator.Object,
                                          _bearerTokenSetter.Object,
                                          _responsibleRepository.Object);

            var list = new List<Responsible> {_responsible};
        }

        [TestMethod]
        public async Task HandlingResponsibleTopicWithoutFailure()
        {
            

            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{code}\", \"Description\" : \"{newDescription}\"}}";
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Once);
            Assert.AreEqual(newDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopi_WhenCodeNotFound_ShouldNotAffectAnyResponsibles()
        {
            var unknownCode = "UnknownCode";
            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{unknownCode}\", \"Description\" : \"{newDescription}\"}}";
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Never);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(unknownCode), Times.Once);
            Assert.AreNotEqual(newDescription, _responsible.Description);
        }

        [TestMethod]
        public async Task HandlingResponsibleTopi_WhenNoResponsiblesInPlant_ShouldNotFail()
        {
            var busReceiverUnderTest = new BusReceiverService(_plantSetter.Object,
                _unitOfWork.Object,
                _telemetryClient.Object,
                _readOnlyContext.Object,
                _applicationAuthenticator.Object,
                _bearerTokenSetter.Object,
                new Mock<IResponsibleRepository>().Object);

            var message = $"{{ \"Plant\" : \"{plant}\", \"ResponsibleGroup\" : \"INSTALLATION\", \"Code\" : \"{code}\", \"Description\" : \"{newDescription}\"}}";
            await busReceiverUnderTest.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));

            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _plantSetter.Verify(p => p.SetPlant(plant), Times.Once);
            _responsibleRepository.Verify(i => i.GetByCodeAsync(code), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task HandlingResponsibleTopic_ShouldFailIfEmpty()
        {

            var message = $"{{}}";

            // Act
            await _dut.ProcessMessageAsync(PcsTopic.Responsible, message, new CancellationToken(false));
        }

    }
}
