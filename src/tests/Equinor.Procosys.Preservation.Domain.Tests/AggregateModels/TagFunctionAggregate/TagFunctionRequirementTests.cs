using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagFunctionAggregate
{
    [TestClass]
    public class TagFunctionRequirementTests
    {
        private const string TestPlant = "PlantA";
        private TagFunctionRequirement _dut;
        private Mock<RequirementDefinition> _requirementDefinitionMock;

        [TestInitialize]
        public void Setup()
        {
            _requirementDefinitionMock = new Mock<RequirementDefinition>();
            _requirementDefinitionMock.SetupGet(r => r.Schema).Returns(TestPlant);
            _requirementDefinitionMock.SetupGet(r => r.Id).Returns(5);
            _dut = new TagFunctionRequirement(TestPlant, 4, _requirementDefinitionMock.Object);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Schema);
            Assert.AreEqual(4, _dut.IntervalWeeks);
            Assert.AreEqual(5, _dut.RequirementDefinitionId);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new TagFunctionRequirement(TestPlant, 4, null));

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            Assert.IsFalse(_dut.IsVoided);

            _dut.Void();
            Assert.IsTrue(_dut.IsVoided);

            _dut.UnVoid();
            Assert.IsFalse(_dut.IsVoided);
        }
    }
}
