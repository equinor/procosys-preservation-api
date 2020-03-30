using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagFunctions
{
    [TestClass]
    public class GetUniqueTagFunctionsQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagFunctionsQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagFunctionsQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagFunctionsQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagFunctionsQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagFunctionsQuery_ShouldReturnCorrectUniqueTagFunctions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagFunctionsQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(10, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == "TF-3"));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagFunctionsQuery_ShouldReturnEmptyListOfUniqueTagFunctions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagFunctionsQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagFunctionsQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(0, result.Data.Count);

                result = await dut.Handle(new GetUniqueTagFunctionsQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
