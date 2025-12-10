using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.ModeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.ModeAggregate
{
    [TestClass]
    public class GetModeByIdQueryHandlerTests : ReadOnlyTestsBase
    {
        private Mode _mode;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _mode = AddMode(context, "M", false);
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
