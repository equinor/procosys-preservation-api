using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTags
{
    [TestClass]
    public class TagDtoTests
    {
        private TagDto _dut;

        [TestInitialize]
        public void Setup() => _dut = new TagDto(
            1,
            ActionStatus.HasOverdue,
            "AreaCode",
            "CallOffNo",
            "CommPkgNo",
            "DisciplineCode",
            true,
            true,
            "McPkgNo",
            "Mode",
            "NextMode",
            "NextResp",
            true,
            true,
            true,
            true,
            true,
            true,
            true,
            "PoNo",
            new List<RequirementDto> {new RequirementDto(0, null, RequirementTypeIcon.Other, default, default, false)},
            "Resp",
            "RespDescription",
            PreservationStatus.Active.GetDisplayValue(),
            "SA",
            "TagFunctionCode",
            "TagDesc",
            "TagNo",
            TagType.Standard,
            "AAAAAAAAABA=");

        [TestMethod]
        public void Constructor_SetsProperties()
        {
            Assert.AreEqual(1, _dut.Id);
            Assert.AreEqual(ActionStatus.HasOverdue, _dut.ActionStatus);
            Assert.AreEqual("AreaCode", _dut.AreaCode);
            Assert.AreEqual("CallOffNo", _dut.CalloffNo);
            Assert.AreEqual("CommPkgNo", _dut.CommPkgNo);
            Assert.AreEqual("DisciplineCode", _dut.DisciplineCode);
            Assert.IsTrue(_dut.IsNew);
            Assert.IsTrue(_dut.IsVoided);
            Assert.AreEqual("McPkgNo", _dut.McPkgNo);
            Assert.AreEqual("Mode", _dut.Mode);
            Assert.AreEqual("NextMode", _dut.NextMode);
            Assert.AreEqual("NextResp", _dut.NextResponsibleCode);
            Assert.IsTrue(_dut.ReadyToBePreserved);
            Assert.IsTrue(_dut.ReadyToBeStarted);
            Assert.IsTrue(_dut.ReadyToBeTransferred);
            Assert.IsTrue(_dut.ReadyToBeCompleted);
            Assert.IsTrue(_dut.ReadyToBeRescheduled);
            Assert.IsTrue(_dut.ReadyToBeDuplicated);
            Assert.IsTrue(_dut.ReadyToUndoStarted);
            Assert.AreEqual("TagDesc", _dut.Description);
            Assert.AreEqual("PoNo", _dut.PurchaseOrderNo);
            Assert.IsNotNull(_dut.Requirements);
            Assert.AreEqual(1, _dut.Requirements.Count());
            Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), _dut.Status);
            Assert.AreEqual("SA", _dut.StorageArea);
            Assert.AreEqual("Resp", _dut.ResponsibleCode);
            Assert.AreEqual("RespDescription", _dut.ResponsibleDescription);
            Assert.AreEqual("TagFunctionCode", _dut.TagFunctionCode);
            Assert.AreEqual("TagNo", _dut.TagNo);
            Assert.AreEqual(TagType.Standard, _dut.TagType);
            Assert.AreEqual("AAAAAAAAABA=", _dut.RowVersion);
        }
    }
}
