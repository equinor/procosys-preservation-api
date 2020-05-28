using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetMode;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetMode
{
    [TestClass]
    public class GetModeByIdQueryHandlerTests : ReadOnlyTestsBase
    {
        private Mode _mode;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _mode = AddMode(context, "M");
            }
        }

        [TestMethod]
        public async Task HandleGetModeByIdQueryHandler_KnownId_ShouldReturnMode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetModeByIdQueryHandler(context);
                var result = await dut.Handle(new GetModeByIdQuery(_mode.Id), default);

                var mode = result.Data;
            
                Assert.AreEqual(_mode.Id, mode.Id);
                Assert.AreEqual(_mode.Title, mode.Title);
            }
        }

        [TestMethod]
        public async Task HandleGetModeByIdQueryHandler_UnknownId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetModeByIdQueryHandler(context);
                var result = await dut.Handle(new GetModeByIdQuery(1235), default);
                Assert.IsNull(result.Data);
            }
        }
    }
}
