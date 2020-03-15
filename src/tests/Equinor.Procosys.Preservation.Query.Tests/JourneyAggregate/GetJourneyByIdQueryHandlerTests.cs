using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class GetJourneyByIdQueryHandlerTests
    {
        private const string TestPlant = "PlantA";
        private const int JourneyId = 162;
        private const int ModeId = 72;
        private const int RespId = 17;
        private Mock<IJourneyRepository> _journeyRepoMock;
        private Mock<IModeRepository> _modeRepoMock;
        private Mock<IResponsibleRepository> _respRepoMock;
        private Journey _journey;
        private Step _step;

        private GetJourneyByIdQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Id).Returns(ModeId);
            modeMock.SetupGet(m => m.Schema).Returns(TestPlant);

            var respMock = new Mock<Responsible>();
            respMock.SetupGet(r => r.Id).Returns(RespId);
            respMock.SetupGet(r => r.Schema).Returns(TestPlant);

            _step = new Step(TestPlant, "S", modeMock.Object, respMock.Object);

            _journey = new Journey(TestPlant, "TitleA");
            _journey.AddStep(_step);

            _modeRepoMock = new Mock<IModeRepository>();
            _modeRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.FromResult(new List<Mode> {modeMock.Object}));
            
            _respRepoMock = new Mock<IResponsibleRepository>();
            _respRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.FromResult(new List<Responsible> {respMock.Object}));

            _journeyRepoMock = new Mock<IJourneyRepository>();
            _journeyRepoMock.Setup(r => r.GetByIdAsync(JourneyId)).Returns(Task.FromResult(_journey));
            
            _dut = new GetJourneyByIdQueryHandler(_journeyRepoMock.Object, _modeRepoMock.Object, _respRepoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_KnownId_ShouldReturnJourney()
        {
            var result = await _dut.Handle(new GetJourneyByIdQuery(JourneyId), default);

            var journey = result.Data;
            
            Assert.AreEqual(_journey.Title, journey.Title);
            
            var steps = journey.Steps.ToList();
            Assert.AreEqual(1, steps.Count);

            var step = steps.First();
            Assert.IsNotNull(step.Mode);
            Assert.IsNotNull(step.Responsible);
            Assert.AreEqual(ModeId, step.Mode.Id);
            Assert.AreEqual(RespId, step.Responsible.Id);
        }

        [TestMethod]
        public async Task HandleGetJourneyByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            var result = await _dut.Handle(new GetJourneyByIdQuery(1525), default);

            Assert.IsNull(result.Data);
        }
    }
}
