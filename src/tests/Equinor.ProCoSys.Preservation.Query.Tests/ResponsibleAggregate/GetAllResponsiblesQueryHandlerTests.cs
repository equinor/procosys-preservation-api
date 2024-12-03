using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.ResponsibleAggregate
{
    [TestClass]
    public class GetAllResponsiblesQueryHandlerTests : ReadOnlyTestsBase
    {
        private Responsible _responsible;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _responsible = AddResponsible(context, "R");
            }
        }

        [TestMethod]
        public async Task HandleGetAllResponsiblesQueryHandler_ShouldReturnResponsibles()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var _dut = new GetAllResponsiblesQueryHandler(context);
                var result = await _dut.Handle(new GetAllResponsiblesQuery(), default);

                var responsibles = result.Data.ToList();

                Assert.AreEqual(1, responsibles.Count);
                var responsibleDto = responsibles.Single();
                Assert.AreEqual(_responsible.Code, responsibleDto.Code);
                Assert.AreEqual(_responsible.Description, responsibleDto.Description);
            }
        }
    }
}
