using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeTests
    {
        private const string TestPlant = "PlantA";
        private readonly RequirementType _dut = new RequirementType(TestPlant, "CodeA", "TitleA", RequirementTypeIcon.Other, 10);

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("CodeA", _dut.Code);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual(RequirementTypeIcon.Other, _dut.Icon);
            Assert.AreEqual(10, _dut.SortKey);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldThrowExceptionTest_ForNullRequirementDefinition()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _dut.AddRequirementDefinition(null));
            Assert.AreEqual(0, _dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldAddRequirementDefinitionToRequirementDefinitionsList()
        {
            var rd = new RequirementDefinition(TestPlant, "", 4, RequirementUsage.ForAll, 0);

            _dut.AddRequirementDefinition(rd);

            Assert.AreEqual(1, _dut.RequirementDefinitions.Count);
            Assert.IsTrue(_dut.RequirementDefinitions.Contains(rd));
        }
 
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
