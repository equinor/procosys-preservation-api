using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ResponsibleAggregate
{
    [TestClass]
    public class GetAllResponsiblesQueryHandlerTests : ReadOnlyTestsBase
    {
        private Responsible _responsible;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                _responsible = AddResponsible(context, "R");
            }
        }

        [TestMethod]
        public async Task HandleGetAllResponsiblesQueryHandler_ShouldReturnResponsibles()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var _dut = new GetAllResponsiblesQueryHandler(context);
                var result = await _dut.Handle(new GetAllResponsiblesQuery(), default);

                var responsibles = result.Data.ToList();

                Assert.AreEqual(1, responsibles.Count);
                var responsibleDto = responsibles.Single();
                Assert.AreEqual(_responsible.Code, responsibleDto.Code);
                Assert.AreEqual(_responsible.Title, responsibleDto.Title);
            }
        }
    }
}
