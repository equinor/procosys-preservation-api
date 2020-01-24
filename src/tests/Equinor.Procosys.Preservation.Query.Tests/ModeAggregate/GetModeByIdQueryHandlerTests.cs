using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.ModeAggregate
{
    [TestClass]
    public class GetModeByIdQueryHandlerTests
    {
        private const int ModeId = 72;
        private Mock<IModeRepository> _modeRepoMock;

        private GetModeByIdQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(m => m.Id).Returns(ModeId);

            _modeRepoMock = new Mock<IModeRepository>();
            _modeRepoMock.Setup(r => r.GetByIdAsync(ModeId))
                .Returns(Task.FromResult(modeMock.Object));
            
            _dut = new GetModeByIdQueryHandler(_modeRepoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetModeByIdQueryHandler_KnownId_ShouldReturnMode()
        {
            var result = await _dut.Handle(new GetModeByIdQuery(ModeId), default);

            var mode = result.Data;
            
            Assert.AreEqual(ModeId, mode.Id);
        }

        [TestMethod]
        public async Task HandleGetModeByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            var result = await _dut.Handle(new GetModeByIdQuery(1235), default);

            Assert.IsNull(result.Data);
        }
    }
}
