using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PathToProjectAttributeTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new ProjectAccessCheckAttribute(PathToProjectType.ProjectName, "P");

            Assert.AreEqual(PathToProjectType.ProjectName, dut.PathToProjectType);
            Assert.AreEqual("P", dut.PropertyName);
        }
    }
}
