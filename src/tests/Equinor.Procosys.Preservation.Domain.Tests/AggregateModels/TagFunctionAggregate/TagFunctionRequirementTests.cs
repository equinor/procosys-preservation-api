using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.TagFunctionAggregate
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
            _requirementDefinitionMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _requirementDefinitionMock.SetupGet(r => r.Id).Returns(5);
            _dut = new TagFunctionRequirement(TestPlant, 4, _requirementDefinitionMock.Object);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual(4, _dut.IntervalWeeks);
            Assert.AreEqual(5, _dut.RequirementDefinitionId);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() => new TagFunctionRequirement(TestPlant, 4, null));
    }
}
