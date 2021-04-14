using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class CheckBoxCheckedTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var fieldMock = new Mock<Field>();
            fieldMock.SetupGet(f => f.Id).Returns(54);
            var f = new CheckBoxChecked("PlantA", fieldMock.Object);

            Assert.AreEqual("PlantA", f.Plant);
            Assert.AreEqual(54, f.FieldId);
        }
    }
}
