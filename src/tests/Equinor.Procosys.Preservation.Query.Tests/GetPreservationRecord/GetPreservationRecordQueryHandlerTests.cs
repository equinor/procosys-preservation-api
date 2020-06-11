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
        public async Task HandleGetPreservationRecordQuery_KnownRecord_ShouldReturnPreservationRecord()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetPreservationRecordQuery(_tagId, _requirementId, _preservationRecordId);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                //// 
                //var preservationRecord =
                //    new PreservationRecordDto(result.Data.Id, result.Data.BulkPreserved, result.Data.RowVersion);

                Assert.IsNotNull(result);
                //Assert.AreEqual(preservationRecord, result);
                Assert.AreEqual(result.Data.Id, _preservationRecordId);
            }
        }

        [TestMethod]
        public async Task HandleGetPreservationRecordQuery_UnknownRecord_ShouldReturnNull()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var query = new GetPreservationRecordQuery(11, 22, 33);
                var dut = new GetPreservationRecordQueryHandler(context);
                var result = await dut.Handle(query, default);

                Assert.IsNull(result);
            }
        }
    }
}
