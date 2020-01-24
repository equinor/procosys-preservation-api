using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.ModeAggregate
{
    [TestClass]
    public class GetAllModesQueryHandlerTests
    {
        private Mock<IModeRepository> _modeRepoMock;
        private Mode _mode;

        private GetAllModesQueryHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            _mode = new Mode("S", "Mode");

            _modeRepoMock = new Mock<IModeRepository>();
            _modeRepoMock.Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(new List<Mode>
                {
                    _mode,
                    new Mock<Mode>().Object,
                    new Mock<Mode>().Object
                }));
            
            _dut = new GetAllModesQueryHandler(_modeRepoMock.Object);
        }

        [TestMethod]
        public async Task HandleGetAllModesQueryHandler_ShouldReturnModes()
        {
            var result = await _dut.Handle(new GetAllModesQuery(), default);

            var modes = result.Data.ToList();
            
            Assert.AreEqual(3, modes.Count);
            Assert.AreEqual(_mode.Title, modes.First().Title);
        }
    }
}
