using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Microsoft.Extensions.Options;

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
        private readonly IClaimsProvider _claimsProvider;
        private readonly IBearerTokenSetter _bearerTokenSetter;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly IProjectApiService _projectApiService;
        private Guid _synchronizationUserOid;
        private const string PreservationBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public BusReceiverService(IPlantSetter plantSetter,
            IUnitOfWork unitOfWork,
            ITelemetryClient telemetryClient,
            IResponsibleRepository responsibleRepository,
            IProjectRepository projectRepository,
            ITagFunctionRepository tagFunctionRepository,
            ICurrentUserSetter currentUserSetter,
            IClaimsProvider claimsProvider,
            IBearerTokenSetter bearerTokenSetter,
            IApplicationAuthenticator authenticator,
            IOptionsMonitor<SynchronizationOptions> options,
            IProjectApiService projectApiService)
        {
            _plantSetter = plantSetter;
            _unitOfWork = unitOfWork;
            _telemetryClient = telemetryClient;
            _responsibleRepository = responsibleRepository;
            _projectRepository = projectRepository;
            _tagFunctionRepository = tagFunctionRepository;
            _currentUserSetter = currentUserSetter;
            _claimsProvider = claimsProvider;
            _bearerTokenSetter = bearerTokenSetter;
            _authenticator = authenticator;
            _projectApiService = projectApiService;
            _synchronizationUserOid = options.CurrentValue.UserOid;
        }

        public async Task ProcessMessageAsync(PcsTopic pcsTopic, string messageJson, CancellationToken cancellationToken)
        {
            _currentUserSetter.SetCurrentUserOid(_synchronizationUserOid);

            var currentUser = _claimsProvider.GetCurrentUser();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.Oid, _synchronizationUserOid.ToString()));
            currentUser.AddIdentity(claimsIdentity);

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
                    await ProcessTagEvent(messageJson);
                    break;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessTagEvent(string messageJson)
        {
            var tagEvent = JsonSerializer.Deserialize<TagTopic>(messageJson);
            if (string.IsNullOrWhiteSpace(tagEvent.Plant) ||
                string.IsNullOrWhiteSpace(tagEvent.TagNo) ||
                string.IsNullOrWhiteSpace(tagEvent.ProjectName))
            {
                throw new ArgumentNullException($"Unable to deserialize JSON to TagEvent {messageJson}");
            }

            TrackTagEvent(tagEvent);

            _plantSetter.SetPlant(tagEvent.Plant);

            var tagToUpdateProjectName = !string.IsNullOrWhiteSpace(tagEvent.ProjectNameOld)
                ? tagEvent.ProjectNameOld
                : tagEvent.ProjectName;
            var tagToUpdateTagNo = !string.IsNullOrWhiteSpace(tagEvent.TagNoOld)
                ? tagEvent.TagNoOld
                : tagEvent.TagNo;

            var project = await _projectRepository.GetProjectWithTagsByNameAsync(tagToUpdateProjectName);
            if (project == null)
            {
                return;
            }

            var tagToUpdate = project.Tags.SingleOrDefault(t => t.TagNo == tagToUpdateTagNo);

            if (tagToUpdate != null)
            {
                if (tagToUpdate.TagNo!=tagEvent.TagNo)
                {
                    tagToUpdate.Rename(tagEvent.TagNo);
                }

                if (!string.IsNullOrWhiteSpace(tagEvent.ProjectNameOld) &&
                    tagEvent.ProjectName != tagEvent.ProjectNameOld)
                {
                    var projectToMoveTagInto = await _projectRepository.GetProjectOnlyByNameAsync(tagEvent.ProjectName);
                    if (projectToMoveTagInto == null)
                    {
                        var bearerToken = await _authenticator.GetBearerTokenForApplicationAsync();
                        _bearerTokenSetter.SetBearerToken(bearerToken, false);

                        _currentUserSetter.SetCurrentUserOid(_synchronizationUserOid);
                        var pcsProject = await _projectApiService.TryGetProjectAsync(tagEvent.Plant, tagEvent.ProjectName);
                        projectToMoveTagInto = new Project(tagEvent.Plant, pcsProject.Name, pcsProject.Description);
                        _projectRepository.Add(projectToMoveTagInto);
                    }

                    project.DetachFromProject(tagToUpdate);
                    projectToMoveTagInto.AddTag(tagToUpdate);
                }

                tagToUpdate.SetArea(tagEvent.AreaCode, tagEvent.AreaDescription);
                tagToUpdate.SetDiscipline(tagEvent.DisciplineCode, tagEvent.DisciplineDescription);
                tagToUpdate.Calloff = tagEvent.CallOffNo;
                tagToUpdate.PurchaseOrderNo = tagEvent.PurchaseOrderNo;
                tagToUpdate.CommPkgNo = tagEvent.CommPkgNo;
                tagToUpdate.Description = tagEvent.Description;
                tagToUpdate.McPkgNo = tagEvent.McPkgNo;
                tagToUpdate.TagFunctionCode = tagEvent.TagFunctionCode;
                tagToUpdate.IsVoided = tagEvent.IsVoided;
            }
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
                throw new ArgumentNullException($"Unable to deserialize JSON to McPkgEvent {messageJson}");
            }

            if (string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNoOld) || string.IsNullOrWhiteSpace(mcPkgEvent.CommPkgNoOld))
            {
                // Nothing to do
                return;
            }

            TrackMcPkgEvent(mcPkgEvent);

            _plantSetter.SetPlant(mcPkgEvent.Plant);

            var project = await _projectRepository.GetProjectWithTagsByNameAsync(mcPkgEvent.ProjectName);

            if (!string.IsNullOrWhiteSpace(mcPkgEvent.McPkgNoOld))
            {
                if (mcPkgEvent.McPkgNoOld != mcPkgEvent.McPkgNo)
                {
                    project.RenameMcPkg(mcPkgEvent.McPkgNoOld, mcPkgEvent.McPkgNo, mcPkgEvent.CommPkgNoOld);
                }
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
                throw new ArgumentNullException($"Unable to deserialize JSON to CommPkgEvent {messageJson}");
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
                throw new ArgumentNullException($"Unable to deserialize JSON to TagFunctionEven {messageJson}");
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
                throw new ArgumentNullException($"Unable to deserialize JSON to ProjectEvent {messageJson}");
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
                throw new ArgumentNullException($"Unable to deserialize JSON to ResponsibleEvent {messageJson}");
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
                    {"Event", ResponsibleTopic.TopicName},
                    {nameof(responsibleEvent.Code), responsibleEvent.Code},
                    {nameof(responsibleEvent.Plant), responsibleEvent.Plant[4..]},
                });

        private void TrackCommPkgEvent(CommPkgTopic commPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", IpoTopic.TopicName},
                    {nameof(commPkgEvent.CommPkgNo), commPkgEvent.CommPkgNo},
                    {nameof(commPkgEvent.Plant), commPkgEvent.Plant[4..]},
                    {nameof(commPkgEvent.ProjectName), commPkgEvent.ProjectName.Replace('$', '_')},
                    {nameof(commPkgEvent.ProjectNameOld), commPkgEvent.ProjectNameOld.Replace('$', '_')}
                });

        private void TrackTagFunctionEvent(TagFunctionTopic tagFunctionEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", TagFunctionTopic.TopicName},
                    {nameof(tagFunctionEvent.Code), tagFunctionEvent.Code},
                    {nameof(tagFunctionEvent.RegisterCode), tagFunctionEvent.RegisterCode},
                    {nameof(tagFunctionEvent.IsVoided), tagFunctionEvent.IsVoided.ToString()},
                    {nameof(tagFunctionEvent.Plant), tagFunctionEvent.Plant[4..]},
                });

        private void TrackProjectEvent(ProjectTopic projectEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", ProjectTopic.TopicName},
                    {nameof(projectEvent.ProjectName), projectEvent.ProjectName},
                    {nameof(projectEvent.IsClosed), projectEvent.IsClosed.ToString()},
                    {nameof(projectEvent.Plant), projectEvent.Plant[4..]},
                });

        private void TrackMcPkgEvent(McPkgTopic mcPkgEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", McPkgTopic.TopicName},
                    {nameof(mcPkgEvent.McPkgNo), mcPkgEvent.McPkgNo},
                    {nameof(mcPkgEvent.Plant), mcPkgEvent.Plant[4..]},
                    {nameof(mcPkgEvent.ProjectName), mcPkgEvent.ProjectName.Replace('$', '_')}
                });

        private void TrackTagEvent(TagTopic tagEvent) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", TagTopic.TopicName},
                    {nameof(tagEvent.TagNo), tagEvent.TagNo},
                    {nameof(tagEvent.Plant), tagEvent.Plant[4..]},
                    {nameof(tagEvent.ProjectName), tagEvent.ProjectName.Replace('$', '_')}
                });
    }
}
