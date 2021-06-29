using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.DeleteAttachment
{
    public class DeleteFieldValueAttachmentCommandHandler : IRequestHandler<DeleteFieldValueAttachmentCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<BlobStorageOptions> _blobStorageOptions;

        public DeleteFieldValueAttachmentCommandHandler(
            IProjectRepository projectRepository, 
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IBlobStorage blobStorage, IOptionsMonitor<BlobStorageOptions> blobStorageOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public async Task<Result<Unit>> Handle(DeleteFieldValueAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var requirement = tag.Requirements.Single(r => r.Id == request.RequirementId);

            var requirementDefinition =
                await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirement.RequirementDefinitionId);

            var attachment = requirement.GetAlreadyRecordedAttachment(request.FieldId, requirementDefinition);
            
            if (attachment != null)
            {
                var fullBlobPath = attachment.GetFullBlobPath(_blobStorageOptions.CurrentValue.BlobContainer);
                await _blobStorage.DeleteAsync(fullBlobPath, cancellationToken);
            }

            requirement.RecordAttachment(null, request.FieldId, requirementDefinition);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
