using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementDefTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementDefinition("SchemaA", "TitleA", 4, 10);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(4, dut.DefaultIntervalWeeks);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsFalse(dut.IsVoided);
            Assert.AreEqual(0, dut.Fields.Count);
        }

        [TestMethod]
        public void AddField_ShouldThrowExceptionTest_ForNullField()
        {
            var dut = new RequirementDefinition("", "", 0, 0);

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddField(null));
            Assert.AreEqual(0, dut.Fields.Count);
        }

        [TestMethod]
        public void AddField_ShouldAddFieldToFieldsList()
        {
            var f = new Mock<Field>();

            var dut = new RequirementDefinition("", "", 0, 0);
            dut.AddField(f.Object);

            Assert.AreEqual(1, dut.Fields.Count);
            Assert.IsTrue(dut.Fields.Contains(f.Object));
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new RequirementDefinition("", "", 0, 0);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}
