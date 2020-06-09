using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagJourneys;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagJourneys
{
    [TestClass]
    public class GetUniqueTagJourneysQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagJourneysQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagJourneysQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagJourneysQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagJourneysQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagJourneysQuery_ShouldReturnCorrectUniqueJourneys()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagJourneysQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Title == _testDataSet.Journey1With2Steps.Title));
                Assert.IsTrue(result.Data.Any(rt => rt.Title == _testDataSet.Journey2With1Step.Title));

                result = await dut.Handle(new GetUniqueTagJourneysQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Title == _testDataSet.Journey1With2Steps.Title));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagJourneysQuery_ShouldReturnEmptyListOfUniqueJourneys()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new GetUniqueTagJourneysQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagJourneysQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
