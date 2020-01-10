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
        public void ConstructorSetsPropertiesTest()
        {
            var rt = new RequirementType("SchemaA", "CodeA", "TitleA", 10);

            Assert.AreEqual("SchemaA", rt.Schema);
            Assert.AreEqual("CodeA", rt.Code);
            Assert.AreEqual("TitleA", rt.Title);
            Assert.AreEqual(10, rt.SortKey);
            Assert.IsFalse(rt.IsVoided);
            Assert.AreEqual(0, rt.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddingEmptyRequirementDefinitionThrowsExceptionTest()
        {
            var rt = new RequirementType("", "", "", 0);

            Assert.ThrowsException<ArgumentNullException>(() => rt.AddRequirementDefinition(null));
            Assert.AreEqual(0, rt.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void RequirementDefinitionIsAddedToRequirementDefinitionsListTest()
        {
            var rt = new RequirementType("", "", "", 0);
            var rd = new Mock<RequirementDefinition>();

            rt.AddRequirementDefinition(rd.Object);

            Assert.AreEqual(1, rt.RequirementDefinitions.Count);
            Assert.IsTrue(rt.RequirementDefinitions.Contains(rd.Object));
        }
    }
}
