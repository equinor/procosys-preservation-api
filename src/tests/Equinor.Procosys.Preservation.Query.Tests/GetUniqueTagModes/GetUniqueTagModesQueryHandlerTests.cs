using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagModes;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagModes
{
    [TestClass]
    public class GetUniqueTagModesQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagModesQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagModesQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagModesQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectUniqueModes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagModesQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Title == _testDataSet.Mode1.Title));

                result = await dut.Handle(new GetUniqueTagModesQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Title == _testDataSet.Mode1.Title));

                result = await dut.Handle(new GetUniqueTagModesQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
