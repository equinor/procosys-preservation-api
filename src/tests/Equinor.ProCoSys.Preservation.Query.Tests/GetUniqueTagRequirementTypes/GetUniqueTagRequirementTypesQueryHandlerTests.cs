using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetUniqueTagRequirementTypes;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetUniqueTagRequirementTypes
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
        public async Task HandleGetUniqueTagRequirementTypesQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);
                var result = await dut.Handle(_queryForProject1, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagRequirementTypesQuery_ShouldReturnCorrectUniqueRequirementTypes()
        {
            var typeCode = "XC";
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var newReqType = AddRequirementTypeWith1DefWithoutField(context, typeCode, "XT", RequirementTypeIcon.Other);
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _testDataSet.Project1.Tags.First().Id);
                tag.AddRequirement(new TagRequirement(_plantProvider.Plant, 1, newReqType.RequirementDefinitions.Single()));
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);
                
                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(3, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == typeCode));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType1.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Icon == _testDataSet.ReqType1.Icon));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType2.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Icon == _testDataSet.ReqType2.Icon));

                result = await dut.Handle(new GetUniqueTagRequirementTypesQuery(_testDataSet.Project2.Name), default);
                Assert.AreEqual(1, result.Data.Count);
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType1.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Icon == _testDataSet.ReqType1.Icon));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagRequirementTypesQuery_ShouldNotRequirementTypes_ForVoidedTagRequirements()
        {
            var typeCode = "XC";
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var newReqType = AddRequirementTypeWith1DefWithoutField(context, typeCode, "XT", RequirementTypeIcon.Other);
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _testDataSet.Project1.Tags.First().Id);
                var tagRequirement =
                    new TagRequirement(_plantProvider.Plant, 1, newReqType.RequirementDefinitions.Single())
                    {
                        IsVoided = true
                    };
                tag.AddRequirement(tagRequirement);
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetUniqueTagRequirementTypesQueryHandler(context);

                var result = await dut.Handle(_queryForProject1, default);
                Assert.AreEqual(2, result.Data.Count);
                Assert.IsFalse(result.Data.Any(rt => rt.Code == typeCode));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType1.Code));
                Assert.IsTrue(result.Data.Any(rt => rt.Code == _testDataSet.ReqType2.Code));
            }
        }

        [TestMethod]
        public async Task HandleGetUniqueTagRequirementTypesQuery_ShouldReturnEmptyListOfUniqueRequirementTypes()
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
