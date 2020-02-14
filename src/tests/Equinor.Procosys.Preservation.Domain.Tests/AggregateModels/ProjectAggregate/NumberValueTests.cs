using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class NumberValueTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenNullValue()
        {
            var fieldMock = new Mock<Field>();
            fieldMock.SetupGet(f => f.Id).Returns(54);
            
            var dut = new NumberValue("SchemaA", fieldMock.Object, "N/A");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(54, dut.FieldId);
            Assert.IsFalse(dut.Value.HasValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenIntValue()
        {
            var dut = new NumberValue("SchemaA", new Mock<Field>().Object, "141");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(141, dut.Value.Value);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenDoubleValue()
        {
            var number = 1282.91;
            var numberAsString = number.ToString("F2");
            var dut = new NumberValue("SchemaA", new Mock<Field>().Object, numberAsString);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(number, dut.Value.Value);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenIllegalValue()
            => Assert.ThrowsException<ArgumentException>(() =>
                new NumberValue("SchemaA", new Mock<Field>().Object, "ABC")
            );
    }
}
