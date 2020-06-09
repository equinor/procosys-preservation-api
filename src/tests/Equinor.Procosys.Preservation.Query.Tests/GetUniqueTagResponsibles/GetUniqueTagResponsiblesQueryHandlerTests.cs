using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagResponsibles
{
    [TestClass]
    public class GetUniqueTagResponsiblesQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagResponsiblesQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagResponsiblesQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagResponsiblesQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagResponsiblesQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagResponsiblesQuery_ShouldReturnCorrectUniqueResponsibleCodes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagResponsiblesQueryHandler(context);
                
                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.Responsible1.Code));

                result = await dut.Handle(new GetUniqueTagResponsiblesQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.Responsible1.Code));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagResponsiblesQuery_ShouldReturnEmptyListOfUniqueResponsibleCodes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagResponsiblesQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagResponsiblesQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagResponsiblesQuery_ShouldReturnCorrectUniqueResponsibleCodes_AfterTransfer()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {

                var tag = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).First();
                tag.StartPreservation();
                tag.Transfer(_testDataSet.Journey1With2Steps);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagResponsiblesQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.Responsible1.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.Responsible2.Code));
            }
        }
    }
}
