using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunctionDetails
{
    [TestClass]
    public class GetTagFunctionDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private TagFunction _tf;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _tf = AddTagFunction(context, "TFC", "RC");
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_KnownTagFunction_ShouldReturnTagFunctionWithAllPropertiesSet()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionDetailsQuery(_tf.Code, _tf.RegisterCode), default);

                var tagFunction = result.Data;

                Assert.IsNotNull(tagFunction);
                Assert.AreEqual(_tf.Code, tagFunction.Code);
                Assert.AreEqual(_tf.Description, tagFunction.Description);
                Assert.AreEqual(_tf.RegisterCode, tagFunction.RegisterCode);
            }
        }

        [TestMethod]
        public async Task HandleGetTagFunctionQuery_UnknownTagFunction_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagFunctionsHavingRequirementQueryHandler(context);
                var result = await dut.Handle(new GetTagFunctionDetailsQuery("XX", _tf.RegisterCode), default);
                Assert.IsNull(result.Data);
            }
        }
    }
}
