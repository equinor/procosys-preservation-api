using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeTests
    {
        private const string TestPlant = "PlantA";
        private RequirementType _dut;
        private RequirementDefinition _rd;

        [TestInitialize]
        public void Setup()
        {
            _dut = new RequirementType(TestPlant, "CodeA", "TitleA", RequirementTypeIcon.Other, 10);
            _rd = new RequirementDefinition(TestPlant, "RD1", 4, RequirementUsage.ForAll, 0);
            _dut.AddRequirementDefinition(_rd);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("CodeA", _dut.Code);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual(RequirementTypeIcon.Other, _dut.Icon);
            Assert.AreEqual(10, _dut.SortKey);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(1, _dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldThrowExceptionTest_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddRequirementDefinition(null));

        [TestMethod]
        public void AddRequirementDefinition_ShouldAddRequirementDefinitionToRequirementDefinitionsList()
        {
            var rd = new RequirementDefinition(TestPlant, "RD2", 4, RequirementUsage.ForAll, 0);

            _dut.AddRequirementDefinition(rd);

            Assert.AreEqual(2, _dut.RequirementDefinitions.Count);
            Assert.IsTrue(_dut.RequirementDefinitions.Contains(rd));
        }

        [TestMethod]
        public void RequirementDefinitions_ShouldThrowExceptionTest_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveRequirementDefinition(null));

        [TestMethod]
        public void RequirementDefinitions_ShouldThrowExceptionTest_WhenRequirementDefinitionIsNotVoided()
            => Assert.ThrowsException<Exception>(() => _dut.RemoveRequirementDefinition(_rd));

        [TestMethod]
        public void RemoveRequirementDefinition_ShouldRemoveRequirementDefinitionFromRequirementDefinitionsList()
        {
            _rd.IsVoided = true;
            _dut.RemoveRequirementDefinition(_rd);

            Assert.AreEqual(0, _dut.RequirementDefinitions.Count);
        }
    }
}
