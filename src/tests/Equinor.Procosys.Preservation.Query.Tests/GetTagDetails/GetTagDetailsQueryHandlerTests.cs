using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagDetails
{
    [TestClass]
    public class GetTagDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private string _journeyTitle = "J1";
        private string _modeTitle = "M1";
        private string _respCode = "R1";
        private int _tagId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProviderMock.Object))
            {
                var journey = AddJourneyWithStep(context, _journeyTitle, 
                    AddMode(context, _modeTitle), 
                    AddResponsible(context, _respCode));
                var reqType = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1");

                var tag = new Tag(_schema,
                    TagType.Standard,
                    "TagNo",
                    "Description",
                    "AreaCode",
                    "Calloff",
                    "DisciplineCode",
                    "McPkgNo",
                    "CommPkgNo",
                    "PurchaseOrderNo",
                    "Remark",
                    "TagFunctionCode",
                    journey.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(_schema, 2, reqType.RequirementDefinitions.ElementAt(0))
                    });

                tag.StartPreservation(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
                context.Tags.Add(tag);
                context.SaveChanges();

                _tagId = tag.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsTagDetails()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object))
            {
                var query = new GetTagDetailsQuery(_tagId);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var dto = result.Data;
                Assert.AreEqual("AreaCode", dto.AreaCode);
                Assert.AreEqual("CommPkgNo", dto.CommPkgNo);
                Assert.AreEqual("Description", dto.Description);
                Assert.AreEqual(_tagId, dto.Id);
                Assert.AreEqual(_journeyTitle, dto.JourneyTitle);
                Assert.AreEqual("McPkgNo", dto.McPkgNo);
                Assert.AreEqual(_modeTitle, dto.Mode);
                Assert.AreEqual("PurchaseOrderNo", dto.PurchaseOrderNo);
                Assert.AreEqual(_respCode, dto.ResponsibleName);
                Assert.AreEqual(PreservationStatus.Active, dto.Status);
                Assert.AreEqual("TagNo", dto.TagNo);
                Assert.AreEqual(TagType.Standard, dto.TagType);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object))
            {
                var query = new GetTagDetailsQuery(0);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
