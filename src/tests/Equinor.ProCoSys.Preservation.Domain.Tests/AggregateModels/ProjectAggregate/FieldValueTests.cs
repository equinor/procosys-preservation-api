using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class FieldValueTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var fieldMock = new Mock<Field>();
            fieldMock.SetupGet(f => f.Id).Returns(54);
            var field = new TestClass("PlantA", fieldMock.Object);

            Assert.AreEqual("PlantA", field.Plant);
            Assert.AreEqual(54, field.FieldId);
        }

        private class TestClass : FieldValue
        {
            public TestClass(string schema, Field field) : base(schema, field)
            {
            }
        }
    }
}
