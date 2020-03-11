using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTags
{
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new Filter("PX", 
                new List<DueFilterType>{DueFilterType.ThisWeek},
                PreservationStatus.NotStarted,
                new List<int>{1},
                new List<string>{"DI"},
                new List<int>{3},
                new List<string>{"TF"},
                new List<int>{5},
                new List<int>{6},
                new List<int>{7},
                "TAG", 
                "COMM",
                "MC",
                "PO",
                "CO");
            Assert.AreEqual("PX", dut.ProjectName);
            Assert.AreEqual("CO", dut.CallOffStartsWith);
            Assert.AreEqual("COMM", dut.CommPkgNoStartsWith);
            Assert.AreEqual("MC", dut.McPkgNoStartsWith);
            Assert.AreEqual("PO", dut.PurchaseOrderNoStartsWith);
            Assert.AreEqual("TAG", dut.TagNoStartsWith);
            Assert.IsTrue(dut.PreservationStatus.HasValue);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.PreservationStatus.Value);
            Assert.AreEqual("DI", dut.DisciplineCodes.Single());
            Assert.AreEqual(DueFilterType.ThisWeek, dut.DueFilters.Single());
            Assert.AreEqual(6, dut.JourneyIds.Single());
            Assert.AreEqual(5, dut.ModeIds.Single());
            Assert.AreEqual(1, dut.RequirementTypeIds.Single());
            Assert.AreEqual(3, dut.ResponsibleIds.Single());
            Assert.AreEqual(7, dut.StepIds.Single());
            Assert.AreEqual("TF", dut.TagFunctionCodes.Single());
            Assert.AreEqual("DI", dut.DisciplineCodes.Single());
        }

        [TestMethod]
        public void Constructor_SetsProperties_WhenNullValues()
        {
            var dut = new Filter("PX", null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            Assert.AreEqual("PX", dut.ProjectName);
            Assert.IsNull(dut.CallOffStartsWith);
            Assert.IsNull(dut.CommPkgNoStartsWith);
            Assert.IsNull(dut.McPkgNoStartsWith);
            Assert.IsNull(dut.PurchaseOrderNoStartsWith);
            Assert.IsNull(dut.TagNoStartsWith);
            Assert.IsFalse(dut.PreservationStatus.HasValue);
            Assert.AreEqual(0, dut.DisciplineCodes.Count());
            Assert.AreEqual(0, dut.DueFilters.Count());
            Assert.AreEqual(0, dut.JourneyIds.Count());
            Assert.AreEqual(0, dut.ModeIds.Count());
            Assert.AreEqual(0, dut.RequirementTypeIds.Count());
            Assert.AreEqual(0, dut.ResponsibleIds.Count());
            Assert.AreEqual(0, dut.StepIds.Count());
            Assert.AreEqual(0, dut.TagFunctionCodes.Count());
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProjectNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Filter(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)
            );
    }
}
