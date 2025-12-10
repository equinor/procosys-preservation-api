using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetAllRequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetAllRequirementTypes
{
    [TestClass]
    public class FieldDtoTests
    {
        private const string RowVersion = "AAAAAAAAABA=";

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new FieldDto(1, "LabelA", true, FieldType.CheckBox, 10, "UnitA", true, RowVersion);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("LabelA", dut.Label);
            Assert.AreEqual("UnitA", dut.Unit);
            Assert.AreEqual(FieldType.CheckBox, dut.FieldType);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsTrue(dut.ShowPrevious.HasValue);
            Assert.IsTrue(dut.ShowPrevious.Value);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(RowVersion, dut.RowVersion);
        }
    }
}
