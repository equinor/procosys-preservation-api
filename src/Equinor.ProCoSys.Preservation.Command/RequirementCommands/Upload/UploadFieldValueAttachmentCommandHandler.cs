﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.Upload
{
    public class UploadFieldValueAttachmentCommandHandler : IRequestHandler<UploadFieldValueAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public UploadFieldValueAttachmentCommandHandler(
            IProjectRepository projectRepository, 
            IRequirementTypeRepository requirementTypeRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IAzureBlobService azureBlobService,
            IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _azureBlobService = azureBlobService;
            _blobStorageOptions = blobStorageOptions;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public async Task<Result<int>> Handle(UploadFieldValueAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithPreservationHistoryByTagIdAsync(request.TagId);
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
                fullBlobPath = attachment.GetFullBlobPath();
                await _azureBlobService.DeleteAsync(
                    _blobStorageOptions.Value.BlobContainer,
                    fullBlobPath, 
                    cancellationToken);
                attachment.SetFileName(request.FileName);
            }

            fullBlobPath = attachment.GetFullBlobPath();
            await _azureBlobService.UploadAsync(
                _blobStorageOptions.Value.BlobContainer,
                fullBlobPath, 
                request.Content,
                "application/octet-stream",
                true, 
                cancellationToken);

            requirement.RecordAttachment(attachment, request.FieldId, requirementDefinition);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
