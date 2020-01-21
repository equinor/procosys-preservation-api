using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                "ProjectNo",
                "PoNo",
                new List<RequirementDto> { new RequirementDto(1, 2, true, 3) },
                PreservationStatus.Active,
                2,
                "TagFunctionCode",
                "TagNo");

        [TestMethod]
        public void Constructor_setsProperties()
        {
            Assert.AreEqual(1, _dut.Id);
            Assert.AreEqual("AreaCode", _dut.AreaCode);
            Assert.AreEqual("CallOffNo", _dut.CalloffNumber);
            Assert.AreEqual("CommPkgNo", _dut.CommPkgNumber);
            Assert.AreEqual("DisciplineCode", _dut.DisciplineCode);
            Assert.IsTrue(_dut.IsAreaTag);
            Assert.IsTrue(_dut.IsVoided);
            Assert.AreEqual("McPkgNo", _dut.McPkgNumber);
            Assert.AreEqual("ProjectNo", _dut.ProjectNumber);
            Assert.AreEqual("PoNo", _dut.PurchaseOrderNumber);
            Assert.IsNotNull(_dut.Requirements);
            Assert.AreEqual(1, _dut.Requirements.Count());
            Assert.AreEqual(PreservationStatus.Active, _dut.Status);
            Assert.AreEqual(2, _dut.StepId);
            Assert.AreEqual("TagFunctionCode", _dut.TagFunctionCode);
            Assert.AreEqual("TagNo", _dut.TagNo);
        }
    }
}
