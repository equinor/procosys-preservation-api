using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.DeleteAttachment
{
    public class DeleteFieldValueAttachmentCommandHandler : IRequestHandler<DeleteFieldValueAttachmentCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public DeleteFieldValueAttachmentCommandHandler(
            IProjectRepository projectRepository, 
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IBlobStorage blobStorage, IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
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
                var fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);
                await _blobStorage.DeleteAsync(fullBlobPath, cancellationToken);
            }

            requirement.RecordAttachment(null, request.FieldId, requirementDefinition);

            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
