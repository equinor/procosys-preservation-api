using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class TagHelperTests : ReadOnlyTestsBase
    {
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);
            }
        }

        [TestMethod]
        public async Task GetProjectName_KnownTagId_ShouldReturnProjectName()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Arrange
                var dut = new TagHelper(context);

                // Act
                var projectName = await dut.GetProjectNameAsync(_testDataSet.Project1.Tags.First().Id);

                // Assert
                Assert.AreEqual(_testDataSet.Project1.Name, projectName);
            }
        }

        [TestMethod]
        public async Task GetProjectName_UnKnownTagId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Arrange
                var dut = new TagHelper(context);

                // Act
                var projectName = await dut.GetProjectNameAsync(0);

                // Assert
                Assert.IsNull(projectName);
            }
        }

        [TestMethod]
        public async Task GetResponsibleCode_KnownTagId_ShouldReturnResponsibleCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Arrange
                var dut = new TagHelper(context);

                // Act
                var responsibleCode = await dut.GetResponsibleCodeAsync(_testDataSet.Project1.Tags.First().Id);

                // Assert
                Assert.AreEqual(_testDataSet.Responsible1.Code, responsibleCode);
            }
        }

        [TestMethod]
        public async Task GetResponsibleCode_UnKnownTagId_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Arrange
                var dut = new TagHelper(context);

                // Act
                var responsibleCode = await dut.GetResponsibleCodeAsync(0);

                // Assert
                Assert.IsNull(responsibleCode);
            }
        }
    }
}
