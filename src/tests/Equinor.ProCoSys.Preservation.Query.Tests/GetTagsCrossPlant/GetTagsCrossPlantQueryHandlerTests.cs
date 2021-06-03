using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsCrossPlant
{
    [TestClass]
    public class GetTagsCrossPlantQueryHandlerTests
    {
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;
        protected ManualTimeProvider _timeProvider;
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        private readonly PlantProvider _plantProvider = new PlantProvider(null);
        private readonly Mock<IPlantCache> _plantCacheMock = new Mock<IPlantCache>();

        private readonly int _intervalWeeks1 = 1;
        private readonly int _intervalWeeks2 = 2;
        private PCSPlant _plantA = new PCSPlant {Id = "PCS$A", Title = "A"};
        private PCSPlant _plantB = new PCSPlant {Id = "PCS$B", Title = "B"};
        private Project _projectA;
        private Project _projectB;
        private Tag _tagA;
        private Tag _tagB;
        private Person _currentUser;
        private Mode _mode1;
        private Mode _mode2;
        private Responsible _responsible1;
        private Responsible _responsible2;

        [TestInitialize]
        public void Setup()
        {
            _plantCacheMock.Setup(p => p.GetPlantTitleAsync(_plantA.Id)).Returns(Task.FromResult(_plantA.Title));
            _plantCacheMock.Setup(p => p.GetPlantTitleAsync(_plantB.Id)).Returns(Task.FromResult(_plantB.Title));
            
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

                (_projectA, _tagA) = CreateTag(context, _plantA.Id, "PrA", _intervalWeeks1);
                (_projectB, _tagB) = CreateTag(context, _plantB.Id, "PrB", _intervalWeeks2);
                
                EnsureProjectIsClosedDiffer(context);
                EnsureTagIsVoidedDiffer(context);
                EnsureTagStatusDiffer(context);
                EnsureActionStatusDifferAndSet(context);
            }
        }

        private void EnsureActionStatusDifferAndSet(PreservationContext context)
        {
            _tagA.AddAction(new Action(_tagA.Plant, "AcA", "AcA desc", null));
            _tagB.AddAction(new Action(_tagB.Plant, "AcB", "AcB desc", _timeProvider.UtcNow));
            context.SaveChangesAsync().Wait();
        }

        private void EnsureTagStatusDiffer(PreservationContext context)
        {
            _tagB.StartPreservation();
            context.SaveChangesAsync().Wait();
        }

        private void EnsureTagIsVoidedDiffer(PreservationContext context)
        {
            _tagA.IsVoided = true;
            context.SaveChangesAsync().Wait();
        }

        private void EnsureProjectIsClosedDiffer(PreservationContext context)
        {
            _projectA.Close();
            context.SaveChangesAsync().Wait();
        }

        private (Project, Tag) CreateTag(
            PreservationContext context,
            string plantId,
            string projectName,
            int intervalWeeks)
        {
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
                TagType.Standard,
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
            context.SaveChangesAsync().Wait();

            return (project, tag);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_WithAllPropertiesSet()
        {
            _timeProvider.ElapseWeeks(_intervalWeeks1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _plantCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);
                var tagDtos = result.Data;

                AssertTag(tagDtos.Single(t => t.Id == _tagA.Id), _tagA, _plantA, _projectA);
                AssertTag(tagDtos.Single(t => t.Id == _tagB.Id), _tagB, _plantB, _projectB);

                AssertDifferentValues(tagDtos.Select(tag => tag.IsProjectClosed).Cast<object>());
                AssertDifferentValues(tagDtos.Select(tag => tag.IsVoided).Cast<object>());
                AssertDifferentValues(tagDtos.Select(tag => tag.Status).Cast<object>());
                AssertDifferentValues(tagDtos.Select(tag => tag.ReadyToBePreserved).Cast<object>());
                AssertDifferentValues(tagDtos.Select(tag => tag.ActionStatus).Cast<object>());
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_FromDifferentPlants()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _plantCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                var tagDtos = result.Data;
                Assert.AreEqual(2, tagDtos.Count);
            }
        }

        private void AssertDifferentValues(IEnumerable<object> values)
        {
            var listOfValues = values.ToList();
            var expected = listOfValues.Count;
            var actual = listOfValues.Distinct().Count();
            Assert.AreEqual(expected, actual);
        }

        private void AssertTag(
            TagDto tagDto,
            Tag tag,
            PCSPlant plant,
            Project project)
        {
            AssertEqualAndNotNull(plant.Id, tagDto.PlantId);
            AssertEqualAndNotNull(plant.Title, tagDto.PlantTitle);
            AssertEqualAndNotNull(project.Name, tagDto.ProjectName);
            AssertEqualAndNotNull(project.Description, tagDto.ProjectDescription);
            Assert.AreEqual(project.IsClosed, project.IsClosed);
            AssertEqualAndNotNull(tag.Id, tagDto.Id);
            Assert.IsTrue(tagDto.ActionStatus.HasValue);
            AssertEqualAndNotNull(tag.AreaCode, tagDto.AreaCode);
            AssertEqualAndNotNull(tag.AreaDescription, tagDto.AreaDescription);
            AssertEqualAndNotNull(tag.Calloff, tagDto.CallOff);
            AssertEqualAndNotNull(tag.CommPkgNo, tagDto.CommPkgNo);
            AssertEqualAndNotNull(tag.Description, tagDto.Description);
            AssertEqualAndNotNull(tag.DisciplineCode, tagDto.DisciplineCode);
            Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
            AssertEqualAndNotNull(tag.McPkgNo, tagDto.McPkgNo);
            AssertEqualAndNotNull(_mode1.Title, tagDto.Mode);
            AssertEqualAndNotNull(_mode2.Title, tagDto.NextMode);
            AssertEqualAndNotNull(_responsible2.Code, tagDto.NextResponsibleCode);
            AssertEqualAndNotNull(_responsible2.Description, tagDto.NextResponsibleDescription);
            AssertEqualAndNotNull(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);
            Assert.AreEqual(tag.IsReadyToBePreserved(), tagDto.ReadyToBePreserved);
            AssertEqualAndNotNull(_responsible1.Code, tagDto.ResponsibleCode);
            AssertEqualAndNotNull(_responsible1.Description, tagDto.ResponsibleDescription);
            Assert.AreEqual(tag.Status, tagDto.Status);
            AssertEqualAndNotNull(tag.TagFunctionCode, tagDto.TagFunctionCode);
            AssertEqualAndNotNull(tag.TagNo, tagDto.TagNo);
            Assert.AreEqual(tag.TagType, tagDto.TagType);
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
