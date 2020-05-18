using System;
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

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Upload
{
    public class UploadFieldValueAttachmentCommandHandler : IRequestHandler<UploadFieldValueAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public UploadFieldValueAttachmentCommandHandler(
            IProjectRepository projectRepository, 
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IBlobStorage blobStorage,
            IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public async Task<Result<int>> Handle(UploadFieldValueAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var requirement = tag.Requirements.Single(r => r.Id == request.RequirementId);

            var requirementDefinition =
                await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(requirement.RequirementDefinitionId);

            var attachment = requirement.GetAlreadyRecordedAttachment(request.FieldId, requirementDefinition);
            
            string fullBlobPath;
            if (attachment == null)
            {
                attachment = new FieldValueAttachment(_plantProvider.Plant, Guid.NewGuid(), request.FileName);
            }
            else
            {
                fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);
                await _blobStorage.DeleteAsync(fullBlobPath, cancellationToken);
                attachment.SetFileName(request.FileName);
            }

            fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);

            await _blobStorage.UploadAsync(fullBlobPath, request.Content, true, cancellationToken);

            requirement.RecordAttachment(attachment, request.FieldId, requirementDefinition);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
