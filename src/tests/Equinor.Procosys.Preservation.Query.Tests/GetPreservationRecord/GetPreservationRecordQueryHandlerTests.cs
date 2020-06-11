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
        private int _requirementId;
        private  int _preservationRecordId;

        //private readonly DateTime _dueTimeUtc = DateTime.UtcNow;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var reqDef = new RequirementDefinition(TestPlant, "TestTitle", 2, RequirementUsage.ForAll, 1);

                var requirement = new TagRequirement(TestPlant, 2, reqDef);
                context.Add(requirement);
                context.SaveChangesAsync().Wait();
                _requirementId = requirement.Id;

                var preservationRecord = new PreservationRecord(TestPlant, new Mock<Person>().Object, true);
                context.Add(preservationRecord);
                context.SaveChangesAsync().Wait();
                _preservationRecordId = preservationRecord.Id;

                var tag = new Tag(TestPlant, TagType.Standard, "TagNo", "Description", journey.Steps.ElementAt(0),
                    new List<TagRequirement> {requirement});
                context.Tags.Add(tag);
                context.SaveChangesAsync().Wait();
                _tagId = tag.Id;

            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_ShouldSucceed_WhenKnownRecord()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetPreservationRecordQuery(_tagId, _requirementId, _preservationRecordId);
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
                var query = new GetPreservationRecordQuery(tagId, _requirementId, _preservationRecordId);
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
                var query = new GetPreservationRecordQuery(_tagId, reqId, _preservationRecordId);
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
                var presevRecordId = 33;
                var query = new GetPreservationRecordQuery(_tagId, _requirementId, presevRecordId);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.AreEqual(1, result.Errors.Count);
                Assert.IsTrue(result.Errors[0].StartsWith(Strings.EntityNotFound(nameof(PreservationPeriod), presevRecordId)));
            }
        }
    }
}
