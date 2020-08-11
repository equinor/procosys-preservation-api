using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetProjectByName;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetProjectDetails
{
    [TestClass]
    public class GetProjectDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private readonly string _name = "Project X";
        private readonly string _description = "Project X - Desc";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddProject(context, _name, _description, true);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsProjectDetails()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetProjectByNameQuery(_name);
                var dut = new GetProjectByNameQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.AreEqual(_name, dto.Name);
                Assert.AreEqual(_description, dto.Description);
                Assert.IsTrue(dto.IsClosed);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfProjectIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetProjectByNameQuery("Unknown");
                var dut = new GetProjectByNameQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
