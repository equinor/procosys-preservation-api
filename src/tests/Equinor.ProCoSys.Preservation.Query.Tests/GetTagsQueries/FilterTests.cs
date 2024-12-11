using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries
{
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new Filter();
            Assert.IsNull(dut.CallOffStartsWith);
            Assert.IsNull(dut.CommPkgNoStartsWith);
            Assert.IsNull(dut.McPkgNoStartsWith);
            Assert.IsNull(dut.PurchaseOrderNoStartsWith);
            Assert.IsNull(dut.TagNoStartsWith);
            Assert.AreEqual(0, dut.PreservationStatus.Count);
            Assert.IsFalse(dut.ActionStatus.HasValue);
            Assert.AreEqual(0, dut.AreaCodes.Count);
            Assert.AreEqual(0, dut.DisciplineCodes.Count);
            Assert.AreEqual(0, dut.DueFilters.Count);
            Assert.AreEqual(0, dut.JourneyIds.Count);
            Assert.AreEqual(0, dut.ModeIds.Count);
            Assert.AreEqual(0, dut.RequirementTypeIds.Count);
            Assert.AreEqual(0, dut.ResponsibleIds.Count);
            Assert.AreEqual(0, dut.StepIds.Count);
            Assert.AreEqual(0, dut.TagFunctionCodes.Count);
        }
    }
}
