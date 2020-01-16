using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementType("SchemaA", "CodeA", "TitleA", 10);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsFalse(dut.IsVoided);
            Assert.AreEqual(0, dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldThrowExceptionTest_ForNullRequirementDefinition()
        {
            var dut = new RequirementType("", "", "", 0);

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddRequirementDefinition(null));
            Assert.AreEqual(0, dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldAddRequirementDefinitionToRequirementDefinitionsList()
        {
            var dut = new RequirementType("", "", "", 0);
            var rd = new Mock<RequirementDefinition>();

            dut.AddRequirementDefinition(rd.Object);

            Assert.AreEqual(1, dut.RequirementDefinitions.Count);
            Assert.IsTrue(dut.RequirementDefinitions.Contains(rd.Object));
        }
 
        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new RequirementType("", "", "", 0);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}
