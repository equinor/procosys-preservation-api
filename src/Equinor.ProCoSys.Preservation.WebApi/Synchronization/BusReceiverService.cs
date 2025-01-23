using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Microsoft.Extensions.Options;
using Equinor.ProCoSys.Auth.Authentication;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Telemetry;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class BusReceiverService : IBusReceiverService
    {
        private readonly IPlantSetter _plantSetter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IProjectApiService _projectApiService;
        private readonly ICertificateEventProcessorService _certificateEventProcessorService;
        private readonly Guid _preservationApiOid;
        private const string PreservationBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public BusReceiverService(IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            ITelemetryClient telemetryClient,
            IResponsibleRepository responsibleRepository,
            IProjectRepository projectRepository,
            ITagFunctionRepository tagFunctionRepository,
            ICurrentUserSetter currentUserSetter,
            IOptionsSnapshot<PreservationAuthenticatorOptions> options,
            IProjectApiService projectApiService,
            ICertificateEventProcessorService certificateEventProcessorService)
        {
            _plantSetter = plantSetter;
            _unitOfWork = unitOfWork;
            _telemetryClient = telemetryClient;
            _responsibleRepository = responsibleRepository;
            _projectRepository = projectRepository;
            _tagFunctionRepository = tagFunctionRepository;
            _currentUserSetter = currentUserSetter;
            _projectApiService = projectApiService;
            _certificateEventProcessorService = certificateEventProcessorService;

            _preservationApiOid = options.Value.PreservationApiObjectId;
        }

        public async Task ProcessMessageAsync(string pcsTopic, string messageJson, CancellationToken cancellationToken)
        {
            _currentUserSetter.SetCurrentUserOid(_preservationApiOid);

            switch (pcsTopic)
            {
                case PcsTopicConstants.Project:
                    await ProcessProjectEvent(messageJson);
                    break;
                case PcsTopicConstants.Responsible:
                    await ProcessResponsibleEvent(messageJson);
                    break;
                case PcsTopicConstants.TagFunction:
                    await ProcessTagFunctionEvent(messageJson);
                    break;
                case PcsTopicConstants.CommPkg:
                    await ProcessCommPkgEvent(messageJson, cancellationToken);
                    break;
                case PcsTopicConstants.McPkg:
                    await ProcessMcPkgEvent(messageJson);
                    break;
                case PcsTopicConstants.Tag:
                    await ProcessTagEvent(messageJson, cancellationToken);
                    break;
                case PcsTopicConstants.Certificate:
                    await _certificateEventProcessorService.ProcessCertificateEventAsync(messageJson);
                    break;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessTagEvent(string messageJson, CancellationToken cancellationToken)
        {
            var tagEvent = JsonSerializer.Deserialize<TagTopic>(messageJson);
            if (tagEvent != null && tagEvent.Behavior == "delete")
            {
                await ProcessTagDeleteEventAsync(tagEvent, messageJson);
            }
            else
            {
                await ProcessTagEventAsync(tagEvent, messageJson, cancellationToken);
            }
        }

        private async Task ProcessTagEventAsync(TagTopic tagEvent, string messageJson, CancellationToken cancellationToken)
        {
            if (tagEvent == null ||
                tagEvent.Plant.IsEmpty() ||
                tagEvent.TagNo.IsEmpty() ||
                tagEvent.ProjectName.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid TagEvent {messageJson}");
            }

            TrackTagEvent(tagEvent);

            _plantSetter.SetPlant(tagEvent.Plant);
            var tagGuid = Guid.ParseExact(tagEvent.ProCoSysGuid, "N");
            var tagToUpdate = await _projectRepository.GetTagOnlyByGuidAsync(tagGuid);

            if (tagToUpdate != null)
            {
                if (tagToUpdate.TagNo != tagEvent.TagNo)
                {
                    tagToUpdate.Rename(tagEvent.TagNo);
                }

                var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tagGuid);
                if (project == null)
                {
                    throw new ArgumentException($"Tag {tagToUpdate.TagNo} found, but project for tag not found.");
                }
                if (project.Name != tagEvent.ProjectName)
                {
                    var projectToMoveTagInto = await FindOrCreatePreservationCopyOfProjectAsync(tagEvent.Plant, tagEvent.ProjectName, cancellationToken);
                    project.MoveToProject(tagToUpdate, projectToMoveTagInto);
                }

                tagToUpdate.SetArea(tagEvent.AreaCode, tagEvent.AreaDescription);
                tagToUpdate.SetDiscipline(tagEvent.DisciplineCode, tagEvent.DisciplineDescription);
                tagToUpdate.Calloff = tagEvent.CallOffNo;
                tagToUpdate.PurchaseOrderNo = tagEvent.PurchaseOrderNo;
                tagToUpdate.CommPkgNo = tagEvent.CommPkgNo;
                tagToUpdate.Description = tagEvent.Description;
                tagToUpdate.McPkgNo = tagEvent.McPkgNo;
                tagToUpdate.TagFunctionCode = tagEvent.TagFunctionCode;
                // when voiding in Main, we IsVoidedInSource in preservation, which also set IsVoided
                tagToUpdate.IsVoidedInSource = tagEvent.IsVoided;
            }
        }

        private async Task ProcessTagDeleteEventAsync(TagTopic tagEvent, string messageJson)
        {
            if (tagEvent == null ||
                tagEvent.Plant.IsEmpty() ||
                tagEvent.ProCoSysGuid.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid TagEvent for delete {messageJson}");
            }

            var guid = new Guid(tagEvent.ProCoSysGuid);
            TrackDeleteEvent(PcsTopicConstants.Tag, tagEvent.ProCoSysGuid, true);

            _plantSetter.SetPlant(tagEvent.Plant);

            var tagToDelete = await _projectRepository.GetTagOnlyByGuidAsync(guid);
            if (tagToDelete != null)
            {
                tagToDelete.IsDeletedInSource = true;
            }
        }

        private async Task<Project> FindOrCreatePreservationCopyOfProjectAsync(string plant, string projectName, CancellationToken cancellationToken)
        {
            var projectToMoveTagInto = await _projectRepository.GetProjectWithTagsByNameAsync(projectName);
            if (projectToMoveTagInto == null)
            {
                var pcsProject = await _projectApiService.TryGetProjectAsync(plant, projectName, cancellationToken);
                if (pcsProject == null)
                {
                    throw new ArgumentException($"Unable to create local copy of {projectName}, not found.");
                }
                projectToMoveTagInto = new Project(plant, pcsProject.Name, pcsProject.Description, pcsProject.ProCoSysGuid);
                _projectRepository.Add(projectToMoveTagInto);
            }

            return projectToMoveTagInto;
        }

        private async Task ProcessMcPkgEvent(string messageJson)
        {
            var mcPkgEvent = JsonSerializer.Deserialize<McPkgTmpTopic>(messageJson);
            if (mcPkgEvent != null && mcPkgEvent.Behavior == "delete")
            {
                TrackDeleteEvent(PcsTopicConstants.McPkg, mcPkgEvent.ProCoSysGuid, false);
                return;
            }

            if (mcPkgEvent == null ||
                mcPkgEvent.Plant.IsEmpty() ||
                mcPkgEvent.McPkgNo.IsEmpty() ||
                mcPkgEvent.CommPkgNo.IsEmpty() ||
                mcPkgEvent.ProjectName.IsEmpty() ||
                (mcPkgEvent.McPkgNoOld.IsEmpty() != mcPkgEvent.CommPkgNoOld.IsEmpty()))
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid McPkgEvent {messageJson}");
            }

            if (mcPkgEvent.McPkgNoOld.IsEmpty() || mcPkgEvent.CommPkgNoOld.IsEmpty())
            {
                // Nothing to do
                return;
            }

            TrackMcPkgEvent(mcPkgEvent);

            _plantSetter.SetPlant(mcPkgEvent.Plant);

            var project = await _projectRepository.GetProjectWithTagsByNameAsync(mcPkgEvent.ProjectName);

            if (project == null)
            {
                // Project is not in the preservation database and hence no preservation tags to update
                return;
            }

            if (!mcPkgEvent.McPkgNoOld.IsEmpty())
            {
                if (mcPkgEvent.McPkgNoOld != mcPkgEvent.McPkgNo)
                {
                    project.RenameMcPkg(mcPkgEvent.CommPkgNoOld, mcPkgEvent.McPkgNoOld, mcPkgEvent.McPkgNo);
                }
                if (mcPkgEvent.CommPkgNoOld != mcPkgEvent.CommPkgNo)
                {
                    project.MoveMcPkg(mcPkgEvent.McPkgNo, mcPkgEvent.CommPkgNoOld, mcPkgEvent.CommPkgNo);
                }
            }
        }

        private async Task ProcessCommPkgEvent(string messageJson, CancellationToken cancellationToken)
        {
            var commPkgEvent = JsonSerializer.Deserialize<CommPkgTmpTopic>(messageJson);
            if (commPkgEvent != null && commPkgEvent.Behavior == "delete")
            {
                TrackDeleteEvent(PcsTopicConstants.CommPkg, commPkgEvent.ProCoSysGuid, false);
                return;
            }

            if (commPkgEvent == null ||
                commPkgEvent.Plant.IsEmpty() ||
                commPkgEvent.CommPkgNo.IsEmpty() ||
                commPkgEvent.ProjectName.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid CommPkgEvent {messageJson}");
            }

            if (commPkgEvent.ProjectNameOld.IsEmpty())
            {
                // Nothing to process
                return;
            }

            TrackCommPkgEvent(commPkgEvent);

            _plantSetter.SetPlant(commPkgEvent.Plant);

            var fromProject = await _projectRepository.GetProjectWithTagsByNameAsync(commPkgEvent.ProjectNameOld);
            if (fromProject == null)
            {
                // Project is not in the preservation database and hence no preservation tags to update
                return;
            }

            var toProject = await FindOrCreatePreservationCopyOfProjectAsync(
                commPkgEvent.Plant,
                commPkgEvent.ProjectName,
                cancellationToken);

            var tagsToMove = fromProject.Tags.Where(t => t.CommPkgNo == commPkgEvent.CommPkgNo).ToList();

            tagsToMove.ForEach(t => fromProject.MoveToProject(t, toProject));
        }

        private async Task ProcessTagFunctionEvent(string messageJson)
        {
            var tagFunctionEvent = JsonSerializer.Deserialize<TagFunctionTmpTopic>(messageJson);
            if (tagFunctionEvent != null && tagFunctionEvent.Behavior == "delete")
            {
                TrackDeleteEvent(PcsTopicConstants.TagFunction, tagFunctionEvent.ProCoSysGuid, false);
                return;
            }

            if (tagFunctionEvent == null ||
                tagFunctionEvent.Plant.IsEmpty() ||
                tagFunctionEvent.Code.IsEmpty() ||
                tagFunctionEvent.RegisterCode.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid TagFunctionEvent {messageJson}");
            }

            TrackTagFunctionEvent(tagFunctionEvent);

            _plantSetter.SetPlant(tagFunctionEvent.Plant);

            if (!tagFunctionEvent.CodeOld.IsEmpty() ||
                !tagFunctionEvent.RegisterCodeOld.IsEmpty())
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
            var projectEvent = JsonSerializer.Deserialize<ProjectTmpTopic>(messageJson);
            if (projectEvent != null && projectEvent.Behavior == "delete")
            {
                TrackDeleteEvent(PcsTopicConstants.Project, projectEvent.ProCoSysGuid, false);
                return;
            }
            if (projectEvent == null || projectEvent.Plant.IsEmpty() || projectEvent.ProjectName.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid ProjectEvent {messageJson}");
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
            var responsibleEvent = JsonSerializer.Deserialize<ResponsibleTmpTopic>(messageJson);
            if (responsibleEvent != null && responsibleEvent.Behavior == "delete")
            {
                TrackDeleteEvent(PcsTopicConstants.Responsible, responsibleEvent.ProCoSysGuid, false);
                return;
            }
            if (responsibleEvent == null || responsibleEvent.Plant.IsEmpty() || responsibleEvent.Code.IsEmpty())
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid ResponsibleEvent {messageJson}");
            }

            TrackResponsibleEvent(responsibleEvent);

            _plantSetter.SetPlant(responsibleEvent.Plant);

            if (!responsibleEvent.CodeOld.IsEmpty())
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

        private void TrackResponsibleEvent(ResponsibleTmpTopic responsibleEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", ResponsibleTopic.TopicName},
                    {nameof(responsibleEvent.ProCoSysGuid), responsibleEvent.ProCoSysGuid},
                    {nameof(responsibleEvent.Code), responsibleEvent.Code},
                    {nameof(responsibleEvent.Plant), responsibleEvent.Plant[4..]}, //TODO: DRY, replace with NormalizePlant
                });

        private void TrackCommPkgEvent(CommPkgTmpTopic commPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", IpoTopic.TopicName},
                    {nameof(commPkgEvent.ProCoSysGuid), commPkgEvent.ProCoSysGuid},
                    {nameof(commPkgEvent.CommPkgNo), commPkgEvent.CommPkgNo},
                    {nameof(commPkgEvent.Plant), commPkgEvent.Plant[4..]}, //TODO: DRY, replace with NormalizePlant
                    {nameof(commPkgEvent.ProjectName), commPkgEvent.ProjectName.Replace('$', '_')}, //TODO: DRY, replace with NormalizeProjectName
                    {nameof(commPkgEvent.ProjectNameOld), commPkgEvent.ProjectNameOld.Replace('$', '_')} //TODO: DRY, replace with NormalizeProjectName
                });

        private void TrackTagFunctionEvent(TagFunctionTmpTopic tagFunctionEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", TagFunctionTopic.TopicName},
                    {nameof(tagFunctionEvent.ProCoSysGuid), tagFunctionEvent.ProCoSysGuid},
                    {nameof(tagFunctionEvent.Code), tagFunctionEvent.Code},
                    {nameof(tagFunctionEvent.RegisterCode), tagFunctionEvent.RegisterCode},
                    {nameof(tagFunctionEvent.IsVoided), tagFunctionEvent.IsVoided.ToString()},
                    {nameof(tagFunctionEvent.Plant), tagFunctionEvent.Plant[4..]}, //TODO: DRY, replace with NormalizePlant
                });

        private void TrackProjectEvent(ProjectTmpTopic projectEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", ProjectTopic.TopicName},
                    {nameof(projectEvent.ProCoSysGuid), projectEvent.ProCoSysGuid},
                    {nameof(projectEvent.ProjectName), projectEvent.ProjectName},
                    {nameof(projectEvent.IsClosed), projectEvent.IsClosed.ToString()},
                    {nameof(projectEvent.Plant), projectEvent.Plant[4..]}, //TODO: DRY, replace with NormalizePlant
                });

        private void TrackMcPkgEvent(McPkgTmpTopic mcPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", McPkgTopic.TopicName},
                    {nameof(mcPkgEvent.ProCoSysGuid), mcPkgEvent.ProCoSysGuid},
                    {nameof(mcPkgEvent.McPkgNo), mcPkgEvent.McPkgNo},
                    {nameof(mcPkgEvent.Plant), mcPkgEvent.Plant[4..]}, //TODO: DRY, replace with NormalizePlant
                    {nameof(mcPkgEvent.ProjectName), mcPkgEvent.ProjectName.Replace('$', '_')} //TODO: DRY, replace with NormalizeProjectName
                });

        private void TrackTagEvent(TagTopic tagEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", TagTopic.TopicName},
                    {nameof(tagEvent.ProCoSysGuid), tagEvent.ProCoSysGuid},
                    {nameof(tagEvent.TagNo), tagEvent.TagNo},
                    {nameof(tagEvent.Plant), tagEvent.Plant[4..]},  //TODO: DRY, replace with NormalizePlant
                    {nameof(tagEvent.ProjectName), tagEvent.ProjectName.Replace('$', '_')} //TODO: DRY, replace with NormalizeProjectName
                });

        private void TrackDeleteEvent(string topic, string guid, bool supported) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event Delete", topic},
                    {"ProCoSysGuid", guid},
                    {"Supported", supported.ToString()}
                });
    }
}
