using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagAggregate
{
    [TestClass]
    public class TagDtoTests
    {
        private TagDto _dut;

        [TestInitialize]
        public void Setup() => _dut = new TagDto(
                1,
                "AreaCode",
                "CallOffNo",
                "CommPkgNo",
                "DisciplineCode",
                true,
                true,
                "McPkgNo",
                "ProjectName",
                "PoNo",
                new List<RequirementDto> { new RequirementDto(1, 2, true, 3) },
                PreservationStatus.Active,
                2,
                "TagFunctionCode",
                "TagNo");

        [TestMethod]
        public void Constructor_SetsProperties()
        {
            Assert.AreEqual(1, _dut.Id);
            Assert.AreEqual("AreaCode", _dut.AreaCode);
            Assert.AreEqual("CallOffNo", _dut.CalloffNo);
            Assert.AreEqual("CommPkgNo", _dut.CommPkgNo);
            Assert.AreEqual("DisciplineCode", _dut.DisciplineCode);
            Assert.IsTrue(_dut.IsAreaTag);
            Assert.IsTrue(_dut.IsVoided);
            Assert.AreEqual("McPkgNo", _dut.McPkgNo);
            Assert.AreEqual("ProjectName", _dut.ProjectName);
            Assert.AreEqual("PoNo", _dut.PurchaseOrderNo);
            Assert.IsNotNull(_dut.Requirements);
            Assert.AreEqual(1, _dut.Requirements.Count());
            Assert.AreEqual(PreservationStatus.Active, _dut.Status);
            Assert.AreEqual(2, _dut.StepId);
            Assert.AreEqual("TagFunctionCode", _dut.TagFunctionCode);
            Assert.AreEqual("TagNo", _dut.TagNo);
        }
    }
}
