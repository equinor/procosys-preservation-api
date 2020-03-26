using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetUniqueTagRequirementTypes
{
    [TestClass]
    public class GetUniqueTagRequirementTypesQueryHandlerTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;
        private GetUniqueTagRequirementTypesQuery _queryForProject1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _queryForProject1 = new GetUniqueTagRequirementTypesQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectUniqueRequirementTypes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);
                
                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType1.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType2.Code));

                result = await dut.Handle(new GetUniqueTagRequirementTypesQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType1.Code));
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnEmptyListOfUniqueRequirementTypes()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);

                var result = await dut.Handle(new GetUniqueTagRequirementTypesQuery("Unknown"), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
