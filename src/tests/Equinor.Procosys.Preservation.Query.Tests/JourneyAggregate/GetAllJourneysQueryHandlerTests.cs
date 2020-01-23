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
    public class GetAllJourneysQueryHandlerTests
    {
        private const int ModeId = 72;
        private const int RespId = 17;
        private Mock<IJourneyRepository> _journeyRepoMock;
        private Mock<IModeRepository> _modeRepoMock;
        private Mock<IResponsibleRepository> _respRepoMock;
        private Journey _journey;
        private Journey _journeyVoided;
        private Step _step;
        private Step _stepVoided;

        private GetAllJourneysQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Id).Returns(ModeId);

            var respMock = new Mock<Responsible>();
            respMock.SetupGet(m => m.Id).Returns(RespId);

            _step = new Step("SchemaA", modeMock.Object, respMock.Object);
            _stepVoided = new Step("SchemaA", modeMock.Object, respMock.Object);
            _stepVoided.Void();

            _journey = new Journey("SchemaA", "TitleA");
            _journey.AddStep(_step);
            _journey.AddStep(_stepVoided);

            _journeyVoided = new Journey("SchemaA", "TitleB");
            _journeyVoided.Void();
            
            _modeRepoMock = new Mock<IModeRepository>();
            _modeRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.FromResult(new List<Mode> {modeMock.Object}));
            
            _respRepoMock = new Mock<IResponsibleRepository>();
            _respRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.FromResult(new List<Responsible> {respMock.Object}));

            var journeys = new List<Journey>
            {
                _journey,
                _journeyVoided
            };
            _journeyRepoMock = new Mock<IJourneyRepository>();
            _journeyRepoMock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(journeys));

            
            _dut = new GetAllJourneysQueryHandler(_journeyRepoMock.Object, _modeRepoMock.Object, _respRepoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetAllJourneysQuery_ShouldGetNonVoidedJourneysOnly_WhenNotGettingVoided()
        {
            var result = await _dut.Handle(new GetAllJourneysQuery(false), default);

            var journeys = result.Data.ToList();

            Assert.AreEqual(1, journeys.Count);
            var journey = journeys.First();
            
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
        public async Task HandleGetAllJourneysQuery_ShouldGetVoidedJourneys_WhenGettingVoided()
        {
            var result = await _dut.Handle(new GetAllJourneysQuery(true), default);

            var journeys = result.Data.ToList();

            Assert.AreEqual(2, journeys.Count);
            var journey = journeys.First();
            
            var steps = journey.Steps.ToList();
            Assert.AreEqual(2, steps.Count);
        }
    }
}
