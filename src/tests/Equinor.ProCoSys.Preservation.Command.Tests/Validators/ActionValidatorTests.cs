using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ActionValidatorTests : ReadOnlyTestsBase
    {
        private int _actionId;
        private readonly string _filename = "fil.txt";
        RequirementTypeIcon _reqIconOther = RequirementTypeIcon.Other;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, "P", "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D", _reqIconOther).RequirementDefinitions.First();

                var tag = AddTag(context, project, TagType.Standard, Guid.NewGuid(), "TagNo", "Tag description", journey.Steps.First(),
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, rd)});

                var action = new Action(Guid.Empty, TestPlant, "A", "D", null);
                tag.AddAction(action);

                var attachment = new ActionAttachment(TestPlant, Guid.Empty, _filename);
                action.AddAttachment(attachment);

                context.SaveChangesAsync().Wait();

                _actionId = action.Id;
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_KnownId_ShouldReturnFalse_WhenNotClosed()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(_actionId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_KnownId_ShouldReturnTrue_WhenClosed()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var action = context.Actions.Single(a => a.Id == _actionId);
                action.Close(DateTime.UtcNow, context.Persons.Single(p => p.Guid == _currentUserOid));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(_actionId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_UnknownActionId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(123456, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_KnownFile_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_actionId, _filename, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownFile_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_actionId, "X", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownActionId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(123456, _filename, default);
                Assert.IsFalse(result);
            }
        }
    }
}
