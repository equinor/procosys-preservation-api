using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetPreservationRecord;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetPreservationRecord
{
    [TestClass]
    public class GetGetPreservationRecordQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _requirementWithoutFieldId;
        private Guid _preservationRecordGuid;
        private int _preservationRecordId;
        private RequirementType _requirementType;
        private RequirementDefinition _requirementDefinitionWithoutOneField;
        private Field _infoField;
        private int _interval;
        private string _comment = "comment";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "RT", "RD", RequirementTypeIcon.Other, 1);
                _requirementDefinitionWithoutOneField =
                    _requirementType.RequirementDefinitions.Single();
                _infoField = AddInfoField(context, _requirementDefinitionWithoutOneField, "I");

                // be sure to use different intervals in the TagRequirement and RequirementDefinition to be able to Assert on correct value for dto.IntervalWeeks
                _interval = _requirementDefinitionWithoutOneField.DefaultIntervalWeeks * 2;

                var requirementWithoutField = new TagRequirement(TestPlant, _interval, _requirementDefinitionWithoutOneField);

                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    Guid.NewGuid(),
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
                requirementWithoutField.SetComment(_comment);
                // Active Period gets a Preservation Record and the current Active Period be a new active Period when preserving
                tag.Preserve(new Mock<Person>().Object, _requirementWithoutFieldId);
                Assert.IsNotNull(activePeriodForRequirementWithOutField.PreservationRecord);

                context.SaveChangesAsync().Wait();
                _preservationRecordGuid = activePeriodForRequirementWithOutField.PreservationRecord.Guid;
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
                var dto = result.Data;
                Assert.AreEqual(dto.Id, _preservationRecordId);
                Assert.IsNotNull(dto.RequirementType);
                Assert.AreEqual(_interval, dto.IntervalWeeks);
                Assert.AreEqual(_comment, dto.Comment);
                Assert.AreEqual(_requirementType.Id, dto.RequirementType.Id);
                Assert.AreEqual(_requirementType.Code, dto.RequirementType.Code);
                Assert.AreEqual(_requirementType.Icon, dto.RequirementType.Icon);
                Assert.AreEqual(_requirementType.Title, dto.RequirementType.Title);
                Assert.IsNotNull(dto.RequirementDefinition);
                Assert.AreEqual(_requirementDefinitionWithoutOneField.Id, dto.RequirementDefinition.Id);
                Assert.AreEqual(_requirementDefinitionWithoutOneField.Title, dto.RequirementDefinition.Title);
                Assert.IsTrue(dto.Fields.Count == 1);
                var field = dto.Fields.ElementAt(0);
                Assert.AreEqual(_infoField.Id, field.Id);
                Assert.AreEqual(_infoField.FieldType, field.FieldType);
                Assert.AreEqual(_infoField.Label, field.Label);
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
