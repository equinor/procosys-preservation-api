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
        public void ConstructorSetsPropertiesTest()
        {
            var rd = new RequirementDefinition("SchemaA", "TitleA", 4, 10);

            Assert.AreEqual("SchemaA", rd.Schema);
            Assert.AreEqual("TitleA", rd.Title);
            Assert.AreEqual(4, rd.DefaultInterval);
            Assert.AreEqual(10, rd.SortKey);
            Assert.IsFalse(rd.IsVoided);
            Assert.AreEqual(0, rd.Fields.Count);
        }

        [TestMethod]
        public void AddingEmptyFieldThrowsExceptionTest()
        {
            var rd = new RequirementDefinition("", "", 0, 0);

            Assert.ThrowsException<ArgumentNullException>(() => rd.AddField(null));
            Assert.AreEqual(0, rd.Fields.Count);
        }

        [TestMethod]
        public void FieldIsAddedToFieldsListTest()
        {
            var rd = new RequirementDefinition("", "", 0, 0);
            var f = new Mock<Field>();

            rd.AddField(f.Object);

            Assert.AreEqual(1, rd.Fields.Count);
            Assert.IsTrue(rd.Fields.Contains(f.Object));
        }
    }
}
