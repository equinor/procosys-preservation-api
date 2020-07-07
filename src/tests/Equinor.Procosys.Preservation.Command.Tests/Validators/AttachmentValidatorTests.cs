using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class AttachmentValidatorTests : ReadOnlyTestsBase
    {
        private int _attachmentId;
                
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, "P", "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D", RequirementTypeIcon.Other).RequirementDefinitions.First();

                var tag = AddTag(context, project, TagType.Standard, "TagNo", "Tag description", journey.Steps.First(),
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, rd)});

                var attachment = new TagAttachment(TestPlant, Guid.Empty, "F");
                tag.AddAttachment(attachment);
                context.SaveChangesAsync().Wait();

                _attachmentId = attachment.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new AttachmentValidator(context);
                var result = await dut.ExistsAsync(_attachmentId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new AttachmentValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}
