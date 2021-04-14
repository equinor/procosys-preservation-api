using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class AbstractAreaTagTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                "PO",
                "X");

            Assert.AreEqual(TagType.PreArea, dut.TagType);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("AreaA", dut.AreaCode);
            Assert.AreEqual("PO", dut.PurchaseOrderCalloffCode);
            Assert.AreEqual("X", dut.TagNoSuffix);
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDiscipline()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                null,
                "123",
                null);

            Assert.AreEqual("#PRE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturniteTagNo_WithDiscipline()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                null,
                "123",
                null);

            Assert.AreEqual("#SITE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                null,
                "123",
                "XX");

            Assert.AreEqual("#PRE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturniteTagNo_WithDisciplineAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                null,
                "123",
                "XX");

            Assert.AreEqual("#SITE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndArea()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                "A300",
                "123",
                null);

            Assert.AreEqual("#PRE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturniteTagNo_WithDisciplineAndArea()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                "A300",
                null,
                null);

            Assert.AreEqual("#SITE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPoTagNo_WithDisciplineAndPoCo()
        {
            var dut = new TestAreaTag(
                TagType.PoArea,
                "I",
                "A300",
                "123",
                null);

            Assert.AreEqual("#PO-I-123", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                "A300",
                "123",
                "XX");

            Assert.AreEqual("#PRE-I-A300-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturniteTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                "A300",
                "123",
                "XX");

            Assert.AreEqual("#SITE-I-A300-XX", dut.GetTagNo());
        }
        
        [TestMethod]
        public void GetTagNo_ShouldReturnPoTagNo_WithDisciplineAndPoCoAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.PoArea,
                "I",
                "A300",
                "123",
                "XX");

            Assert.AreEqual("#PO-I-123-XX", dut.GetTagNo());
        }

        public class TestAreaTag : AbstractAreaTag
        {
            public TestAreaTag(TagType tagType, string disciplineCode, string areaCode, string purchaseOrderCalloffCode, string tagNoSuffix)
            {
                TagType = tagType;
                DisciplineCode = disciplineCode;
                AreaCode = areaCode;
                TagNoSuffix = tagNoSuffix;
                PurchaseOrderCalloffCode = purchaseOrderCalloffCode;
            }

            public override TagType TagType { get; }
            public override string DisciplineCode { get; }
            public override string AreaCode { get; }
            public override string PurchaseOrderCalloffCode { get; }
            public override string TagNoSuffix { get; }
        }
    }
}
