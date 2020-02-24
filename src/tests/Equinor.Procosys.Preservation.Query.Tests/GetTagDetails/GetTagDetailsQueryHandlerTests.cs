using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private Mock<ITimeService> _timeServiceMock;
        private DateTime _startedPreservationUtc;
        private int _intervalWeeks = 2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
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
                        new Requirement(_schema, _intervalWeeks, reqType.RequirementDefinitions.ElementAt(0))
                    });

                _timeServiceMock = new Mock<ITimeService>();
                _startedPreservationUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
                _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservationUtc);
                tag.StartPreservation(_startedPreservationUtc);
                context.Tags.Add(tag);
                context.SaveChanges();

                _tagId = tag.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsTagDetails()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservationUtc.AddWeeks(_intervalWeeks));

                var query = new GetTagDetailsQuery(_tagId);
                var dut = new GetTagDetailsQueryHandler(context, _timeServiceMock.Object);

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
                Assert.IsTrue(dto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object,
                _plantProviderMock.Object))
            {
                var query = new GetTagDetailsQuery(0);
                var dut = new GetTagDetailsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}
