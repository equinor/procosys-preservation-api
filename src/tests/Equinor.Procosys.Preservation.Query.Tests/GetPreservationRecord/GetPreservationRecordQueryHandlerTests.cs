using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetPreservationRecord;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests.GetPreservationRecord
{
    [TestClass]
    public class GetGetPreservationRecordQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _requirementWithoutFieldId;
        private Guid _preservationRecordGuid;
        private int _preservationRecordId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var requirementDefinitionWithoutField =
                    AddRequirementTypeWith1DefWithoutField(context, "RT", "", RequirementTypeIcon.Other, 1).RequirementDefinitions.Single();

                var requirementWithoutField = new TagRequirement(TestPlant, 1, requirementDefinitionWithoutField);

                var tag = new Tag(TestPlant,
                    TagType.Standard, 
                    "TagNo",
                    "Description",
                    journey.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        requirementWithoutField
                    });
                context.Tags.Add(tag);
                
                Assert.IsNull(requirementWithoutField.ActivePeriod);
                // All TagRequirements get an active Period when starting
                tag.StartPreservation();
                Assert.IsNotNull(requirementWithoutField.ActivePeriod);
                
                context.SaveChangesAsync().Wait();
                _tagId = tag.Id;
                _requirementWithoutFieldId = requirementWithoutField.Id;

                var activePeriodForRequirementWithOutField =
                    tag.Requirements.Single(r => r.Id == requirementWithoutField.Id).ActivePeriod;

                Assert.IsNull(activePeriodForRequirementWithOutField.PreservationRecord);
                // Active Period gets a Preservation Record and the current Active Period be a new active Period when preserving
                tag.Preserve(new Mock<Person>().Object, _requirementWithoutFieldId);
                Assert.IsNotNull(activePeriodForRequirementWithOutField.PreservationRecord);

                context.SaveChangesAsync().Wait();
                _preservationRecordGuid = activePeriodForRequirementWithOutField.PreservationRecord.ObjectGuid;
                _preservationRecordId = activePeriodForRequirementWithOutField.PreservationRecord.Id;
            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_ShouldSucceed_WhenKnownRecord()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetPreservationRecordQuery(_tagId, _requirementWithoutFieldId, _preservationRecordGuid);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Data.Id, _preservationRecordId);
            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_ShouldFail_WhenTagNotExist()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var tagId = 11;
                var query = new GetPreservationRecordQuery(tagId, _requirementWithoutFieldId, _preservationRecordGuid);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.AreEqual(1, result.Errors.Count);
                Assert.IsTrue(result.Errors[0].StartsWith(Strings.EntityNotFound(nameof(Tag), tagId)));
            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_ShouldFail_WhenTagRequirementNotExist()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var reqId = 22;
                var query = new GetPreservationRecordQuery(_tagId, reqId, _preservationRecordGuid);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.AreEqual(1, result.Errors.Count);
                Assert.IsTrue(result.Errors[0].StartsWith(Strings.EntityNotFound(nameof(TagRequirement), reqId)));
            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_ShouldFail_WhenPreservationRecordNotExist()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var presevRecordGuid = new Guid();
                var query = new GetPreservationRecordQuery(_tagId, _requirementWithoutFieldId, presevRecordGuid);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.AreEqual(1, result.Errors.Count);
                Assert.IsTrue(result.Errors[0].StartsWith($"{nameof(PreservationPeriod)} not found"));
            }
        }
    }
}
