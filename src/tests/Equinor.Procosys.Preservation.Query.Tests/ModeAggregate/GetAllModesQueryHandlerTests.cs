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
                AddMode(context, _mode1Title);
                AddMode(context, _mode2Title);
            }
        }

        [TestMethod]
        public async Task HandleGetAllModesQueryHandler_ShouldReturnModes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllModesQueryHandler(context);

                var result = await dut.Handle(new GetAllModesQuery(TestPlant), default);

                var modes = result.Data.ToList();

                Assert.AreEqual(2, modes.Count);
                Assert.AreEqual(_mode1Title, modes.First().Title);
                Assert.AreEqual(_mode2Title, modes.Last().Title);
            }
        }
    }
}
