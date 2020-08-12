using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.MainApi.Certificate;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.AutoTransfer
{
    public class AutoTransferCommandHandler : IRequestHandler<AutoTransferCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ICertificateApiService _certificateApiService;
        private readonly ILogger<AutoTransferCommandHandler> _logger;

        public AutoTransferCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ICertificateApiService certificateApiService,
            ILogger<AutoTransferCommandHandler> logger)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _certificateApiService = certificateApiService;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(AutoTransferCommand request, CancellationToken cancellationToken)
        {
            var autoTransferMethod = GetAutoTransferMethod(request.CertificateType);

            if (autoTransferMethod == AutoTransferMethod.None)
            {
                _logger.LogInformation($"Early exit in AutoTransfer handling. Nothing to do for certificate of type {request.CertificateType}");
                return new SuccessResult<Unit>(Unit.Value);
            }

            var project = await _projectRepository.GetProjectOnlyByNameAsync(request.ProjectName);
            if (project == null)
            {
                _logger.LogInformation($"Early exit in AutoTransfer handling. Project {request.ProjectName} does not exists in Preservation module");
                return new SuccessResult<Unit>(Unit.Value);
            }
            
            if (project.IsClosed)
            {
                _logger.LogInformation($"Early exit in AutoTransfer handling. Project {request.ProjectName} is closed in Preservation module");
                return new SuccessResult<Unit>(Unit.Value);
            }

            var certificateTagModel = await _certificateApiService.TryGetCertificateTagsAsync(
                _plantProvider.Plant,
                request.ProjectName,
                request.CertificateNo,
                request.CertificateType);

            if (certificateTagModel == null)
            {
                var error = $"Certificate {request.CertificateNo} of type {request.CertificateType} not found in project {request.ProjectName}";
                _logger.LogError(error);
                return new NotFoundResult<Unit>(error);
            }

            if (!certificateTagModel.CertificateIsAccepted)
            {
                _logger.LogInformation($"Early exit in AutoTransfer handling. Certificate {request.CertificateNo} of type {request.CertificateType} in project {request.ProjectName} is not Accepted");
                return new SuccessResult<Unit>(Unit.Value);
            }

            var tagNos = certificateTagModel.Tags.Select(t => t.TagNo);
            await AutoTransferTagsAsync(request.ProjectName, tagNos, autoTransferMethod);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task AutoTransferTagsAsync(string projectName, IEnumerable<string> tagNos, AutoTransferMethod autoTransferMethod)
        {
            _logger.LogDebug($"Start auto transfer of tags in project {projectName}");
            var journeys = await _journeyRepository.GetJourneysWithAutoTransferStepsAsync(autoTransferMethod);

            var autoTransferSteps = journeys.SelectMany(j => j.Steps).Where(s => s.AutoTransferMethod == autoTransferMethod);

            var autoTransferStepIds = autoTransferSteps.Select(s => s.Id);

            var tagsToTransfer = await _projectRepository.GetStandardTagsInProjectInStepsAsync(projectName, tagNos, autoTransferStepIds);

            foreach (var tag in tagsToTransfer)
            {
                var journey = journeys.Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                tag.AutoTransfer(journey, autoTransferMethod);

                _logger.LogDebug($"Tag {tag.TagNo} in project {projectName} auto transfer in journey {journey.Title}");
            }

            _logger.LogDebug($"End auto transfer of {tagsToTransfer.Count} tag(s) in project {projectName}");
        }

        private AutoTransferMethod GetAutoTransferMethod(string certificateType)
            => certificateType switch
            {
                "RFCC" => AutoTransferMethod.OnRfccSign,
                "RFOC" => AutoTransferMethod.OnRfocSign,
                _ => AutoTransferMethod.None
            };
    }
}
