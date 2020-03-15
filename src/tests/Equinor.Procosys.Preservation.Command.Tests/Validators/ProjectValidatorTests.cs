using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddPerson(context, _currentUserOid, "Ole", "Lukkøye");

                var step = AddJourneyWithStep(context, "J", "S", AddMode(context, "M"), AddResponsible(context, "R")).Steps.First();
                var notClosedProject = AddProject(context, ProjectNameNotClosed, "Project description");
                var closedProject = AddProject(context, ProjectNameClosed, "Project description", true);

                var rd = AddRequirementTypeWith1DefWithoutField(context, "T", "D").RequirementDefinitions.First();

                var req = new Requirement(TestPlant, 2, rd);
                var t1 = AddTag(context, notClosedProject, TagType.Standard, "T1", "Tag description", step, new List<Requirement>{ req });
                _tagInNotClosedProjectId = t1.Id;
                var t2 = AddTag(context, closedProject, TagType.Standard, "T2", "Tag description", step, new List<Requirement>{ req });
                _tagInClosedProjectId = t2.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownName_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync(ProjectNameNotClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownName_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownClosed_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownNotClosed_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameNotClosed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_UnknownName_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tagInNotClosedProjectId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tagInClosedProjectId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(0, default);
                Assert.IsFalse(result);
            }
        }
    }
}
