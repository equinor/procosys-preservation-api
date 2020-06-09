using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagAreas;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagAreas
{
    [TestClass]
    public class GetUniqueTagAreasQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagAreasQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagAreasQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagAreasQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagAreasQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagAreasQuery_ShouldReturnCorrectUniqueAreas()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagAreasQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(10, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == "AREA-3"));
                Assert.IsTrue(result.Data.Any(rt => rt.Description == "AREA-3-Description"));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagAreasQuery_ShouldReturnEmptyListOfUniqueAreas()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagAreasQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagAreasQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(0, result.Data.Count);

                result = await dut.Handle(new GetUniqueTagAreasQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
