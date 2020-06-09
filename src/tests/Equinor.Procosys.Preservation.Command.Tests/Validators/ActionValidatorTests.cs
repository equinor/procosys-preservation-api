using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ActionValidatorTests : ReadOnlyTestsBase
    {
        private int _actionId;
        private readonly string _filename = "fil.txt";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var project = AddProject(context, "P", "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D").RequirementDefinitions.First();

                var tag = AddTag(context, project, TagType.Standard, "TagNo", "Tag description", journey.Steps.First(),
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, rd)});

                var action = new Action(TestPlant, "A", "D", null);
                tag.AddAction(action);

                var attachment = new ActionAttachment(TestPlant, Guid.Empty, _filename);
                action.AddAttachment(attachment);

                context.SaveChangesAsync().Wait();

                _actionId = action.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownIds_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.ExistsAsync(_actionId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownActionId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.ExistsAsync(123456, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_KnownId_ReturnsFalse_WhenNotClosed()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(_actionId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_KnownId_ReturnsTrue_WhenClosed()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var action = context.Actions.Single(a => a.Id == _actionId);
                action.Close(DateTime.UtcNow, context.Persons.Single(p => p.Oid == _currentUserOid));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(_actionId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsClosedAsync_UnknownActionId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.IsClosedAsync(123456, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_KnownFile_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_actionId, _filename, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownFile_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_actionId, "X", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownActionId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new ActionValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(123456, _filename, default);
                Assert.IsFalse(result);
            }
        }
    }
}
