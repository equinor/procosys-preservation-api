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
using ServiceResult;

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

        private PCSPlant _plantA = new PCSPlant {Id = "PCS$A", Title = "A"};
        private PCSPlant _plantB = new PCSPlant {Id = "PCS$B", Title = "B"};
        private Project _projectA;
        private Project _projectB;
        private Tag _tagA;
        private Tag _tagB;

        private Person _currentUser;

        [TestInitialize]
        public void Setup()
        {
            _plantCacheMock.Setup(p => p.GetPlantAsync(_plantA.Id)).Returns(Task.FromResult(_plantA));
            _plantCacheMock.Setup(p => p.GetPlantAsync(_plantB.Id)).Returns(Task.FromResult(_plantB));
            
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

                (_projectA, _tagA) = CreateTag(context, _plantA.Id, "PrA");
                (_projectB, _tagB) = CreateTag(context, _plantB.Id, "PrB");
            }
        }

        private (Project, Tag) CreateTag(PreservationContext context, string plantId, string projectName)
        {
            var mode = new Mode(plantId, "M1", false);
            context.Modes.Add(mode);
                
            var responsible = new Responsible(plantId, "Resp1", "Resp1-Desc");
            context.Responsibles.Add(responsible);
            context.SaveChangesAsync().Wait();

            var journey = new Journey(plantId, "J1");
            var step = new Step(plantId, "S1", mode, responsible);
            journey.AddStep(step);
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();

            var requirementType = new RequirementType(plantId, "RT", "RT title", RequirementTypeIcon.Other, 1);
            context.RequirementTypes.Add(requirementType);

            var requirementDefinition = new RequirementDefinition(plantId, "RD", 2, RequirementUsage.ForAll, 1);
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
                new List<TagRequirement> {new TagRequirement(plantId, 2, requirementDefinition)});
            project.AddTag(tag);
            context.SaveChangesAsync().Wait();

            var attachment = new TagAttachment(plantId, Guid.Empty, "fil.txt");
            tag.AddAttachment(attachment);

            context.SaveChangesAsync().Wait();
            return (project, tag);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTags_FromDifferentPlants()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagsCrossPlantQuery();
                var dut = new GetTagsCrossPlantQueryHandler(context, _plantCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var tagDtos = result.Data;
                Assert.AreEqual(2, tagDtos.Count);

                AssertTag(tagDtos.Single(t => t.Id == _tagA.Id), _tagA, _plantA, _projectA);
                AssertTag(tagDtos.Single(t => t.Id == _tagB.Id), _tagB, _plantB, _projectB);
            }
        }

        private void AssertTag(TagDto tagDto, Tag tag, PCSPlant plant, Project project)
        {
            Assert.AreEqual(tagDto.PlantId, plant.Id);
            Assert.AreEqual(tagDto.PlantTitle, plant.Title);
            Assert.AreEqual(tagDto.ProjectName, project.Name);
            Assert.AreEqual(tagDto.ProjectDescription, project.Description);
            Assert.AreEqual(tagDto.Id, tag.Id);
            Assert.AreEqual(tagDto.AttachmentCount, tag.Attachments.Count);
        }
    }
}
