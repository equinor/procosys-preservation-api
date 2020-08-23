using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ModeAggregate
{
    [TestClass]
    public class GetAllModesQueryHandlerTests : ReadOnlyTestsBase
    {
        private readonly string _mode1Title = "M1";
        private readonly string _mode2Title = "M2";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = AddMode(context, _mode1Title, true);
                mode.IsVoided = true;
                AddMode(context, _mode2Title, false);
                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task HandleGetAllModesQueryHandler_ShouldReturnModes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllModesQueryHandler(context);

                var result = await dut.Handle(new GetAllModesQuery(true), default);

                var modes = result.Data.ToList();

                Assert.AreEqual(2, modes.Count);
                Assert.AreEqual(_mode1Title, modes.First().Title);
                Assert.AreEqual(_mode2Title, modes.Last().Title);
                Assert.IsTrue(modes.First().ForSupplier);
                Assert.IsFalse(modes.Last().ForSupplier);
            }
        }

        [TestMethod]
        public async Task HandleGetAllModesQueryHandler_ShouldNotReturnVoidedModes_WhenExcludedInRequest()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllModesQueryHandler(context);

                var result = await dut.Handle(new GetAllModesQuery(false), default);

                var modes = result.Data.ToList();

                Assert.AreEqual(1, modes.Count);
                Assert.AreEqual(_mode2Title, modes.Single().Title);
            }
        }
    }
}
