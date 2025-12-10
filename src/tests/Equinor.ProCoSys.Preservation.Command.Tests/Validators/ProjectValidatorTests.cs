using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ProjectValidatorTests : ReadOnlyTestsBase
    {
        private const string ProjectNameNotClosed = "Project name";
        private const string ProjectNameClosed = "Project name (closed)";
        private int _tagInClosedProjectId;
        private int _tag1InNotClosedProjectId;
        private int _tag2InNotClosedProjectId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var step = AddJourneyWithStep(context, "J", "S", AddMode(context, "M", false), AddResponsible(context, "R")).Steps.First();
                var notClosedProject = AddProject(context, ProjectNameNotClosed, "Project description");
                var closedProject = AddProject(context, ProjectNameClosed, "Project description", true);

                var rd = AddRequirementTypeWith1DefWithoutField(context, "T", "D", RequirementTypeIcon.Other).RequirementDefinitions.First();

                var req = new TagRequirement(TestPlant, 2, rd);
                var t1 = AddTag(context, notClosedProject, TagType.Standard, Guid.NewGuid(), "T1", "Tag description", step, new List<TagRequirement> { req });
                _tag1InNotClosedProjectId = t1.Id;
                var t2 = AddTag(context, notClosedProject, TagType.Standard, Guid.NewGuid(), "T2", "Tag description", step, new List<TagRequirement> { req });
                _tag2InNotClosedProjectId = t2.Id;
                var t3 = AddTag(context, closedProject, TagType.Standard, Guid.NewGuid(), "T3", "Tag description", step, new List<TagRequirement> { req });
                _tagInClosedProjectId = t3.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownName_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync(ProjectNameNotClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownName_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.ExistsAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownClosed_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameClosed, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_KnownNotClosed_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync(ProjectNameNotClosed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsExistingAndClosedAsync_UnknownName_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsExistingAndClosedAsync("XX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tag1InNotClosedProjectId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(_tagInClosedProjectId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsClosedForTagAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.IsClosedForTagAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AllTagsInSameProjectAsync_TagsInSameProject_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.AllTagsInSameProjectAsync(new List<int> { _tag1InNotClosedProjectId, _tag2InNotClosedProjectId }, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AllTagsInSameProjectAsync_TagsNotInSameProject_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.AllTagsInSameProjectAsync(new List<int> { _tag1InNotClosedProjectId, _tagInClosedProjectId }, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AllTagsInSameProjectAsync_KnownAndUnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.AllTagsInSameProjectAsync(new List<int> { _tag1InNotClosedProjectId, 0 }, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AllTagsInSameProjectAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ProjectValidator(context);
                var result = await dut.AllTagsInSameProjectAsync(new List<int> { 0 }, default);
                Assert.IsFalse(result);
            }
        }
    }
}
