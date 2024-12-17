using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagFunctions
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
                
                for (int i = 0; i < 10; i++)
                {
                    var tf = AddTagFunction(context, $"{_testDataSet.TagFunctionPrefix}-{i}", "R");
                    var rt = AddRequirementTypeWith1DefWithoutField(context, _testDataSet.ReqType1.Code, "R", RequirementTypeIcon.Other);
                    tf.AddRequirement(new TagFunctionRequirement(TestPlant, _testDataSet.IntervalWeeks, rt.RequirementDefinitions.First()));
                    context.SaveChangesAsync().Wait();
                }

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
