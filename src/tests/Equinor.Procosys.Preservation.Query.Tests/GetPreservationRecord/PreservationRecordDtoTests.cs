using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.GetPreservationRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetPreservationRecord
{
    [TestClass]
    public class PreservationRecordDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationRecordDto(2, true, "Req Type Title", "Req type code", RequirementTypeIcon.Area, "Req Def Title", 4, "Comment", null);

            Assert.AreEqual(2, dut.Id);
            Assert.IsTrue(dut.BulkPreserved);
            Assert.AreEqual("Req Type Title", dut.RequirementTypeTitle);
            Assert.AreEqual("Req type code", dut.RequirementTypeCode);
            Assert.AreEqual(RequirementTypeIcon.Area, dut.Icon);
            Assert.AreEqual("Req Def Title", dut.RequirementDefinitionTitle);
            Assert.AreEqual(4, dut.IntervalWeeks);
            Assert.AreEqual("Comment", dut.Comment);
        }
    }
}
