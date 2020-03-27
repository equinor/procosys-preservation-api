using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagDisciplines
{
    [TestClass]
    public class GetUniqueTagDisciplinesQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagDisciplinesQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagDisciplinesQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagDisciplinesQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectUniqueDisciplines()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagDisciplinesQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(10, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == "DI-3"));
                Assert.IsTrue(result.Data.Any(rt => rt.Description == "DI-3-Description"));
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnEmptyListOfUniqueDisciplines()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetUniqueTagDisciplinesQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagDisciplinesQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(0, result.Data.Count);

                result = await dut.Handle(new GetUniqueTagDisciplinesQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
