using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
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
            
            var dut = new NumberValue(TestPlant, _fieldMock.Object, null);

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual(FieldId, dut.FieldId);
            Assert.IsFalse(dut.Value.HasValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenIntValue()
        {
            var intValue = 141;
            var dut = new NumberValue(TestPlant, _fieldMock.Object, intValue);

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(intValue, dut.Value.Value);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenDoubleValue()
        {
            var doubleValue = 1282.91;
            var dut = new NumberValue(TestPlant, _fieldMock.Object, doubleValue);

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.IsTrue(dut.Value.HasValue);
            Assert.AreEqual(doubleValue, dut.Value.Value);
        }
    }
}
