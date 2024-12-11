using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTags
{
    [TestClass]
    public class GetTagsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var sorting = new Sorting(SortingDirection.Asc, SortingProperty.Area);
            var filter = new Filter();
            var paging = new Paging(0,10);
            var dut = new GetTagsQuery("PX", sorting, filter, paging);

            Assert.AreEqual("PX", dut.ProjectName);
            Assert.AreEqual(sorting, dut.Sorting);
            Assert.AreEqual(filter, dut.Filter);
            Assert.AreEqual(paging, dut.Paging);
        }

        [TestMethod]
        public void Constructor_SetsCorrectDefaultValues()
        {
            var dut = new GetTagsQuery("PX");

            Assert.AreEqual("PX", dut.ProjectName);
            Assert.IsNotNull(dut.Sorting);
            Assert.AreEqual(GetTagsQuery.DefaultSortingDirection, dut.Sorting.Direction);
            Assert.AreEqual(GetTagsQuery.DefaultSortingProperty, dut.Sorting.Property);

            Assert.IsNotNull(dut.Filter);
            Assert.AreEqual(0, dut.Filter.PreservationStatus.Count);
            Assert.IsFalse(dut.Filter.ActionStatus.HasValue);
            Assert.IsNull(dut.Filter.CallOffStartsWith);
            Assert.IsNull(dut.Filter.CommPkgNoStartsWith);
            Assert.IsNull(dut.Filter.McPkgNoStartsWith);
            Assert.IsNull(dut.Filter.PurchaseOrderNoStartsWith);
            Assert.IsNull(dut.Filter.TagNoStartsWith);
            Assert.IsNull(dut.Filter.StorageAreaStartsWith);
            Assert.IsNotNull(dut.Filter.DueFilters);
            Assert.AreEqual(0, dut.Filter.DueFilters.Count());
            Assert.IsNotNull(dut.Filter.AreaCodes);
            Assert.AreEqual(0, dut.Filter.AreaCodes.Count());
            Assert.IsNotNull(dut.Filter.DisciplineCodes);
            Assert.AreEqual(0, dut.Filter.DisciplineCodes.Count());
            Assert.IsNotNull(dut.Filter.JourneyIds);
            Assert.AreEqual(0, dut.Filter.JourneyIds.Count());
            Assert.IsNotNull(dut.Filter.ModeIds);
            Assert.AreEqual(0, dut.Filter.ModeIds.Count());
            Assert.IsNotNull(dut.Filter.RequirementTypeIds);
            Assert.AreEqual(0, dut.Filter.RequirementTypeIds.Count());
            Assert.IsNotNull(dut.Filter.ResponsibleIds);
            Assert.AreEqual(0, dut.Filter.ResponsibleIds.Count());
            Assert.IsNotNull(dut.Filter.StepIds);
            Assert.AreEqual(0, dut.Filter.StepIds.Count());
            Assert.IsNotNull(dut.Filter.TagFunctionCodes);
            Assert.AreEqual(0, dut.Filter.TagFunctionCodes.Count());
            
            Assert.IsNotNull(dut.Paging);
            Assert.AreEqual(GetTagsQuery.DefaultPage, dut.Paging.Page);
            Assert.AreEqual(GetTagsQuery.DefaultPagingSize, dut.Paging.Size);
        }
        
        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => new GetTagsQuery(""));

    }
}
