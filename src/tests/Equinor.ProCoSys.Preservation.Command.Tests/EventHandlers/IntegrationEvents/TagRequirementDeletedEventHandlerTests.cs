using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers.IntegrationEvents;

[TestClass]
public class TagRequirementDeletedEventHandlerTests
{
    private static DateTime TestTime => DateTime.Parse("2012-12-12T11:22:33Z").ToUniversalTime();
    private const string TestPlant = "PCS$PlantA";
    private TagRequirementDeletedEventHandler _dut;
    private IList<IIntegrationEvent> _publishedEvents;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        var timeProvider = new ManualTimeProvider(TestTime);
        TimeService.SetProvider(timeProvider);

        var mockPublisher = new Mock<IIntegrationEventPublisher>();
        mockPublisher.Setup(x => x.PublishAsync(It.IsAny<IIntegrationEvent>(), default))
            .Callback<IIntegrationEvent, CancellationToken>((e, _) => _publishedEvents.Add(e));

        var mockProjectRepository = new Mock<IProjectRepository>();
        mockProjectRepository.Setup(p => p.GetProjectOnlyByTagGuidAsync(It.IsAny<Guid>())).ReturnsAsync(new Mock<Project>().Object);

        _dut = new TagRequirementDeletedEventHandler(mockPublisher.Object, mockProjectRepository.Object);

        _publishedEvents = [];
    }

    [TestMethod]
    public async Task Handle_ShouldSendTagRequirementDeleteEvent()
    {
        // Arrange
        var tagRequirement = new Mock<TagRequirement>();
        var domainEvent = new TagRequirementDeletedEvent(string.Empty, Guid.Empty, tagRequirement.Object);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var result = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(result, typeof(TagRequirementDeleteEvent));
    }

    [TestMethod]
    public async Task Handle_ShouldSendPreservationPeriodDeleteEvent()
    {
        // Arrange
        var requirementDefinition = new RequirementDefinition(TestPlant, "Requirement Definition", 1, RequirementUsage.ForSuppliersOnly, 1);
        var tagRequirement = new TagRequirement(TestPlant, 1, requirementDefinition);
        tagRequirement.StartPreservation();

        var domainEvent = new TagRequirementDeletedEvent(string.Empty, Guid.Empty, tagRequirement);

        // Act
        await _dut.Handle(domainEvent, CancellationToken.None);
        var result = _publishedEvents.Select(e => e.GetType()).ToList();

        // Assert
        CollectionAssert.Contains(result, typeof(PreservationPeriodDeleteEvent));
    }
}
