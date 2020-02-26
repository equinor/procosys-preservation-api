using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ProjectValidatorTests : ReadOnlyTestsBase
    {
        private const string ProjectNameNotClosed = "Project name";
        private const string ProjectNameClosed = "Project name (closed)";
        private int _tagInClosedProjectId;
        private int _tagInNotClosedProjectId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider))
            {
                var notClosedProject = AddProject(context, ProjectNameNotClosed, "Project description");
                var closedProject = AddProject(context, ProjectNameClosed, "Project description", true);
                var t1 = AddTag(context, notClosedProject, "T1", "Tag description", new Mock<Step>().Object, new List<Requirement>{ new Mock<Requirement>().Object});
                _tagInNotClosedProjectId = t1.Id;
                var t2 = AddTag(context, closedProject, "T2", "Tag description", new Mock<Step>().Object, new List<Requirement>{ new Mock<Requirement>().Object});
                _tagInClosedProjectId = t2.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownName_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync(ProjectNameNotClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownName_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownClosed_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownNotClosed_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameNotClosed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_UnknownName_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tagInNotClosedProjectId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tagInClosedProjectId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(0, default);
                Assert.IsFalse(result);
            }
        }
    }
}
