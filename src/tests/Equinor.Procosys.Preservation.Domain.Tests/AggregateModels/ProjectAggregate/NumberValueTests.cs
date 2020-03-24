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
        private Mock<Field> _fieldMock;
        private const string TestPlant = "PlantA";
        private const int FieldId = 23;

        [TestInitialize]
        public void Setup()
        {
            _fieldMock = new Mock<Field>();
            _fieldMock.SetupGet(f => f.Id).Returns(FieldId);
            _fieldMock.SetupGet(f => f.Plant).Returns(TestPlant);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenNullValue()
        {
            
            var dut = new NumberValue(TestPlant, _fieldMock.Object, "N/A");

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual(FieldId, dut.FieldId);
            Assert.IsFalse(dut.Value.HasValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenIntValue()
        {
            var dut = new NumberValue(TestPlant, _fieldMock.Object, "141");

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(141, dut.Value.Value);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenDoubleValue()
        {
            var number = 1282.91;
            var numberAsString = number.ToString("F2");
            var dut = new NumberValue(TestPlant, _fieldMock.Object, numberAsString);

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(number, dut.Value.Value);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenIllegalValue()
            => Assert.ThrowsException<ArgumentException>(() =>
                new NumberValue(TestPlant, _fieldMock.Object, "ABC")
            );

        [TestMethod]
        public void IsValidValue_ShouldAcceptNumber()
        {
            var number = 1282.91;
            var valid = NumberValue.IsValidValue(number.ToString("F2"), out var parsedNumber);

            Assert.AreEqual(number, parsedNumber);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void IsValidValue_ShouldAcceptBlank()
        {
            var valid = NumberValue.IsValidValue(string.Empty, out var parsedNumber);

            Assert.IsNull(parsedNumber);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void IsValidValue_ShouldAcceptNull()
        {
            var valid = NumberValue.IsValidValue(null, out var parsedNumber);

            Assert.IsNull(parsedNumber);
            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void IsValidValue_ShouldAcceptNA()
        {
            var valid = NumberValue.IsValidValue("NA", out var parsedNumber);

            Assert.IsNull(parsedNumber);
            Assert.IsTrue(valid);
        }
    }
}
