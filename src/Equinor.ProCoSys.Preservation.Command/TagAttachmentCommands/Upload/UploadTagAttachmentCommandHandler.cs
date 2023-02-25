using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Upload
{
    public class UploadTagAttachmentCommandHandler : IRequestHandler<UploadTagAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public UploadTagAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IBlobStorage blobStorage, IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<int>> Handle(UploadTagAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithAttachmentsByTagIdAsync(request.TagId);

            var attachment = tag.GetAttachmentByFileName(request.FileName);

            if (!request.OverwriteIfExists && attachment != null)
            {
                throw new Exception($"Tag {tag.Id} already has an attachment with filename {request.FileName}");
            }

            if (attachment == null)
            {
                attachment = new TagAttachment(
                    _plantProvider.Plant,
                    Guid.NewGuid(),
                    request.FileName);
                tag.AddAttachment(attachment);
            }

            var fullBlobPath = attachment.GetFullBlobPath(_blobStorageOptions.Value.BlobContainer);

            await _blobStorage.UploadAsync(fullBlobPath, request.Content, request.OverwriteIfExists, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
