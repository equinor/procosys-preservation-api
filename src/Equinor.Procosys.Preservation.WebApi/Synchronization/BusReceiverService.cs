using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.WebApi.Telemetry;

namespace Equinor.ProCoSys.IPO.WebApi.Synchronization
{
    public class BusReceiverService : IBusReceiverService
    {
        private readonly IPlantSetter _plantSetter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private const string PreservationBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public BusReceiverService(IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            ITelemetryClient telemetryClient,
            IResponsibleRepository responsibleRepository,
            IProjectRepository projectRepository,
            ITagFunctionRepository tagFunctionRepository)
        {
            _plantSetter = plantSetter;
            _unitOfWork = unitOfWork;
            _telemetryClient = telemetryClient;
            _responsibleRepository = responsibleRepository;
            _projectRepository = projectRepository;
            _tagFunctionRepository = tagFunctionRepository;
        }

        public async Task ProcessMessageAsync(PcsTopic pcsTopic, string messageJson, CancellationToken cancellationToken)
        {
            switch (pcsTopic)
            {
                case PcsTopic.Project:
                    await ProcessProjectEvent(messageJson);
                    break;
                case PcsTopic.Responsible:
                    await ProcessResponsibleEvent(messageJson);
                    break;
                case PcsTopic.TagFunction:
                    await ProcessTagFunctionEvent(messageJson);
                    break;
                case PcsTopic.CommPkg:
                    await ProcessCommPkgEvent(messageJson);
                    break;
                case PcsTopic.McPkg:
                    await ProcessMcPkgEvent(messageJson);
                    break;
                case PcsTopic.Tag:
                    // TODO: Handle Tag
                    throw new NotImplementedException();

            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessMcPkgEvent(string messageJson)
        {
            var mcPkgEvent = JsonSerializer.Deserialize<McPkgTopic>(messageJson);

            if (string.IsNullOrWhiteSpace(mcPkgEvent.Plant) ||
                string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNo) ||
                string.IsNullOrWhiteSpace(mcPkgEvent.CommPkgNo) ||
                string.IsNullOrWhiteSpace(mcPkgEvent.ProjectName) ||
                (string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNoOld) != (string.IsNullOrWhiteSpace(mcPkgEvent.CommPkgNoOld))))
            {
                throw new Exception($"Unable to deserialize JSON to McPkgEvent {messageJson}");
            }

            if (string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNoOld) || string.IsNullOrWhiteSpace(mcPkgEvent.CommPkgNoOld))
            {
                // Nothing to do
                return;
            }

            TrackMcPkgEvent(mcPkgEvent);

            _plantSetter.SetPlant(mcPkgEvent.Plant);

            var project = await _projectRepository.GetProjectOnlyByNameAsync(mcPkgEvent.ProjectName);

            if (!string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNoOld))
            {
                project.RenameMcPkg(mcPkgEvent.McPkgNoOld, mcPkgEvent.McPkgNo, mcPkgEvent.CommPkgNoOld);
                if (mcPkgEvent.CommPkgNoOld != mcPkgEvent.CommPkgNo)
                {
                    project.MoveMcPkg(mcPkgEvent.McPkgNo, mcPkgEvent.CommPkgNoOld, mcPkgEvent.CommPkgNo);
                }
            }
        }

        private async Task ProcessCommPkgEvent(string messageJson)
        {
            var commPkgEvent = JsonSerializer.Deserialize<CommPkgTopic>(messageJson);

            if (string.IsNullOrWhiteSpace(commPkgEvent.Plant) ||
                string.IsNullOrWhiteSpace(commPkgEvent.CommPkgNo) ||
                string.IsNullOrWhiteSpace(commPkgEvent.ProjectName))
            {
                throw new Exception($"Unable to deserialize JSON to CommPkgEvent {messageJson}");
            }

            if (string.IsNullOrWhiteSpace(commPkgEvent.ProjectNameOld))
            {
                // Nothing to process
                return;
            }

            TrackCommPkgEvent(commPkgEvent);

            _plantSetter.SetPlant(commPkgEvent.Plant);

            await _projectRepository.MoveCommPkgAsync(
                commPkgEvent.CommPkgNo,
                commPkgEvent.ProjectNameOld,
                commPkgEvent.ProjectName);
        }

        private async Task ProcessTagFunctionEvent(string messageJson)
        {
            var tagFunctionEvent = JsonSerializer.Deserialize<TagFunctionTopic>(messageJson);
            if (string.IsNullOrWhiteSpace(tagFunctionEvent.Plant) ||
                string.IsNullOrWhiteSpace(tagFunctionEvent.Code) ||
                string.IsNullOrWhiteSpace(tagFunctionEvent.RegisterCode))
            {
                throw new Exception($"Unable to deserialize JSON to TagFunctionEven {messageJson}");
            }

            TrackTagFunctionEvent(tagFunctionEvent);

            _plantSetter.SetPlant(tagFunctionEvent.Plant);

            if (!string.IsNullOrWhiteSpace(tagFunctionEvent.CodeOld) ||
                !string.IsNullOrWhiteSpace(tagFunctionEvent.RegisterCodeOld))
            {
                var tagFunction =
                    await _tagFunctionRepository.GetByCodesAsync(tagFunctionEvent.CodeOld, tagFunctionEvent.RegisterCodeOld);
                if (tagFunction != null)
                {
                    tagFunction.RenameTagFunction(tagFunctionEvent.Code, tagFunctionEvent.RegisterCode);
                    tagFunction.Description = tagFunctionEvent.Description;
                    tagFunction.IsVoided = tagFunctionEvent.IsVoided;
                }
            }
            else
            {
                var tagFunction =
                    await _tagFunctionRepository.GetByCodesAsync(tagFunctionEvent.Code, tagFunctionEvent.RegisterCode);
                if (tagFunction != null)
                {
                    tagFunction.Description = tagFunctionEvent.Description;
                    tagFunction.IsVoided = tagFunctionEvent.IsVoided;
                }
            }
            
        }

        private async Task ProcessProjectEvent(string messageJson)
        {
            var projectEvent = JsonSerializer.Deserialize<ProjectTopic>(messageJson);
            if (string.IsNullOrWhiteSpace(projectEvent.Plant) || string.IsNullOrWhiteSpace(projectEvent.ProjectName))
            {
                throw new Exception($"Unable to deserialize JSON to ProjectEvent {messageJson}");
            }

            TrackProjectEvent(projectEvent);

            _plantSetter.SetPlant(projectEvent.Plant);

            var project = await _projectRepository.GetProjectOnlyByNameAsync(projectEvent.ProjectName);
            if (project != null)
            {
                project.Description = projectEvent.Description;
                project.IsClosed = projectEvent.IsClosed;
            }
        }

        private async Task ProcessResponsibleEvent(string messageJson)
        {
            var responsibleEvent = JsonSerializer.Deserialize<ResponsibleTopic>(messageJson);
            if (string.IsNullOrWhiteSpace(responsibleEvent.Plant) || string.IsNullOrWhiteSpace(responsibleEvent.Code))
            {
                throw new Exception($"Unable to deserialize JSON to ResponsibleEvent {messageJson}");
            }

            TrackResponsibleEvent(responsibleEvent);

            _plantSetter.SetPlant(responsibleEvent.Plant);

            if (!string.IsNullOrWhiteSpace(responsibleEvent.CodeOld))
            {
                var responsible = await _responsibleRepository.GetByCodeAsync(responsibleEvent.CodeOld);
                if (responsible != null)
                {
                    responsible.Description = responsibleEvent.Description;
                    responsible.RenameResponsible(responsibleEvent.Code);
                }
            }
            else
            {
                var responsible = await _responsibleRepository.GetByCodeAsync(responsibleEvent.Code);
                if (responsible != null)
                {
                    responsible.Description = responsibleEvent.Description;
                }
            }
        }

        private void TrackResponsibleEvent(ResponsibleTopic responsibleEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, ResponsibleTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.ResponsibleCode, responsibleEvent.Code},
                    {PcsServiceBusTelemetryConstants.Plant, responsibleEvent.Plant[4..]},
                });

        private void TrackCommPkgEvent(CommPkgTopic commPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, IpoTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.CommPkgNo, commPkgEvent.CommPkgNo},
                    {PcsServiceBusTelemetryConstants.Plant, commPkgEvent.Plant[4..]},
                    {PcsServiceBusTelemetryConstants.ProjectName, commPkgEvent.ProjectName.Replace('$', '_')},
                    {PcsServiceBusTelemetryConstants.ProjectNameOld, commPkgEvent.ProjectNameOld.Replace('$', '_')}
                });

        private void TrackTagFunctionEvent(TagFunctionTopic tagFunctionEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, TagFunctionTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.Code, tagFunctionEvent.Code},
                    {PcsServiceBusTelemetryConstants.RegisterCode, tagFunctionEvent.RegisterCode},
                    {PcsServiceBusTelemetryConstants.IsVoided, tagFunctionEvent.IsVoided.ToString()},
                    {PcsServiceBusTelemetryConstants.Plant, tagFunctionEvent.Plant[4..]},
                });

        private void TrackProjectEvent(ProjectTopic projectEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, ProjectTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.ProjectName, projectEvent.ProjectName},
                    {PcsServiceBusTelemetryConstants.IsVoided, projectEvent.IsClosed.ToString()},
                    {PcsServiceBusTelemetryConstants.Plant, projectEvent.Plant[4..]},
                });

        private void TrackMcPkgEvent(McPkgTopic mcPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {PcsServiceBusTelemetryConstants.Event, McPkgTopic.TopicName},
                    {PcsServiceBusTelemetryConstants.McPkgNo, mcPkgEvent.McPkgNo},
                    {PcsServiceBusTelemetryConstants.Plant, mcPkgEvent.Plant[4..]},
                    {PcsServiceBusTelemetryConstants.ProjectName, mcPkgEvent.ProjectName.Replace('$', '_')}
                });
    }
}
