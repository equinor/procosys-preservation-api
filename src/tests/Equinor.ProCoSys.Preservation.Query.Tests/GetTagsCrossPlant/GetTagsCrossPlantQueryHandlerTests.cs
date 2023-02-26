using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Auth.Time;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Auth.Permission;
using Equinor.ProCoSys.Auth.Caches;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsCrossPlant
{
    [TestClass]
    public class GetTagsCrossPlantQueryHandlerTests
    {
        // NB The PlantProvider affects when debugging and locking into DBSets in PreservationContext
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;
        protected ManualTimeProvider _timeProvider;
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        private readonly PlantProviderForTest _plantProvider = new PlantProviderForTest(null);
        private readonly Mock<IPermissionCache> _permissionCacheMock = new Mock<IPermissionCache>();

        private readonly int _intervalWeeks1 = 1;
        private readonly int _intervalWeeks2 = 2;
        private AccessablePlant _plantA = new AccessablePlant {Id = "PCS$A", Title = "A"};
        private AccessablePlant _plantB = new AccessablePlant {Id = "PCS$B", Title = "B"};
        private int _projectAId;
        private int _projectBId;
        private int _standardTagId;
        private int _voidedTagId;
        private int _siteTagId;
        private Person _currentUser;
        private Mode _mode1;
        private Mode _mode2;
        private Responsible _responsible1;
        private Responsible _responsible2;

        [TestInitialize]
        public void Setup()
        {
            _permissionCacheMock.Setup(p => p.GetPlantTitleForCurrentUserAsync(_plantA.Id)).Returns(Task.FromResult(_plantA.Title));
            _permissionCacheMock.Setup(p => p.GetPlantTitleForCurrentUserAsync(_plantB.Id)).Returns(Task.FromResult(_plantB.Title));
            
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(x => x.GetCurrentUserOid()).Returns(_currentUserOid);
            _currentUserProvider = currentUserProviderMock.Object;

            var eventDispatcher = new Mock<IEventDispatcher>();
            _eventDispatcher = eventDispatcher.Object;

            _timeProvider = new ManualTimeProvider(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // ensure current user exists in db
                _currentUser = new Person(_currentUserOid, "Ole", "Lukkøye");
                context.Persons.Add(_currentUser);
                context.SaveChangesAsync().Wait();

                _plantProvider.SetPlant(_plantA.Id);
                (_projectAId, _standardTagId) = CreateTag(context, "PrA1", TagType.Standard, _intervalWeeks1, false);
                (_, _voidedTagId) = CreateTag(context, "PrA2", TagType.Standard, _intervalWeeks1, true);
                _plantProvider.SetPlant(_plantB.Id);
                (_projectBId, _siteTagId) = CreateTag(context, "PrB", TagType.SiteArea, _intervalWeeks2, false);
                _plantProvider.SetCrossPlantQuery();
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_WithAllPropertiesSet()
        {
            Project projectA;
            Project projectB;
            Tag standardTag;
            Tag siteTag;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                projectA = context.Projects.Single(p => p.Id == _projectAId);
                projectB = context.Projects.Single(p => p.Id == _projectBId);
                projectB.Close();

                standardTag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagId);
                standardTag.StartPreservation();
                siteTag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _siteTagId);

                standardTag.AddAction(new Action(standardTag.Plant, "AcA", "AcA desc", null));
                siteTag.AddAction(new Action(siteTag.Plant, "AcB", "AcB desc", _timeProvider.UtcNow));
                context.SaveChangesAsync().Wait();
            
                _timeProvider.ElapseWeeks(standardTag.Requirements.Single().IntervalWeeks);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _permissionCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);
                var tagDtos = result.Data;

                var standardTagDto = tagDtos.Single(t => t.Id == standardTag.Id);
                AssertTag(standardTagDto, standardTag, _plantA, projectA);
                AssertJourneySpecificDataOnStandardTag(standardTagDto);
                var siteTagDto = tagDtos.Single(t => t.Id == siteTag.Id);
                AssertTag(siteTagDto, siteTag, _plantB, projectB);
                AssertJourneySpecificDataOnSiteTag(siteTagDto);

                Assert.IsTrue(siteTagDto.ProjectIsClosed);
                Assert.AreEqual(ActionStatus.HasOpen, standardTagDto.ActionStatus);
                Assert.AreEqual(ActionStatus.HasOverdue, siteTagDto.ActionStatus);
                Assert.AreEqual(PreservationStatus.Active, standardTagDto.Status);
                Assert.AreEqual(PreservationStatus.NotStarted, siteTagDto.Status);
                Assert.IsTrue(standardTagDto.ReadyToBePreserved);
                Assert.IsFalse(siteTagDto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_FromDifferentPlants()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _permissionCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                Assert.AreEqual(2, result.Data.Count);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldNotReturnVoidedTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _permissionCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                Assert.IsNull(result.Data.SingleOrDefault(t => t.Id == _voidedTagId));
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_LimitedToMax()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery(1);
                var dut = new GetTagsCrossPlantQueryHandler(context, _permissionCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                Assert.AreEqual(1, result.Data.Count);
            }
        }

        private void AssertTag(
            TagDto tagDto,
            Tag tag,
            AccessablePlant plant,
            Project project)
        {
            AssertEqualAndNotNull(plant.Id, tagDto.PlantId);
            AssertEqualAndNotNull(plant.Title, tagDto.PlantTitle);
            AssertEqualAndNotNull(project.Name, tagDto.ProjectName);
            AssertEqualAndNotNull(project.Description, tagDto.ProjectDescription);
            Assert.AreEqual(project.IsClosed, tagDto.ProjectIsClosed);
            AssertEqualAndNotNull(tag.Id, tagDto.Id);
            Assert.IsTrue(tagDto.ActionStatus.HasValue);
            AssertEqualAndNotNull(tag.AreaCode, tagDto.AreaCode);
            AssertEqualAndNotNull(tag.AreaDescription, tagDto.AreaDescription);
            AssertEqualAndNotNull(tag.Calloff, tagDto.Calloff);
            AssertEqualAndNotNull(tag.CommPkgNo, tagDto.CommPkgNo);
            AssertEqualAndNotNull(tag.Description, tagDto.Description);
            AssertEqualAndNotNull(tag.DisciplineCode, tagDto.DisciplineCode);
            AssertEqualAndNotNull(tag.McPkgNo, tagDto.McPkgNo);
            AssertEqualAndNotNull(_mode1.Title, tagDto.Mode);
            AssertEqualAndNotNull(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);
            Assert.AreEqual(tag.IsReadyToBePreserved(), tagDto.ReadyToBePreserved);
            AssertEqualAndNotNull(_responsible1.Code, tagDto.ResponsibleCode);
            AssertEqualAndNotNull(_responsible1.Description, tagDto.ResponsibleDescription);
            Assert.AreEqual(tag.Status, tagDto.Status);
            AssertEqualAndNotNull(tag.TagFunctionCode, tagDto.TagFunctionCode);
            AssertEqualAndNotNull(tag.TagNo, tagDto.TagNo);
            Assert.AreEqual(tag.TagType, tagDto.TagType);
        }

        private void AssertJourneySpecificDataOnStandardTag(TagDto tagDto)
        {
            AssertEqualAndNotNull(_mode2.Title, tagDto.NextMode);
            AssertEqualAndNotNull(_responsible2.Code, tagDto.NextResponsibleCode);
            AssertEqualAndNotNull(_responsible2.Description, tagDto.NextResponsibleDescription);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertJourneySpecificDataOnSiteTag(TagDto tagDto)
        {
            Assert.IsNull(tagDto.NextMode);
            Assert.IsNull(tagDto.NextResponsibleCode);
            Assert.IsNull(tagDto.NextResponsibleDescription);
        }

        private (int, int) CreateTag(
            PreservationContext context,
            string projectName,
            TagType tagType,
            int intervalWeeks,
            bool voidTag)
        {
            var plantId = _plantProvider.Plant;
            _mode1 = new Mode(plantId, "M1", false);
            _mode2 = new Mode(plantId, "M2", false);
            context.Modes.Add(_mode1);
            context.Modes.Add(_mode2);

            _responsible1 = new Responsible(plantId, "Resp1", "Resp1-Desc");
            _responsible2 = new Responsible(plantId, "Resp2", "Resp2-Desc");
            context.Responsibles.Add(_responsible1);
            context.Responsibles.Add(_responsible2);
            context.SaveChangesAsync().Wait();

            var journey = new Journey(plantId, "J1");
            var step = new Step(plantId, "S1", _mode1, _responsible1);
            journey.AddStep(step);
            journey.AddStep(new Step(plantId, "S2", _mode2, _responsible2));
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();

            var requirementType = new RequirementType(plantId, "RT", "RT title", RequirementTypeIcon.Other, 1);
            context.RequirementTypes.Add(requirementType);

            var requirementDefinition = new RequirementDefinition(plantId, "RD", intervalWeeks, RequirementUsage.ForAll, 1);
            requirementType.AddRequirementDefinition(requirementDefinition);
            context.SaveChangesAsync().Wait();

            var project = new Project(plantId, projectName, $"{projectName} Desc");
            context.Projects.Add(project);

            var tag = new Tag(
                plantId,
                tagType,
                tagType == TagType.Standard ? Guid.NewGuid() : null,
                "Tag A",
                "Tag desc",
                step,
                new List<TagRequirement> {new TagRequirement(plantId, _intervalWeeks1, requirementDefinition)})
            {
                Calloff = "C",
                CommPkgNo = "Cp",
                McPkgNo = "Mp",
                PurchaseOrderNo = "PO",
                TagFunctionCode = "TF"
            };
            tag.SetArea("A", "A desc");
            tag.SetDiscipline("D", "D desc");
            project.AddTag(tag);
            if (voidTag)
            {
                tag.IsVoided = true;
            }
            context.SaveChangesAsync().Wait();

            return (project.Id, tag.Id);
        }

        private void AssertEqualAndNotNull(string expected, string actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        private void AssertEqualAndNotNull(int expected, int actual)
        {
            Assert.IsTrue(actual > 0);
            Assert.AreEqual(expected, actual);
        }
    }
}
